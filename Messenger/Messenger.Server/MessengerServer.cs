using Messenger.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger.Server
{
    public class MessengerServer
    {
        private TcpListener tcpListener;
        private List<ClientHandler> clients = new List<ClientHandler>();
        private DatabaseManager db;
        private bool isRunning = false;
        private readonly object clientsLock = new object();
        private Dictionary<int, HashSet<int>> chatParticipantsCache = new Dictionary<int, HashSet<int>>();
        private readonly object cacheLock = new object();

        public MessengerServer(string dbPath)
        {
            db = new DatabaseManager(dbPath);
        }

        public void Start()
        {
            try
            {
                db.InitializeDatabase();
                var allChats = db.GetAllChats();
                foreach (var chat in allChats)
                {
                    UpdateChatParticipantsCache(chat.Id);
                }

                int port = 8888;
                tcpListener = new TcpListener(IPAddress.Any, port);
                tcpListener.Start();
                isRunning = true;

                Log($"Сервер запущен на порту {port}");
                Log($"Локальный IP: {GetLocalIPAddress()}");
                Log("Ожидание подключений...\n");

                var acceptThread = new Thread(AcceptClients);
                acceptThread.IsBackground = true;
                acceptThread.Start();
            }
            catch (Exception ex)
            {
                Log($"Ошибка запуска: {ex.Message}");
            }
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            return "127.0.0.1";
        }

        private void AcceptClients()
        {
            while (isRunning)
            {
                try
                {
                    var tcpClient = tcpListener.AcceptTcpClient();
                    string clientIp = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
                    Log($"Новый клиент подключен: {clientIp}. Всего: {clients.Count + 1}");
                    var clientHandler = new ClientHandler(tcpClient, this, db);
                    lock (clientsLock)
                        clients.Add(clientHandler);
                    var clientThread = new Thread(clientHandler.HandleClient);
                    clientThread.IsBackground = true;
                    clientThread.Start();
                    Log($"Новый клиент подключен. Всего: {clients.Count}");
                }
                catch (SocketException ex)
                {
                    if (isRunning)
                        Log($"Ошибка принятия клиента: {ex.Message}");
                    // При остановке сервера исключение ожидаемо, выходим
                    if (!isRunning) break;
                }
                catch (Exception ex)
                {
                    if (isRunning)
                        Log($"Ошибка принятия клиента: {ex.Message}");
                }
            }
        }

        public void BroadcastToChat(int chatId, NetworkPacket packet, int excludeUserId = -1)
        {
            List<ClientHandler> clientsCopy;
            lock (clientsLock)
            {
                clientsCopy = clients.ToList();
            }
            foreach (var client in clientsCopy)
            {
                if (client.User != null && client.User.Id != excludeUserId && UserHasAccessToChatCached(client.User.Id, chatId))
                {
                    client.SendPacket(packet);
                }
            }
        }

        public void BroadcastToDepartment(string department, NetworkPacket packet, int excludeUserId = -1)
        {
            lock (clientsLock)
            {
                foreach (var client in clients)
                {
                    if (client.User != null && client.User.Department == department && client.User.Id != excludeUserId)
                        client.SendPacket(packet);
                }
            }
        }

        public void BroadcastToUser(int userId, NetworkPacket packet)
        {
            lock (clientsLock)
            {
                var client = clients.FirstOrDefault(c => c.User?.Id == userId);
                client?.SendPacket(packet);
            }
        }

        public void RemoveClient(ClientHandler client)
        {
            lock (clientsLock)
            {
                clients.Remove(client);
                Log($"Клиент отключен. Осталось: {clients.Count}");
            }
        }

        public void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        public void Stop()
        {
            isRunning = false;
            tcpListener?.Stop();
            lock (clientsLock)
            {
                foreach (var client in clients)
                    client.Disconnect();
                clients.Clear();
            }
            tcpListener?.Stop();
            db.Close();
            Log("Сервер остановлен");
        }

        public void UpdateChatParticipantsCache(int chatId)
        {
            lock (cacheLock)
            {
                var participants = db.GetChatParticipants(chatId).Select(u => u.Id).ToHashSet();
                chatParticipantsCache[chatId] = participants;
            }
        }

        private bool UserHasAccessToChatCached(int userId, int chatId)
        {
            lock (cacheLock)
            {
                if (chatParticipantsCache.TryGetValue(chatId, out var participants))
                    return participants.Contains(userId);
            }
            // fallback – запрос в БД
            return db.UserHasAccessToChat(userId, chatId);
        }
    }
}
