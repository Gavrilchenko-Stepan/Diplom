using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Messenger.Shared;
using CommandType = Messenger.Shared.CommandType;
using System.Text.Json;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Messenger.Server
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "Messenger Server";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
  ╔═══════════════════════════════════════╗
  ║     ЛОКАЛЬНЫЙ КОРПОРАТИВНЫЙ           ║
  ║         MESSENGER - СЕРВЕР            ║
  ╚═══════════════════════════════════════╝
");
            Console.ResetColor();

            // --- Инициализация INI в папке с EXE ---
            IniFile ini = new IniFile("server.ini");
            string dbPath = ini.Read("Server", "DatabasePath", "");

            bool needPick = string.IsNullOrWhiteSpace(dbPath) || !File.Exists(dbPath);

            if (needPick)
            {
                Console.WriteLine("Требуется указать путь к базе данных.");
                using (var form = new DatabasePathForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        dbPath = form.SelectedPath;
                        ini.Write("Server", "DatabasePath", dbPath);
                        Console.WriteLine($"Путь к БД сохранён в server.ini: {dbPath}");
                    }
                    else
                    {
                        Console.WriteLine("Путь к БД не указан. Сервер не будет запущен.");
                        return;
                    }
                }
            }

            // --- Запуск сервера с указанным путём ---
            var server = new MessengerServer(dbPath);
            server.Start();

            Console.WriteLine("\nНажмите 'Q' для остановки сервера...");
            while (Console.ReadKey().Key != ConsoleKey.Q) { }

            server.Stop();
        }
    }
}
