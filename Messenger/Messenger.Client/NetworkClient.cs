using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Messenger.Shared;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Messenger.Client
{
    public class NetworkClient
    {
        private TcpClient client;
        private SslStream sslStream;
        private StreamReader reader;
        private StreamWriter writer;
        private bool isConnected;
        private Thread receiveThread;

        public string ServerIP { get; private set; }
        public event Action<NetworkPacket> OnPacketReceived;
        public event Action OnDisconnected;
        public bool IsConnected => isConnected;
        private bool _disconnecting = false;

        public async Task<bool> Connect(string serverIP, int port = 8888)
        {
            try
            {
                ServerIP = serverIP;
                client = new TcpClient();
                await client.ConnectAsync(serverIP, port);

                var networkStream = client.GetStream();
                sslStream = new SslStream(networkStream, false, (sender, cert, chain, errors) => true);
                await sslStream.AuthenticateAsClientAsync(serverIP);

                reader = new StreamReader(sslStream, Encoding.UTF8);
                writer = new StreamWriter(sslStream, Encoding.UTF8) { AutoFlush = true };

                isConnected = true;
                receiveThread = new Thread(ReceiveLoop);
                receiveThread.IsBackground = true;
                receiveThread.Start();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения: {ex.Message}");
                return false;
            }
        }

        private void ReceiveLoop()
        {
            while (isConnected && client.Connected)
            {
                try
                {
                    // SslStream не имеет DataAvailable, поэтому просто читаем строку (блокируется)
                    // Для синхронного чтения используем reader.ReadLine()
                    string json = reader.ReadLine();
                    if (!string.IsNullOrEmpty(json))
                    {
                        Console.WriteLine($"Получен JSON: {json}");
                        var packet = JsonSerializer.Deserialize<NetworkPacket>(json);
                        OnPacketReceived?.Invoke(packet);
                    }
                }
                catch (IOException)
                {
                    // Соединение разорвано
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка чтения: {ex.Message}");
                    break;
                }
            }
            Disconnect();
        }

        public void SendPacket(NetworkPacket packet)
        {
            try
            {
                if (isConnected && client.Connected)
                {
                    string json = JsonSerializer.Serialize(packet);
                    writer.WriteLine(json);
                }
            }
            catch
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            if (_disconnecting || !isConnected) return;
            _disconnecting = true;

            try
            {
                if (isConnected)
                {
                    SendPacket(new NetworkPacket { Command = CommandType.Logout });
                }
                isConnected = false;
                reader?.Close();
                writer?.Close();
                sslStream?.Close();
                client?.Close();
            }
            catch { }
            OnDisconnected?.Invoke();
            _disconnecting = false;
        }
    }
}
