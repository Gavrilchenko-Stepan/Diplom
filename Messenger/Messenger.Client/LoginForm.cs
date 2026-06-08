using Messenger.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Messenger.Client
{
    public partial class LoginForm : Form
    {
        private NetworkClient networkClient;
        public User CurrentUser { get; private set; }
        public NetworkClient NetworkClient => networkClient;
        private IniFile ini;
        private const string INI_SECTION = "Client";
        private const string KEY_SERVER = "ServerAddress";

        public LoginForm()
        {
            InitializeComponent();
            ApplyFuturisticStyle();

            UIHelper.SetRoundedRegion(this, 20);

            // --- Хранение в AppData ---
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configDir = Path.Combine(appData, "MessengerClient");
            Directory.CreateDirectory(configDir);
            string iniPath = Path.Combine(configDir, "client.ini");
            ini = new IniFile(iniPath);

            // Загружаем сохранённый адрес сервера (если есть)
            string savedServer = ini.Read(INI_SECTION, KEY_SERVER, "");
            if (!string.IsNullOrEmpty(savedServer))
                txtServerIP.Text = savedServer;
            else if (string.IsNullOrWhiteSpace(txtServerIP.Text))
                txtServerIP.Text = Environment.MachineName;

            networkClient = new NetworkClient();
            if (string.IsNullOrWhiteSpace(txtServerIP.Text))
                txtServerIP.Text = Environment.MachineName;
            networkClient.OnPacketReceived += OnPacketReceived;
            networkClient.OnDisconnected += OnDisconnected;
            this.AcceptButton = btnLogin;
            btnLogin.Click += BtnLogin_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void ApplyFuturisticStyle()
        {
            this.BackColor = Color.FromArgb(30, 30, 46);
            this.ForeColor = Color.White;
            panelMain.BackColor = Color.FromArgb(45, 45, 58);
            lblTitle.ForeColor = Color.FromArgb(0, 229, 255);
            lblSubtitle.ForeColor = Color.FromArgb(180, 180, 200);
            lblServer.ForeColor = Color.White;
            lblUsername.ForeColor = Color.White;
            lblPassword.ForeColor = Color.White;

            txtServerIP.BackColor = Color.FromArgb(60, 60, 80);
            txtServerIP.ForeColor = Color.White;
            txtServerIP.BorderStyle = BorderStyle.FixedSingle;

            txtUsername.BackColor = Color.FromArgb(60, 60, 80);
            txtUsername.ForeColor = Color.White;
            txtUsername.BorderStyle = BorderStyle.FixedSingle;

            txtPassword.BackColor = Color.FromArgb(60, 60, 80);
            txtPassword.ForeColor = Color.White;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;

            btnLogin.BackColor = Color.FromArgb(0, 229, 255);
            btnLogin.ForeColor = Color.Black;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;

            btnCancel.BackColor = Color.Transparent;
            btnCancel.ForeColor = Color.FromArgb(0, 229, 255);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(0, 229, 255);
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServerIP.Text) || string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowError("Заполните все поля");
                return;
            }

            btnLogin.Enabled = false;
            lblStatus.Text = "Подключение...";
            lblStatus.Visible = true;
            lblError.Visible = false;

            var connectTask = networkClient.Connect(txtServerIP.Text);
            var timeoutTask = Task.Delay(5000);
            var completedTask = await Task.WhenAny(connectTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                ShowError("Превышен таймаут подключения к серверу");
                networkClient.Disconnect();
                btnLogin.Enabled = true;
                return;
            }

            bool connected = await networkClient.Connect(txtServerIP.Text);
            if (!connected)
            {
                ShowError("Не удалось подключиться к серверу");
                btnLogin.Enabled = true;
                return;
            }

            lblStatus.Text = "Отправка данных...";
            networkClient.SendPacket(new NetworkPacket
            {
                Command = Shared.CommandType.Login,
                Data = new { username = txtUsername.Text, password = txtPassword.Text }
            });

            string hashedPassword = SecurityHelper.HashPassword(txtPassword.Text);

            networkClient.SendPacket(new NetworkPacket
            {
                Command = Shared.CommandType.Login,
                Data = new { username = txtUsername.Text, password = hashedPassword }
            });
        }

        private void OnPacketReceived(NetworkPacket packet)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<NetworkPacket>(OnPacketReceived), packet);
                return;
            }

            if (packet.Command == Shared.CommandType.LoginResponse)
            {
                var json = (JsonElement)packet.Data;
                bool success = json.GetProperty("success").GetBoolean();
                if (success)
                {
                    ini.Write(INI_SECTION, KEY_SERVER, txtServerIP.Text);

                    var userElem = json.GetProperty("user");
                    CurrentUser = JsonSerializer.Deserialize<User>(userElem.GetRawText());
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    string msg = json.GetProperty("message").GetString();
                    ShowError(msg);
                    networkClient.Disconnect();
                    btnLogin.Enabled = true;
                }
            }
        }

        private void OnDisconnected()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(OnDisconnected));
                return;
            }
            if (!IsDisposed)
            {
                ShowError("Соединение потеряно");
                btnLogin.Enabled = true;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e) => Close();

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
            lblStatus.Visible = false;
        }
    }
}
