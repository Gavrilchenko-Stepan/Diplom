using Messenger.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
    public partial class MainForm : Form
    {
        private User currentUser = null;
        private NetworkClient networkClient = null;
        private List<Chat> chats = new List<Chat>();
        private Chat currentChat;
        private Dictionary<int, List<Shared.Message>> messages = new Dictionary<int, List<Shared.Message>>();
        private Timer refreshTimer;
        private Font boldFont11;
        private Font normalFont9;
        private Font smallFont8;
        private Font iconFont;
        private bool _isUpdatingList = false;
        private Font messageSenderFont;
        private Font messageTextFont;
        private Font messageTimeFont;
        private Font dateFont;
        private int hoveredChatIndex = -1;

        private List<int> selectedMessageIndices = new List<int>();
        private int lastSelectedIndex = -1;

        private Shared.Message editingMessage = null;
        private string originalMessageText = "";

        private Timer typingTimer;
        private bool isTypingSent = false;
        private Timer clearTypingTimer;
        private const int avatarSize = 32;
        private const int messageMaxWidth = 400;
        private const int messagePadding = 10;
        private const int messageMargin = 20;

        private IniFile ini;
        private const string INI_SECTION = "MainForm";
        private const string KEY_LAST_CHAT = "LastChatId";
        private ToolTip chatToolTip;
        private bool _disconnectedShown = false;

        public MainForm()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.WindowState = FormWindowState.Normal;

            this.Load += (s, e) => UpdateFormRegion();
            this.Resize += (s, e) => UpdateFormRegion();
            this.ResizeEnd += (s, e) => UpdateFormRegion();

            this.Resize += (sender, e) =>
            {
                Debug.WriteLine($"panelTop.Width = {panelTop.Width}, ClientSize.Width = {this.ClientSize.Width}");
                UpdateButtonsPosition();
            };

            this.Shown += (s, e) => UpdateButtonsPosition();

            picOnlineIndicator.BringToFront();

            // Скругление углов верхней панели
            UIHelper.SetRoundedRegion(this, 20);

            /// Скругление аватарки чата
            picChatAvatar.SizeMode = PictureBoxSizeMode.StretchImage;
            picChatAvatar.Paint += (s, e) =>
            {
                using (var graphicsPath = new GraphicsPath())
                {
                    graphicsPath.AddEllipse(0, 0, picChatAvatar.Width, picChatAvatar.Height);
                    picChatAvatar.Region = new Region(graphicsPath);
                }
            };
            picChatAvatar.Resize += (s, e) => picChatAvatar.Invalidate();

            // Таймер для отправки "печатает"
            typingTimer = new Timer { Interval = 1500 };
            typingTimer.Tick += (s, e) =>
            {
                if (isTypingSent) SendTypingStop();
                typingTimer.Stop();
                isTypingSent = false;
            };

            clearTypingTimer = new Timer { Interval = 4000 };
            clearTypingTimer.Tick += (s, e) => { HideTypingIndicator(); clearTypingTimer.Stop(); };

            picUserAvatar.Paint += PicUserAvatar_Paint;

            // Создание круглой области для онлайн-индикатора
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, picOnlineIndicator.Width, picOnlineIndicator.Height);
            picOnlineIndicator.Region = new Region(path);
            picOnlineIndicator.BackColor = Color.Gray; // временно, потом обновится

            // Подписка на событие Resize для динамического позиционирования кнопок
            this.Resize += (s, e) => UpdateButtonsPosition();

            // Настройка шрифтов для сообщений
            messageSenderFont = new Font("Segoe UI", 9, FontStyle.Bold);
            messageTextFont = new Font("Segoe UI", 10);
            messageTimeFont = new Font("Segoe UI", 8);
            dateFont = new Font("Segoe UI", 9, FontStyle.Bold);

            // Двойная буферизация для списка чатов
            typeof(ListBox).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null, lstChats, new object[] { true });

            // Настройка поля ввода
            txtMessage.Multiline = true;
            txtMessage.WordWrap = true;
            txtMessage.ScrollBars = ScrollBars.Vertical;
            txtMessage.AcceptsReturn = true;
            txtMessage.TextChanged += TxtMessage_TextChanged;

            // Шрифты для списка чатов
            boldFont11 = new Font("Segoe UI", 11, FontStyle.Bold);
            normalFont9 = new Font("Segoe UI", 9);
            smallFont8 = new Font("Segoe UI", 8);
            try { iconFont = new Font("Segoe UI Emoji", 16); }
            catch { iconFont = new Font("Segoe UI", 16); }

            // Настройка списка чатов (фиксированная высота элементов)
            lstChats.DrawMode = DrawMode.OwnerDrawFixed;
            lstChats.DrawItem += LstChats_DrawItem;

            // Настройка списка сообщений (динамическая высота элементов)
            lstMessages.DrawMode = DrawMode.OwnerDrawVariable;
            lstMessages.MeasureItem += LstMessages_MeasureItem;
            lstMessages.DrawItem += LstMessages_DrawItem;
            lstMessages.Resize += (s, e) => lstMessages.Refresh();
            lstMessages.KeyDown += LstMessages_KeyDown;
            lstMessages.MouseDown += LstMessages_MouseDown;


            // Подписка на события
            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
            this.btnNewChat.Click += BtnNewChat_Click;
            this.btnSend.Click += BtnSend_Click;
            this.lstChats.SelectedIndexChanged += LstChats_SelectedIndexChanged;
            this.txtSearchChats.TextChanged += TxtSearchChats_TextChanged;
            this.txtSearchChats.Enter += TxtSearchChats_Enter;
            this.txtSearchChats.Leave += TxtSearchChats_Leave;
            this.btnViewParticipants.Click += BtnViewParticipants_Click;
            lstMessages.DoubleClick += LstMessages_DoubleClick;
            this.btnSettings.Click += (s, e) => cmsSettingsMenu.Show(btnSettings, new Point(0, btnSettings.Height));

            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;

            lstChats.MouseMove += LstChats_MouseMove;
            lstChats.MouseLeave += (s, e) => { hoveredChatIndex = -1; lstChats.Invalidate(); };

            // 1. Очистка поля по клику на крестик
            lblClearSearch.Click += (sender, e) =>
            {
                txtSearchChats.Text = "";
                txtSearchChats.Focus();
            };

            // 2. При изменении текста — показываем/скрываем крестик
            txtSearchChats.TextChanged += (sender, e) =>
            {
                bool hasText = !string.IsNullOrEmpty(txtSearchChats.Text) && txtSearchChats.Text != "Поиск чатов...";
                lblClearSearch.Visible = hasText;
            };

            // 3. При потере фокуса — если поле пустое или содержит плейсхолдер, скрываем крестик
            txtSearchChats.Leave += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearchChats.Text) || txtSearchChats.Text == "Поиск чатов...")
                {
                    lblClearSearch.Visible = false;
                }
            };

            // 4. Для красоты — меняем цвет крестика при наведении мыши
            lblClearSearch.MouseEnter += (sender, e) => lblClearSearch.ForeColor = Color.White;
            lblClearSearch.MouseLeave += (sender, e) => lblClearSearch.ForeColor = Color.Gray;

            var contextMenu = new ContextMenuStrip();
            var leaveChatMenuItem = new ToolStripMenuItem("🚪 Покинуть чат");
            leaveChatMenuItem.Click += LeaveChatMenuItem_Click;
            contextMenu.Items.Add(leaveChatMenuItem);
            lstChats.ContextMenuStrip = contextMenu;

            // Подписка на событие Opening для динамической блокировки пункта меню
            contextMenu.Opening += (s, ev) =>
            {
                if (lstChats.SelectedItem is Chat chat && currentUser != null)
                {
                    leaveChatMenuItem.Enabled = !(chat.Type == ChatType.Department && !currentUser.IsAdmin);
                }
            };

            ShowChat(false);

            chatToolTip = new ToolTip();
            chatToolTip.InitialDelay = 1000;
            lstChats.MouseMove += LstChats_MouseMoveForToolTip;
            lstChats.MouseLeave += (s, e) => chatToolTip.SetToolTip(lstChats, null);
        }

        private void UpdateFormRegion()
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                // В полноэкранном режиме убираем скругление
                this.Region = null;
            }
            else
            {
                int radius = 20;
                // Чтобы радиус не превышал половину меньшей стороны окна
                int maxRadius = Math.Min(this.ClientSize.Width, this.ClientSize.Height) / 2;
                if (radius > maxRadius) radius = maxRadius;
                UIHelper.SetRoundedRegion(this, 20);
            }
        }

        private void SendTypingStart()
        {
            var data = new Dictionary<string, object>
    {
        { "chatId", currentChat.Id },
        { "userId", currentUser.Id },
        { "isTyping", true }
    };
            networkClient.SendPacket(new NetworkPacket { Command = Shared.CommandType.TypingStatus, Data = data });
        }

        private void SendTypingStop()
        {
            var data = new Dictionary<string, object>
    {
        { "chatId", currentChat.Id },
        { "userId", currentUser.Id },
        { "isTyping", false }
    };
            networkClient.SendPacket(new NetworkPacket { Command = Shared.CommandType.TypingStatus, Data = data });
        }

        private void ShowTypingIndicator(int userId)
        {
            var user = currentChat.Participants?.FirstOrDefault(p => p.Id == userId);
            if (user != null)
            {
                lblTyping.Text = $"{user.FullName} печатает...";
                lblTyping.Visible = true;
                clearTypingTimer.Stop();
                clearTypingTimer.Start();
            }
        }

        private void HideTypingIndicator()
        {
            lblTyping.Visible = false;
            clearTypingTimer.Stop();
        }

        private void LstChats_MouseMove(object sender, MouseEventArgs e)
        {
            int index = lstChats.IndexFromPoint(e.Location);
            if (index != hoveredChatIndex)
            {
                hoveredChatIndex = index;
                lstChats.Invalidate();
            }
        }

        private void LstMessages_DoubleClick(object sender, EventArgs e)
        {
            Point mousePos = lstMessages.PointToClient(Cursor.Position);
            int index = lstMessages.IndexFromPoint(mousePos);
            if (index < 0) return;
            if (lstMessages.Items[index] is Shared.Message msg)
            {
                StartEditingMessage(msg);
            }
        }

        // ========== Загрузка формы ==========
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Hide();
            ShowLoginForm();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (MessageBox.Show("Выйти из приложения?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    refreshTimer?.Stop();
                    networkClient?.Disconnect();
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                refreshTimer?.Stop();
                networkClient?.Disconnect();
            }
        }

        private void ShowLoginForm()
        {
            // Отключаем старый клиент, если он существует
            if (networkClient != null)
            {
                networkClient.OnPacketReceived -= OnPacketReceived;
                networkClient.OnDisconnected -= OnDisconnected;
                networkClient.Disconnect();
            }

            using (var loginForm = new LoginForm())
            {
                var result = loginForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    currentUser = loginForm.CurrentUser;
                    networkClient = loginForm.NetworkClient;
                    networkClient.OnPacketReceived += OnPacketReceived;
                    networkClient.OnDisconnected += OnDisconnected;

                    string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string configDir = Path.Combine(appData, "MessengerClient");
                    Directory.CreateDirectory(configDir);
                    string iniPath = Path.Combine(configDir, "client.ini");
                    ini = new IniFile(iniPath);

                    UpdateUIAfterLogin();
                    LoadChats();
                    refreshTimer = new Timer { Interval = 3000 };
                    refreshTimer.Tick += (s, e) => LoadChats();
                    refreshTimer.Start();
                    this.Show();
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        private void UpdateUIAfterLogin()
        {
            // 1. Текстовая информация
            lblUserName.Text = $"👤 {currentUser.FullName}";
            lblUserDepartment.Text = $"🏢 Отдел: {currentUser.Department ?? "—"}";
            lblUserPosition.Text = $"💼 Должность: {currentUser.Position ?? "—"}";

            // 2. Аватар (48x48 с инициалами)
            string initials = GetInitials(currentUser.FullName);
            Bitmap bmp = new Bitmap(48, 48);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                UIHelper.DrawAvatar(g, new Rectangle(0, 0, 48, 48), initials);
            }
            picUserAvatar.Image?.Dispose();
            picUserAvatar.Image = bmp;
            picUserAvatar.SizeMode = PictureBoxSizeMode.Zoom;

            // 3. Онлайн-индикатор
            UpdateOnlineIndicator();

            // 4. Статусная строка (оставляем как было)
            lblConnectionStatus.Text = "● Подключено к серверу";
            lblConnectionStatus.ForeColor = Color.FromArgb(76, 175, 80);
            lblServerInfo.Text = $"Сервер: {networkClient.ServerIP}:8888";

            // 5. Кнопка админа
            tsmiAdminPanel.Visible = currentUser.IsAdmin;

            // 6. Клик по аватару (открытие профиля)
            picUserAvatar.Cursor = Cursors.Hand;
            picUserAvatar.Click += (s, e) =>
            {
                using (var profileForm = new UserProfileForm(currentUser, networkClient))
                    profileForm.ShowDialog();
            };
        }

        // Вспомогательный метод для инициалов
        private string GetInitials(string fullName)
        {
            var parts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
                return parts[0][0].ToString().ToUpper();
            else if (parts.Length >= 2)
                return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
            return "?";
        }

        // ========== Сетевые методы ==========
        private void LoadChats()
        {
            if (networkClient?.IsConnected == true)
                networkClient.SendPacket(new NetworkPacket { Command = Shared.CommandType.GetChats, UserId = currentUser.Id });
            else
                Console.WriteLine("LoadChats: клиент не подключён");
        }

        private void OnPacketReceived(NetworkPacket packet)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<NetworkPacket>(OnPacketReceived), packet);
                return;
            }

            try
            {
                switch (packet.Command)
                {
                    case Shared.CommandType.ChatsList:
                        var jsonElemChats = (JsonElement)packet.Data;
                        string jsonChats = jsonElemChats.GetRawText();
                        chats = JsonSerializer.Deserialize<List<Chat>>(jsonChats);

                        // Преобразование имён личных чатов
                        foreach (var chat in chats)
                        {
                            if (chat.Type == ChatType.Private && chat.Participants != null)
                            {
                                var other = chat.Participants.FirstOrDefault(p => p.Id != currentUser.Id);
                                if (other != null)
                                    chat.Name = other.FullName;
                            }
                        }

                        UpdateChatsList();
                        UpdateTotalUsers();
                        UpdateFooterStats();
                        ApplySearchFilter();

                        // Проверка на удаление текущего чата
                        if (currentChat != null && !chats.Any(c => c.Id == currentChat.Id))
                        {
                            currentChat = null;
                            lblChatName.Text = "Выберите чат";
                            lstMessages.Items.Clear();
                            btnSend.Enabled = false;
                            ShowChat(false);
                        }

                        if (currentChat != null)
                        {
                            var refreshedChat = chats.FirstOrDefault(c => c.Id == currentChat.Id);
                            if (refreshedChat != null)
                            {
                                currentChat = refreshedChat;
                                UpdateCurrentChatHeader();
                            }
                        }

                        // Восстанавливаем последний чат
                        int lastId = LoadLastChatId();
                        if (lastId > 0)
                        {
                            var lastChat = chats.FirstOrDefault(c => c.Id == lastId);
                            if (lastChat != null)
                                lstChats.SelectedItem = lastChat;
                            else
                                ShowChat(false);
                        }
                        else
                        {
                            ShowChat(false);
                        }
                        break;

                    case Shared.CommandType.MessagesList:
                        var jsonElemMsgs = (JsonElement)packet.Data;
                        string jsonMsgs = jsonElemMsgs.GetRawText();
                        List<Shared.Message> msgs = null;
                        try
                        {
                            msgs = JsonSerializer.Deserialize<List<Shared.Message>>(jsonMsgs);
                        }
                        catch (JsonException ex)
                        {
                            Console.WriteLine($"Ошибка десериализации списка сообщений: {ex.Message}");
                            return;
                        }
                        if (msgs.Any())
                        {
                            int chatId = msgs.First().ChatId;
                            Debug.WriteLine($"Получен список сообщений для чата {chatId}, всего {msgs.Count}");
                            messages[chatId] = msgs;
                            if (currentChat != null && chatId == currentChat.Id)
                                DisplayMessages();
                        }
                        else
                        {
                            if (currentChat != null)
                            {
                                messages[currentChat.Id] = new List<Shared.Message>();
                                DisplayMessages();
                            }
                        }
                        break;

                    case Shared.CommandType.NewMessage:
                        var jsonElemNewMsg = (JsonElement)packet.Data;
                        string jsonNewMsg = jsonElemNewMsg.GetRawText();
                        var newMsg = JsonSerializer.Deserialize<Shared.Message>(jsonNewMsg);
                        Debug.WriteLine($"Получено новое сообщение: '{newMsg.Text}' в чат {newMsg.ChatId}");
                        HandleNewMessage(newMsg);
                        break;

                    case Shared.CommandType.UserStatusChanged:
                        var jsonElemUser = (JsonElement)packet.Data;
                        string jsonUser = jsonElemUser.GetRawText();
                        var user = JsonSerializer.Deserialize<User>(jsonUser);
                        Debug.WriteLine($"UserStatusChanged: {user.FullName} is {user.IsOnline}");

                        // Если изменился статус текущего пользователя
                        if (user.Id == currentUser.Id)
                        {
                            currentUser.IsOnline = user.IsOnline;
                            currentUser.LastSeen = user.LastSeen;
                            UpdateOnlineIndicator();
                        }

                        // Остальная логика обновления статусов в чатах
                        UpdateUserStatus(user);
                        UpdateFooterStats();
                        break;

                    case Shared.CommandType.ChatCreated:
                        var jsonElemChatCreated = (JsonElement)packet.Data;
                        string jsonChatCreated = jsonElemChatCreated.GetRawText();
                        var newChat = JsonSerializer.Deserialize<Chat>(jsonChatCreated);

                        if (newChat.Type == ChatType.Private && newChat.Participants != null)
                        {
                            var other = newChat.Participants.FirstOrDefault(p => p.Id != currentUser.Id);
                            if (other != null)
                                newChat.Name = other.FullName;
                        }

                        if (!chats.Any(c => c.Id == newChat.Id))
                        {
                            chats.Add(newChat);
                            currentChat = newChat;
                            UpdateChatsList();
                        }
                        break;

                    case Shared.CommandType.ChatUpdated:
                        var jsonChatUpdated = (JsonElement)packet.Data;
                        string jsonUpdated = jsonChatUpdated.GetRawText();
                        var updatedChat = JsonSerializer.Deserialize<Chat>(jsonUpdated);

                        var existing = chats.FirstOrDefault(c => c.Id == updatedChat.Id);
                        if (existing != null)
                        {
                            existing.Name = updatedChat.Name;
                            existing.Participants = updatedChat.Participants;
                            if (currentChat?.Id == updatedChat.Id)
                            {
                                currentChat.Participants = updatedChat.Participants;
                                UpdateCurrentChatHeader();
                            }
                            UpdateChatsList();
                        }
                        break;

                    case Shared.CommandType.MessageDeleted:
                        int deletedId = ((JsonElement)packet.Data).GetInt32();
                        HandleMessageDeleted(deletedId);
                        break;

                    case Shared.CommandType.MessageEdited:
                        var jsonEdited = (JsonElement)packet.Data;
                        var editedMsg = JsonSerializer.Deserialize<Shared.Message>(jsonEdited.GetRawText());
                        Invoke(new Action(() => HandleMessageEdited(editedMsg)));
                        break;
                    case Shared.CommandType.TypingStatus:
                        if (packet.Data is JsonElement jsonTyping && jsonTyping.ValueKind == JsonValueKind.Object)
                        {
                            try
                            {
                                var typingData = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonTyping.GetRawText());
                                if (typingData != null &&
                                    typingData.TryGetValue("chatId", out var chatIdObj) &&
                                    typingData.TryGetValue("userId", out var userIdObj) &&
                                    typingData.TryGetValue("isTyping", out var isTypingObj))
                                {
                                    int chatId = Convert.ToInt32(chatIdObj);
                                    int userId = Convert.ToInt32(userIdObj);
                                    bool isTyp = Convert.ToBoolean(isTypingObj);
                                    if (currentChat?.Id == chatId && userId != currentUser.Id)
                                    {
                                        if (isTyp) ShowTypingIndicator(userId);
                                        else HideTypingIndicator();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Ошибка обработки TypingStatus: {ex.Message}");
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки: {ex.Message}");
            }
        }

        private void OnDisconnected()
        {
            if (_disconnectedShown) return;
            _disconnectedShown = true;

            if (InvokeRequired)
            {
                Invoke(new Action(OnDisconnected));
                return;
            }
            lblConnectionStatus.Text = "● Отключено";
            lblConnectionStatus.ForeColor = Color.Red;
            btnSend.Enabled = false;
            refreshTimer?.Stop();
            MessageBox.Show("Соединение с сервером потеряно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // ========== Обновление списка чатов ==========
        private void UpdateChatsList()
        {
            if (_isUpdatingList) return;
            _isUpdatingList = true;

            int topIndex = lstChats.TopIndex;

            lstChats.BeginUpdate();
            try
            {
                lstChats.Items.Clear();
                var sorted = chats.OrderByDescending(c => c.LastMessageTime).ToList();
                foreach (var chat in sorted)
                    lstChats.Items.Add(chat);

                if (currentChat != null)
                {
                    // Пытаемся восстановить выделение текущего чата
                    for (int i = 0; i < lstChats.Items.Count; i++)
                    {
                        if (((Chat)lstChats.Items[i]).Id == currentChat.Id)
                        {
                            lstChats.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    // Явно сбрасываем интерфейс в состояние "нет выбранного чата"
                    lstChats.SelectedIndex = -1;
                    lblChatName.Text = "Выберите чат";
                    lstMessages.Items.Clear();
                    btnSend.Enabled = false;
                }

                // Восстанавливаем позицию прокрутки
                if (topIndex >= 0 && topIndex < lstChats.Items.Count)
                    lstChats.TopIndex = topIndex;
                else if (lstChats.Items.Count > 0)
                    lstChats.TopIndex = 0;
            }
            finally
            {
                lstChats.EndUpdate();
                _isUpdatingList = false;
            }
        }

        private void UpdateTotalUsers()
        {
            var ids = new HashSet<int>();
            foreach (var chat in chats)
                foreach (var u in chat.Participants)
                    ids.Add(u.Id);
            lblTotalUsers.Text = $"Всего пользователей: {ids.Count}";
        }

        private void LstChats_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingList) return;

            if (!(lstChats.SelectedItem is Chat chat)) return;
            currentChat = chat;

            SaveLastChat();
            ShowChat(true);

            chat.UnreadCount = 0;
            UpdateChatsList();
            btnSend.Enabled = true;

            if (messages.ContainsKey(chat.Id))
                messages.Remove(chat.Id);
            lstMessages.Items.Clear();
            networkClient.SendPacket(new NetworkPacket { Command = Shared.CommandType.GetMessages, UserId = currentUser.Id, Data = chat.Id });

            UpdateCurrentChatHeader();

            if (picChatAvatar.Image != null)
            {
                picChatAvatar.Image.Dispose();
                picChatAvatar.Image = null;
            }

            int size = 50;
            string avatarText = "?";
            if (currentChat.Type == ChatType.Private && currentChat.Participants != null)
            {
                var other = currentChat.Participants.FirstOrDefault(p => p.Id != currentUser.Id);
                if (other != null) avatarText = GetInitials(other.FullName);
            }
            else if (!string.IsNullOrWhiteSpace(currentChat.Name))
            {
                avatarText = currentChat.Name[0].ToString().ToUpper();
            }
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                UIHelper.DrawAvatar(g, new Rectangle(0, 0, size, size), avatarText);
            }
            picChatAvatar.Image?.Dispose();
            picChatAvatar.Image = bmp;
        }

        // ========== Отображение сообщений ==========
        private void DisplayMessages()
        {
            selectedMessageIndices.Clear();
            lastSelectedIndex = -1;
            lstMessages.Items.Clear();
            if (!messages.ContainsKey(currentChat.Id)) return;

            var msgs = messages[currentChat.Id];
            DateTime? lastDate = null;
            foreach (var msg in msgs)
            {
                if (lastDate == null || msg.SentAt.Date != lastDate.Value.Date)
                {
                    lstMessages.Items.Add(msg.SentAt.ToString("d MMMM yyyy"));
                    lastDate = msg.SentAt.Date;
                }
                lstMessages.Items.Add(msg);
            }
            if (lstMessages.Items.Count > 0)
                lstMessages.TopIndex = lstMessages.Items.Count - 1;

            if (msgs.Any())
            {
                int lastId = msgs.Max(m => m.Id);
                SendReadReceipt(currentChat.Id, lastId);
            }
        }

        private void HandleNewMessage(Shared.Message msg)
        {
            if (messages.ContainsKey(msg.ChatId) && messages[msg.ChatId].Any(m => m.Id == msg.Id))
                return;

            // Добавляем в словарь
            if (!messages.ContainsKey(msg.ChatId))
                messages[msg.ChatId] = new List<Shared.Message>();
            messages[msg.ChatId].Add(msg);

            var chat = chats.FirstOrDefault(c => c.Id == msg.ChatId);
            if (chat != null)
            {
                chat.LastMessage = msg.Text;
                chat.LastMessageTime = msg.SentAt;
                if (currentChat?.Id != msg.ChatId)
                    chat.UnreadCount++;
                UpdateChatsList();

                // Обновляем UI текущего чата
                if (currentChat?.Id == msg.ChatId)
                {
                    lstMessages.BeginUpdate();
                    try
                    {
                        // Добавляем разделитель даты, если нужно
                        DateTime? lastDate = null;
                        if (lstMessages.Items.Count > 0)
                        {
                            object lastItem = lstMessages.Items[lstMessages.Items.Count - 1];
                            if (lastItem is string lastStr)
                                DateTime.TryParse(lastStr, out var parsed);
                            else if (lastItem is Shared.Message lastMsg)
                                lastDate = lastMsg.SentAt.Date;
                        }
                        if (lastDate == null || msg.SentAt.Date != lastDate.Value.Date)
                        {
                            lstMessages.Items.Add(msg.SentAt.ToString("d MMMM yyyy"));
                        }
                        lstMessages.Items.Add(msg);
                    }
                    finally
                    {
                        lstMessages.EndUpdate();
                    }
                    // Принудительная перерисовка и прокрутка вниз
                    lstMessages.Refresh();
                    if (lstMessages.Items.Count > 0)
                        lstMessages.TopIndex = lstMessages.Items.Count - 1;

                    SendReadReceipt(msg.ChatId, msg.Id);
                }
            }
        }

        private void UpdateUserStatus(User user)
        {
            Debug.WriteLine($"UpdateUserStatus: получили статус для {user.FullName} (Id={user.Id}) - онлайн={user.IsOnline}");
            bool updated = false;
            foreach (var chat in chats)
            {
                var p = chat.Participants?.FirstOrDefault(x => x.Id == user.Id);
                if (p != null)
                {
                    Debug.WriteLine($"  найден в чате {chat.Name}, старый статус {p.IsOnline}, новый {user.IsOnline}");
                    p.IsOnline = user.IsOnline;
                    p.LastSeen = user.LastSeen;
                    updated = true;
                }
            }
            if (updated)
            {
                Debug.WriteLine("  Обновляем список чатов...");
                UpdateChatsList();
            }
            else
            {
                Debug.WriteLine("  Участник не найден ни в одном чате");
            }

            // Обновляем заголовок текущего чата, если он личный и статус изменился именно у собеседника
            if (currentChat?.Type == ChatType.Private)
            {
                var other = currentChat.Participants?.FirstOrDefault(p => p.Id == user.Id);
                if (other != null)
                {
                    UpdateCurrentChatHeader(); // теперь вся информация о статусе берётся из currentChat.Participants
                }
            }
        }

        private void BtnSend_Click(object sender, EventArgs e) => SendOrEdit();

        private void TxtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                SendOrEdit();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter && e.Shift)
            {
                // Вставляем новую строку в позицию курсора
                int pos = txtMessage.SelectionStart;
                txtMessage.Text = txtMessage.Text.Insert(pos, Environment.NewLine);
                txtMessage.SelectionStart = pos + Environment.NewLine.Length;
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape && editingMessage != null)
            {
                CancelEditing();
                e.Handled = true;
            }
        }

        // ========== Создание нового чата ==========
        private void BtnNewChat_Click(object sender, EventArgs e)
        {
            using (var form = new NewChatForm(currentUser.Id, currentUser.Department, networkClient, currentUser.IsAdmin))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Console.WriteLine("Чат создан");
                }
            }
        }

        // ========== Админ-панель ==========
        private void BtnAdminPanel_Click(object sender, EventArgs e)
        {
            using (var form = new AdminDashboardForm(networkClient, currentUser))
            {
                form.ShowDialog();
            }
        }

        // ========== Выход ==========
        private void BtnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Выйти из системы?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                refreshTimer?.Stop();
                networkClient?.Disconnect();
                if (networkClient != null)
                {
                    networkClient.OnPacketReceived -= OnPacketReceived;
                    networkClient.OnDisconnected -= OnDisconnected;
                }
                this.Hide();
                ShowLoginForm();
            }
        }

        // ========== Поиск чатов ==========
        private void TxtSearchChats_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearchChats.Text.ToLower().Trim();

            int topIndex = lstChats.TopIndex;

            lstChats.BeginUpdate();
            try
            {
                if (string.IsNullOrWhiteSpace(search) || search == "поиск чатов...")
                {
                    UpdateChatsList();
                    return;
                }
                else
                {
                    lstChats.Items.Clear();
                    var filtered = chats.Where(c => c.Name.ToLower().Contains(search)).ToList();
                    foreach (var c in filtered)
                        lstChats.Items.Add(c);
                }
            }
            finally
            {
                if (currentChat != null)
                {
                    for (int i = 0; i < lstChats.Items.Count; i++)
                    {
                        if (((Chat)lstChats.Items[i]).Id == currentChat.Id)
                        {
                            lstChats.SelectedIndex = i;
                            break;
                        }
                    }
                }
                if (topIndex >= 0 && topIndex < lstChats.Items.Count)
                    lstChats.TopIndex = topIndex;
                lstChats.EndUpdate();
            }

            // Индикатор печати
            if (currentChat != null && !string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                if (!isTypingSent)
                {
                    SendTypingStart();
                    isTypingSent = true;
                }
                typingTimer.Stop();
                typingTimer.Start();
            }
            else if (isTypingSent && string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                SendTypingStop();
                typingTimer.Stop();
                isTypingSent = false;
            }
        }

        private void TxtSearchChats_Enter(object sender, EventArgs e)
        {
            if (txtSearchChats.Text == "Поиск чатов...") { txtSearchChats.Text = ""; txtSearchChats.ForeColor = Color.White; }
        }

        private void TxtSearchChats_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchChats.Text)) { txtSearchChats.Text = "Поиск чатов..."; txtSearchChats.ForeColor = Color.Gray; }
        }

        // ========== Отрисовка списка чатов ==========
        private void LstChats_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || !(lstChats.Items[e.Index] is Chat chat)) return;

            // Определяем цвет фона
            Color backColor;
            if ((e.State & DrawItemState.Selected) != 0)
                backColor = Color.FromArgb(58, 58, 74); // #3A3A4A
            else if (e.Index == hoveredChatIndex)
                backColor = Color.FromArgb(53, 53, 69); // #353545
            else
                backColor = Color.FromArgb(37, 37, 48); // #252530

            using (var brush = new SolidBrush(backColor))
                e.Graphics.FillRectangle(brush, e.Bounds);

            // Аватар (круг)
            Rectangle avatarRect = new Rectangle(e.Bounds.X + 12, e.Bounds.Y + 12, 42, 42);
            string avatarText = chat.Name.Length > 0 ? chat.Name[0].ToString().ToUpper() : "?";
            if (chat.Type == ChatType.Private && chat.Participants != null)
            {
                var other = chat.Participants.FirstOrDefault(p => p.Id != currentUser?.Id);
                if (other != null) avatarText = GetInitials(other.FullName);
            }
            UIHelper.DrawAvatar(e.Graphics, avatarRect, avatarText);

            // Статус онлайн для личных чатов (зелёная точка)
            if (chat.Type == ChatType.Private && chat.Participants != null)
            {
                var other = chat.Participants.FirstOrDefault(p => p.Id != currentUser?.Id);
                bool isOnline = other?.IsOnline ?? false;
                Rectangle statusRect = new Rectangle(avatarRect.Right - 12, avatarRect.Bottom - 12, 12, 12);
                using (var brush = new SolidBrush(isOnline ? Color.Lime : Color.Gray))
                    e.Graphics.FillEllipse(brush, statusRect);
                using (var pen = new Pen(Color.White, 2))
                    e.Graphics.DrawEllipse(pen, statusRect);
            }

            int x = e.Bounds.X + 68;
            int yTitle = e.Bounds.Y + 15;
            int yMessage = e.Bounds.Y + 42;

            // Название чата
            using (var font = new Font("Segoe UI", 11, FontStyle.Bold))
            {
                int rightMargin = 60;
                int titleWidth = e.Bounds.Width - x - rightMargin;
                if (titleWidth < 50) titleWidth = 50;
                Rectangle titleRect = new Rectangle(x, yTitle, titleWidth, 30);
                TextRenderer.DrawText(e.Graphics, chat.Name, font, titleRect, Color.White, TextFormatFlags.EndEllipsis);
            }

            // Последнее сообщение
            if (!string.IsNullOrEmpty(chat.LastMessage))
            {
                using (var font = new Font("Segoe UI", 9))
                {
                    Rectangle msgRect = new Rectangle(x, yMessage, e.Bounds.Width - x - 20, 30);
                    TextRenderer.DrawText(e.Graphics, chat.LastMessage, font, msgRect, Color.FromArgb(180, 180, 200), TextFormatFlags.EndEllipsis);
                }
            }

            // Время последнего сообщения
            if (chat.LastMessageTime > DateTime.MinValue)
            {
                string timeStr = chat.LastMessageTime.ToString("HH:mm");
                SizeF timeSize = e.Graphics.MeasureString(timeStr, new Font("Segoe UI", 8));
                float timeX = e.Bounds.Right - timeSize.Width - 20;
                using (var brush = new SolidBrush(Color.FromArgb(180, 180, 200)))
                {
                    e.Graphics.DrawString(timeStr, new Font("Segoe UI", 8), brush, timeX, yTitle);
                }
            }

            // Непрочитанные сообщения
            if (chat.UnreadCount > 0)
            {
                string cnt = chat.UnreadCount.ToString();
                var sz = e.Graphics.MeasureString(cnt, new Font("Segoe UI", 8));
                int badgeSize = Math.Max(18, (int)sz.Width + 8);
                Rectangle rect = new Rectangle(e.Bounds.Right - badgeSize - 15, e.Bounds.Y + 40, badgeSize, 18);
                using (var brush = new SolidBrush(Color.FromArgb(244, 67, 54)))
                    e.Graphics.FillEllipse(brush, rect);
                using (var sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    e.Graphics.DrawString(cnt, new Font("Segoe UI", 8), Brushes.White, rect, sf);
            }

            // Разделитель
            using (Pen pen = new Pen(Color.FromArgb(50, 50, 70)))
                e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);

            e.DrawFocusRectangle();
        }

        // ========== Отрисовка сообщений ==========
        private void LstMessages_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || currentUser == null) return;
            object item = lstMessages.Items[e.Index];
            if (item == null) return;

            if (item is string dateStr)
            {
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (var brush = new SolidBrush(Color.FromArgb(180, 180, 200)))
                    e.Graphics.DrawString(dateStr, dateFont, brush, e.Bounds, sf);
                return;
            }

            if (!(item is Shared.Message msg)) return;

            bool isPrivate = currentChat?.Type == ChatType.Private;
            bool isMy = msg.SenderId == currentUser.Id;
            bool needAvatar = !isPrivate && !isMy && IsLastInSeries(e.Index);
            bool needSenderName = !isPrivate && !isMy && IsFirstInSeries(e.Index);

            int avatarAreaWidth = (!isPrivate && !isMy) ? (avatarSize + 10) : 0; // место под аватар только для чужих в групповых

            // Позиция блока сообщения
            int x;
            if (isMy)
            {
                // Свои: прижаты к правому краю, без отступа под аватар
                x = e.Bounds.Right - messageMaxWidth - messageMargin;
            }
            else
            {
                // Чужие: отступ слева с учетом места под аватар
                x = e.Bounds.Left + messageMargin + avatarAreaWidth;
            }

            int topOffset = 2;
            Rectangle bubbleRect = new Rectangle(x, e.Bounds.Y + topOffset, messageMaxWidth, e.Bounds.Height - 4);

            Color bgColor = isMy ? Color.FromArgb(60, 80, 120) : Color.FromArgb(50, 50, 65);
            using (var path = GetRoundedRect(bubbleRect, 10))
            using (var brush = new SolidBrush(bgColor))
            using (var pen = new Pen(Color.White, 1))
            {
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }

            // Аватар для чужих (слева)
            if (needAvatar)
            {
                Rectangle avatarRect = new Rectangle(bubbleRect.Left - avatarSize - 10, e.Bounds.Y + 5, avatarSize, avatarSize);
                string initials = GetInitials(msg.SenderName);
                UIHelper.DrawAvatar(e.Graphics, avatarRect, initials);
            }

            int currentY = e.Bounds.Y + (IsFirstInSeries(e.Index) ? 5 : 2);

            // Имя отправителя (только для чужих, первых в серии)
            if (needSenderName)
            {
                string senderDisplay = string.IsNullOrEmpty(msg.SenderDepartment)
                    ? msg.SenderName
                    : $"{msg.SenderName} ({msg.SenderDepartment})";
                using (var brush = new SolidBrush(Color.White))
                    e.Graphics.DrawString(senderDisplay, messageSenderFont, brush,
                        new RectangleF(x + messagePadding, currentY, messageMaxWidth - 2 * messagePadding, 100));
                SizeF senderSize = e.Graphics.MeasureString(senderDisplay, messageSenderFont, messageMaxWidth - 2 * messagePadding);
                currentY += (int)senderSize.Height + 5;
            }
            else
            {
                currentY += 2;
            }

            // Текст сообщения
            using (var brush = new SolidBrush(Color.White))
                e.Graphics.DrawString(msg.Text, messageTextFont, brush,
                    new RectangleF(x + messagePadding, currentY, messageMaxWidth - 2 * messagePadding, 1000));

            // Время и отметка о редактировании
            string timeStr = msg.SentAt.ToString("HH:mm");
            if (msg.EditedAt.HasValue) timeStr += " (ред.)";
            using (var brush = new SolidBrush(Color.White))
            {
                SizeF timeSize = e.Graphics.MeasureString(timeStr, messageTimeFont);
                float timeX = bubbleRect.Right - messagePadding - timeSize.Width;
                float timeY = bubbleRect.Bottom - timeSize.Height - 5;
                e.Graphics.DrawString(timeStr, messageTimeFont, brush, timeX, timeY);
            }
        }

        private GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void SendReadReceipt(int chatId, int lastReadId)
        {
            var data = new Dictionary<string, int> { { "chatId", chatId }, { "lastReadMessageId", lastReadId } };
            networkClient.SendPacket(new NetworkPacket { Command = Shared.CommandType.MessagesRead, UserId = currentUser.Id, Data = data });
        }

        private void UpdateCurrentChatHeader()
        {
            if (currentChat == null) return;

            if (currentChat.Type == ChatType.Private)
            {
                var other = currentChat.Participants?.FirstOrDefault(p => p.Id != currentUser.Id);
                if (other != null)
                {
                    string displayName = other.FullName;
                    if (!string.IsNullOrWhiteSpace(other.Position))
                        displayName += $" ({other.Position})";
                    lblChatName.Text = displayName;

                    // Обновление статуса
                    if (other.IsOnline)
                    {
                        lblLastSeen.Text = "онлайн";
                        lblLastSeen.ForeColor = Color.LightGreen;
                    }
                    else if (other.LastSeen.HasValue)
                    {
                        var diff = DateTime.Now - other.LastSeen.Value;
                        if (diff.TotalMinutes < 1)
                            lblLastSeen.Text = "только что";
                        else if (diff.TotalHours < 1)
                            lblLastSeen.Text = $"был(а) {diff.Minutes} мин. назад";
                        else if (diff.TotalDays < 1)
                            lblLastSeen.Text = $"был(а) сегодня в {other.LastSeen.Value:HH:mm}";
                        else if (diff.TotalDays < 2)
                            lblLastSeen.Text = $"был(а) вчера в {other.LastSeen.Value:HH:mm}";
                        else
                            lblLastSeen.Text = $"был(а) {other.LastSeen.Value:dd.MM.yyyy}";
                        lblLastSeen.ForeColor = Color.White;
                    }
                    else
                        lblLastSeen.Text = "";
                }
            }
            else
            {
                lblChatName.Text = currentChat.Name;
                lblLastSeen.Text = "";
            }

            btnViewParticipants.Visible = (currentChat.Type != ChatType.Private);
        }

        private void BtnManageParticipants_Click(object sender, EventArgs e)
        {
            if (currentChat?.Type == ChatType.Department || currentChat?.Type == ChatType.Group)
            {
                using (var form = new ManageParticipantsForm(currentChat.Id, currentUser.Id, networkClient, currentUser.IsAdmin))
                {
                    form.ShowDialog();
                }
            }
        }

        private void LstMessages_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && selectedMessageIndices.Count > 0)
            {
                // Получаем выделенные сообщения
                var selectedMessages = selectedMessageIndices
                    .Select(i => lstMessages.Items[i] as Shared.Message)
                    .Where(m => m != null)
                    .ToList();

                if (selectedMessages.Count == 0) return;

                // Проверка прав
                if (!selectedMessages.All(m => m.SenderId == currentUser.Id || currentUser.IsAdmin))
                {
                    MessageBox.Show("Вы можете удалять только свои сообщения.", "Доступ запрещён",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string confirmMsg = selectedMessages.Count == 1
                    ? "Удалить выбранное сообщение?"
                    : $"Удалить {selectedMessages.Count} сообщений?";
                if (MessageBox.Show(confirmMsg, "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                foreach (var msg in selectedMessages)
                {
                    networkClient.SendPacket(new NetworkPacket
                    {
                        Command = Shared.CommandType.DeleteMessage,
                        Data = msg.Id
                    });
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                selectedMessageIndices.Clear();
                lastSelectedIndex = -1;
                lstMessages.Invalidate();
                e.Handled = true;
            }
        }

        private void StartEditingMessage(Shared.Message msg)
        {
            if (msg.SenderId != currentUser.Id && !currentUser.IsAdmin) return;

            editingMessage = msg;
            originalMessageText = msg.Text;
            txtMessage.Text = msg.Text;
            txtMessage.Focus();
            txtMessage.Select(txtMessage.Text.Length, 0);

            // Изменяем внешний вид кнопки
            btnSend.Text = "✎ Сохранить";
            btnSend.BackColor = Color.FromArgb(76, 175, 80);
            btnSend.ForeColor = Color.White;

            // Показываем подсказку (если есть lblEditingHint)
            if (lblEditingHint != null)
            {
                lblEditingHint.Visible = true;
                lblEditingHint.Text = "Редактирование сообщения (Esc – отмена)";
            }
        }

        private void CancelEditing()
        {
            editingMessage = null;
            txtMessage.Text = "";
            btnSend.Text = "Отправить";
            btnSend.BackColor = Color.FromArgb(0, 229, 255);
            btnSend.ForeColor = Color.Black;
            if (lblEditingHint != null) lblEditingHint.Visible = false;

            txtMessage.Height = 40;
            panelMessageInput.Height = 70;
            btnSend.Top = (70 - btnSend.Height) / 2;
        }

        private void SendOrEdit()
        {
            if (editingMessage != null)
            {
                string newText = txtMessage.Text.Trim();
                if (string.IsNullOrEmpty(newText)) return;

                if (newText == originalMessageText)
                {
                    CancelEditing();
                    return;
                }

                var data = new Dictionary<string, object>
        {
            { "messageId", editingMessage.Id },
            { "newText", newText }
        };
                networkClient.SendPacket(new NetworkPacket
                {
                    Command = Shared.CommandType.EditMessage,
                    Data = data
                });
                CancelEditing(); // интерфейс сбросим, обновление придёт через MessageEdited
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtMessage.Text) || currentChat == null) return;
                var msg = new Shared.Message
                {
                    ChatId = currentChat.Id,
                    SenderId = currentUser.Id,
                    SenderName = currentUser.FullName,
                    Text = txtMessage.Text.Trim(),
                    SentAt = DateTime.Now
                };
                networkClient.SendPacket(new NetworkPacket
                {
                    Command = Shared.CommandType.SendMessage,
                    UserId = currentUser.Id,
                    Data = msg
                });
                txtMessage.Clear();

                txtMessage.Height = 40;
                panelMessageInput.Height = 70;
                btnSend.Top = (70 - btnSend.Height) / 2;
            }
        }

        private void HandleMessageDeleted(int messageId)
        {
            foreach (var kv in messages)
            {
                var msgList = kv.Value;
                var msg = msgList.FirstOrDefault(m => m.Id == messageId);
                if (msg != null)
                {
                    msgList.Remove(msg);
                    // Если удаление произошло в текущем открытом чате – обновляем интерфейс
                    if (kv.Key == currentChat?.Id)
                    {
                        DisplayMessages();
                    }
                    break;
                }
            }
        }

        private void HandleMessageEdited(Shared.Message editedMsg)
        {
            if (messages.ContainsKey(editedMsg.ChatId))
            {
                var list = messages[editedMsg.ChatId];
                var oldMsg = list.FirstOrDefault(m => m.Id == editedMsg.Id);
                if (oldMsg != null)
                {
                    oldMsg.Text = editedMsg.Text;
                    oldMsg.EditedAt = editedMsg.EditedAt;
                    if (currentChat?.Id == editedMsg.ChatId)
                    {
                        DisplayMessages(); // перерисовываем список
                    }
                }
            }
        }

        private void ApplySearchFilter()
        {
            string search = txtSearchChats.Text.ToLower().Trim();
            if (string.IsNullOrWhiteSpace(search) || search == "поиск чатов...")
            {
                // Если поиск пуст, показываем все чаты (уже отображены)
                return;
            }

            // Фильтруем список чатов по имени
            var filtered = chats.Where(c => c.Name.ToLower().Contains(search)).ToList();

            lstChats.BeginUpdate();
            try
            {
                lstChats.Items.Clear();
                foreach (var chat in filtered)
                    lstChats.Items.Add(chat);
            }
            finally
            {
                lstChats.EndUpdate();
            }
        }

        private void LstMessages_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0 || lstMessages.Items[e.Index] == null) return;

            if (lstMessages.Items[e.Index] is string)
            {
                e.ItemHeight = 30;
                return;
            }

            if (lstMessages.Items[e.Index] is Shared.Message msg)
            {
                bool isPrivate = currentChat?.Type == ChatType.Private;
                bool isMy = msg.SenderId == currentUser.Id;
                bool needSenderName = !isPrivate && !isMy && IsFirstInSeries(e.Index);

                int textWidth = messageMaxWidth - 2 * messagePadding; // ширина текста внутри блока (одинаковая для всех)

                int y = 2;
                if (needSenderName)
                {
                    string senderDisplay = string.IsNullOrEmpty(msg.SenderDepartment)
                        ? msg.SenderName
                        : $"{msg.SenderName} ({msg.SenderDepartment})";
                    SizeF senderSize = e.Graphics.MeasureString(senderDisplay, messageSenderFont, textWidth);
                    y += (int)senderSize.Height + 5;
                }
                else
                {
                    y += 2;
                }

                SizeF textSize = e.Graphics.MeasureString(msg.Text, messageTextFont, textWidth);
                y += (int)textSize.Height + 5;

                string timeStr = msg.SentAt.ToString("HH:mm");
                if (msg.EditedAt.HasValue) timeStr += " (ред.)";
                SizeF timeSize = e.Graphics.MeasureString(timeStr, messageTimeFont);
                y += (int)timeSize.Height + 5;

                int minHeight = (!isPrivate && !isMy && IsLastInSeries(e.Index)) ? (avatarSize + 10) : 0;
                e.ItemHeight = Math.Max(y, minHeight);
            }
        }

        private void DeleteSelectedMessages()
        {
            var selectedMessages = selectedMessageIndices
                .Select(i => lstMessages.Items[i] as Shared.Message)
                .Where(m => m != null)
                .ToList();
            if (selectedMessages.Count == 0) return;
            if (!selectedMessages.All(m => m.SenderId == currentUser.Id || currentUser.IsAdmin))
            {
                MessageBox.Show("Вы можете удалять только свои сообщения.");
                return;
            }
            if (MessageBox.Show($"Удалить {selectedMessages.Count} сообщений?", "Подтверждение",
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            foreach (var msg in selectedMessages)
                networkClient.SendPacket(new NetworkPacket { Command = Shared.CommandType.DeleteMessage, Data = msg.Id });
        }

        private void TxtMessage_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                txtMessage.Height = 40;
                panelMessageInput.Height = 70;
                btnSend.Top = (70 - btnSend.Height) / 2;
                return;
            }

            // Вычисляем требуемую высоту поля с учётом переноса текста
            int width = txtMessage.ClientSize.Width;
            if (width <= 0) return;

            Size proposedSize = new Size(width, int.MaxValue);
            TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;
            Size textSize = TextRenderer.MeasureText(txtMessage.Text, txtMessage.Font, proposedSize, flags);

            int newHeight = textSize.Height + 8; // небольшой отступ
            const int minHeight = 40;
            const int maxHeight = 150;

            if (newHeight < minHeight) newHeight = minHeight;
            if (newHeight > maxHeight) newHeight = maxHeight;

            if (txtMessage.Height != newHeight)
            {
                // Изменяем высоту поля
                txtMessage.Height = newHeight;

                // Изменяем высоту панели (учитываем отступы: 10 сверху + 10 снизу)
                int panelNewHeight = newHeight + 20;
                panelMessageInput.Height = panelNewHeight;

                // Перемещаем кнопку по вертикали в центр панели
                btnSend.Top = (panelNewHeight - btnSend.Height) / 2;
            }

            // Индикатор печати (добавить в конец метода)
            if (currentChat != null && !string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                if (!isTypingSent)
                {
                    SendTypingStart();
                    isTypingSent = true;
                }
                typingTimer.Stop();
                typingTimer.Start();
            }
            else if (isTypingSent && string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                SendTypingStop();
                typingTimer.Stop();
                isTypingSent = false;
            }
        }

        private void LstMessages_MouseDown(object sender, MouseEventArgs e)
        {
            lstMessages.Focus();

            int index = lstMessages.IndexFromPoint(e.Location);
            // Безопасная проверка
            bool isValidIndex = (index >= 0 && index < lstMessages.Items.Count);
            bool isMessage = isValidIndex && (lstMessages.Items[index] is Shared.Message);

            // Левая кнопка: логика выделения
            if (e.Button == MouseButtons.Left)
            {
                if (!isMessage)
                {
                    selectedMessageIndices.Clear();
                    lastSelectedIndex = -1;
                    lstMessages.Invalidate();
                    return;
                }

                // Далее работаем с валидным индексом
                if (ModifierKeys.HasFlag(Keys.Control))
                {
                    if (selectedMessageIndices.Contains(index))
                        selectedMessageIndices.Remove(index);
                    else
                        selectedMessageIndices.Add(index);
                    lastSelectedIndex = index;
                }
                else if (ModifierKeys.HasFlag(Keys.Shift) && lastSelectedIndex != -1)
                {
                    int start = Math.Min(lastSelectedIndex, index);
                    int end = Math.Max(lastSelectedIndex, index);
                    selectedMessageIndices.Clear();
                    for (int i = start; i <= end; i++)
                        if (lstMessages.Items[i] is Shared.Message)
                            selectedMessageIndices.Add(i);
                    lastSelectedIndex = index;
                }
                else
                {
                    selectedMessageIndices.Clear();
                    selectedMessageIndices.Add(index);
                    lastSelectedIndex = index;
                }
                lstMessages.Invalidate();
            }
            // Правая кнопка
            else if (e.Button == MouseButtons.Right)
            {
                if (isMessage)
                {
                    if (!selectedMessageIndices.Contains(index))
                    {
                        selectedMessageIndices.Clear();
                        selectedMessageIndices.Add(index);
                        lastSelectedIndex = index;
                        lstMessages.Invalidate();
                    }
                    ShowContextMenu(e.Location);
                }
            }
        }

        private void ShowContextMenu(Point location)
        {
            var selectedMsgs = selectedMessageIndices
            .Where(i => i >= 0 && i < lstMessages.Items.Count && lstMessages.Items[i] is Shared.Message)
            .Select(i => lstMessages.Items[i] as Shared.Message)
            .Where(m => m != null)
            .ToList();

            if (selectedMsgs.Count == 0) return;

            var cms = new ContextMenuStrip();

            // Копировать
            cms.Items.Add("📋 Копировать текст", null, (s, e) =>
            {
                var text = string.Join(Environment.NewLine, selectedMsgs.Select(m => m.Text));
                Clipboard.SetText(text);
            });

            // Редактировать (только одно сообщение и есть права)
            if (selectedMsgs.Count == 1)
            {
                var msg = selectedMsgs[0];
                if (msg.SenderId == currentUser.Id || currentUser.IsAdmin)
                {
                    cms.Items.Add("✏ Редактировать", null, (s, e) => StartEditingMessage(msg));
                }
            }

            // Удалить (если все выделенные – свои или админ)
            bool canDelete = selectedMsgs.All(m => m.SenderId == currentUser.Id || currentUser.IsAdmin);
            if (canDelete)
            {
                string deleteText = selectedMsgs.Count == 1 ? "🗑 Удалить" : $"🗑 Удалить ({selectedMsgs.Count})";
                cms.Items.Add(deleteText, null, (s, e) => DeleteSelectedMessages());
            }

            cms.Show(lstMessages, location);
        }

        private void BtnViewParticipants_Click(object sender, EventArgs e)
        {
            if (currentChat == null) return;
            using (var form = new ViewParticipantsForm(currentChat.Id, currentUser.Id, networkClient))
            {
                form.ShowDialog();
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && editingMessage != null)
            {
                CancelEditing();
                e.Handled = true;
            }
        }

        // Отрисовка градиентного фона с прозрачностью
        private void PanelTop_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = panelTop.ClientRectangle;
            using (LinearGradientBrush brush = new LinearGradientBrush(rect,
                Color.FromArgb(200, 20, 20, 30),   // полупрозрачный тёмный
                Color.FromArgb(200, 45, 45, 58),
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, rect);
            }
        }

        // Отрисовка аватара (круг, белая обводка, тень)
        private void PicUserAvatar_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            if (pb == null || pb.Image == null) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, pb.Width, pb.Height);
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(rect);
            pb.Region = new Region(path);

            // Тень (сдвинутая копия)
            using (var shadowBrush = new SolidBrush(Color.FromArgb(80, 0, 0, 0)))
            {
                e.Graphics.TranslateTransform(2, 2);
                e.Graphics.FillPath(shadowBrush, path);
                e.Graphics.TranslateTransform(-2, -2);
            }
            // Белая обводка
            using (Pen pen = new Pen(Color.White, 2))
            {
                e.Graphics.DrawEllipse(pen, rect);
            }
        }

        // Обновление цвета онлайн-индикатора и белая обводка
        private void UpdateOnlineIndicator()
        {
            if (currentUser != null)
            {
                picOnlineIndicator.BackColor = currentUser.IsOnline ? Color.Lime : Color.Red;
                // Добавляем белую обводку (перерисовываем)
                using (Graphics g = picOnlineIndicator.CreateGraphics())
                using (Pen pen = new Pen(Color.White, 1))
                {
                    g.DrawEllipse(pen, 0, 0, picOnlineIndicator.Width - 1, picOnlineIndicator.Height - 1);
                }
            }
        }

        // Динамическое позиционирование кнопок (привязаны к правому краю)
        private void UpdateButtonsPosition()
        {
            if (panelTop.Width == 0) return;
            int margin = 20;
            int rightEdge = panelTop.ClientSize.Width - margin;

            // Кнопка-шестерёнка – самая правая
            btnSettings.Location = new Point(rightEdge - btnSettings.Width, (panelTop.Height - btnSettings.Height) / 2);
            // Кнопка "Новый чат" – слева от шестерёнки
            btnNewChat.Location = new Point(btnSettings.Left - btnNewChat.Width - margin, btnSettings.Top);
        }

        private void UpdateFooterStats()
        {
            List<User> allUsers = chats.SelectMany(c => c.Participants)
                        .GroupBy(u => u.Id)
                        .Select(g => g.First())
                        .ToList();
            int total = allUsers.Count;
            int online = allUsers.Count(u => u.IsOnline);

            if (lblOnlineCount != null)
                lblOnlineCount.Text = $"🟢 Онлайн: {online}";
            if (lblTotalUsers != null)
                lblTotalUsers.Text = $"👥 Всего: {total}";
        }

        /// <summary>
        /// Определяет, является ли сообщение первым в серии от одного отправителя
        /// (разделитель даты или смена отправителя прерывают серию)
        /// </summary>
        private bool IsFirstInSeries(int currentIndex)
        {
            if (currentIndex < 0) return true;
            if (!(lstMessages.Items[currentIndex] is Shared.Message currentMsg)) return true;

            for (int i = currentIndex - 1; i >= 0; i--)
            {
                if (lstMessages.Items[i] is string)
                    return true; // разделитель даты прерывает серию
                if (lstMessages.Items[i] is Shared.Message prevMsg)
                    return prevMsg.SenderId != currentMsg.SenderId;
            }
            return true;
        }

        /// <summary>
        /// Определяет, является ли сообщение последним в серии от одного отправителя
        /// (разделитель даты или смена отправителя прерывают серию)
        /// </summary>
        private bool IsLastInSeries(int currentIndex)
        {
            if (currentIndex < 0) return true;
            if (!(lstMessages.Items[currentIndex] is Shared.Message currentMsg)) return true;

            for (int i = currentIndex + 1; i < lstMessages.Items.Count; i++)
            {
                if (lstMessages.Items[i] is string)
                    return true; // разделитель даты прерывает серию
                if (lstMessages.Items[i] is Shared.Message nextMsg)
                    return nextMsg.SenderId != currentMsg.SenderId;
            }
            return true;
        }

        private void HideCurrentChat(object sender, EventArgs e)
        {
            if (currentChat != null)
            {
                ShowChat(false);
                // Сбрасываем сохранённый ID последнего чата
                ini.Write(INI_SECTION, KEY_LAST_CHAT, "0");
            }
        }

        private void SaveLastChat()
        {
            if (currentChat != null && ini != null)
                ini.Write(INI_SECTION, KEY_LAST_CHAT, currentChat.Id.ToString());
        }

        private int LoadLastChatId()
        {
            if (ini == null) return 0;
            string val = ini.Read(INI_SECTION, KEY_LAST_CHAT, "0");
            return int.TryParse(val, out int id) ? id : 0;
        }

        private void LeaveChatMenuItem_Click(object sender, EventArgs e)
        {
            // Получаем выделенный чат в списке (не тот, который открыт, а на который кликнули правой кнопкой)
            if (lstChats.SelectedItem is Chat chat)
            {
                // Запрещаем выход из чата отдела для не-администраторов
                if (chat.Type == ChatType.Department && !currentUser.IsAdmin)
                {
                    MessageBox.Show("Вы не можете покинуть чат отдела. Обратитесь к администратору.",
                        "Действие запрещено", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string confirmMessage = chat.Type == ChatType.Private
                    ? "Вы уверены, что хотите удалить этот диалог? Он исчезнет из списка, но останется у собеседника."
                    : "Вы уверены, что хотите покинуть чат? Вы больше не сможете писать в него, если вас не добавят снова.";

                if (MessageBox.Show(confirmMessage, "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Отправляем команду на удаление себя из участников чата
                    networkClient.SendPacket(new NetworkPacket
                    {
                        Command = Shared.CommandType.RemoveChatParticipant,
                        Data = new { chatId = chat.Id, userId = currentUser.Id }
                    });
                }
            }
            else
            {
                MessageBox.Show("Выберите чат в списке.");
            }
        }

        private void ShowChat(bool showChat)
        {
            if (showChat && currentChat != null)
            {
                panelChatHeader.Visible = true;
                lstMessages.Visible = true;
                panelMessageInput.Visible = true;
                lblWelcomePlaceholder.Visible = false;
            }
            else
            {
                panelChatHeader.Visible = false;
                lstMessages.Visible = false;
                panelMessageInput.Visible = false;
                lblWelcomePlaceholder.Visible = true;
                currentChat = null;
                lblChatName.Text = "Выберите чат";
                lstMessages.Items.Clear();
                btnSend.Enabled = false;
            }
            panelRight.PerformLayout();
        }

        private void LstChats_MouseMoveForToolTip(object sender, MouseEventArgs e)
        {
            int index = lstChats.IndexFromPoint(e.Location);
            if (index >= 0 && index < lstChats.Items.Count && lstChats.Items[index] is Chat chat)
            {
                Rectangle itemBounds = lstChats.GetItemRectangle(index);
                int x = itemBounds.X + 68;
                int yTitle = itemBounds.Y + 15;
                int maxWidth = itemBounds.Width - x - 20;

                using (var g = lstChats.CreateGraphics())
                using (var font = new Font("Segoe UI", 11, FontStyle.Bold))
                {
                    SizeF textSize = g.MeasureString(chat.Name, font);
                    int textWidth = (int)Math.Ceiling(textSize.Width);
                    if (textWidth > maxWidth) textWidth = maxWidth;
                    Rectangle textRect = new Rectangle(x, yTitle, textWidth, 30);
                    if (textRect.Contains(e.Location))
                    {
                        chatToolTip.SetToolTip(lstChats, chat.Name);
                        return;
                    }
                }
            }
            chatToolTip.SetToolTip(lstChats, null);
        }
    }
}
