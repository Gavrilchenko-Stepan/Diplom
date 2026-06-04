using Messenger.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Messenger.Client
{
    public partial class MainForm : Form
    {
        private User currentUser;
        private NetworkClient networkClient;
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

        public MainForm()
        {
            InitializeComponent();

            this.Shown += (s, e) => UpdateButtonsPosition();

            picOnlineIndicator.BringToFront();

            // Скругление углов верхней панели
            SetRoundedRegion(panelTop, 20);

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
                if (MessageBox.Show("Выйти из приложения?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            refreshTimer?.Stop();
            networkClient?.Disconnect();
        }

        private void ShowLoginForm()
        {
            using (var loginForm = new LoginForm())
            {
                var result = loginForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    currentUser = loginForm.CurrentUser;
                    networkClient = loginForm.NetworkClient;
                    networkClient.OnPacketReceived += OnPacketReceived;
                    networkClient.OnDisconnected += OnDisconnected;

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
                g.Clear(Color.FromArgb(63, 81, 181));
                using (Font font = new Font("Segoe UI", 18, FontStyle.Bold))
                using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(initials, font, Brushes.White, new Rectangle(0, 0, 48, 48), sf);
                }
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
                            lblChatInfo.Text = "";
                            lstMessages.Items.Clear();
                            btnSend.Enabled = false;
                        }
                        break;

                    case Shared.CommandType.MessagesList:
                        var jsonElemMsgs = (JsonElement)packet.Data;
                        string jsonMsgs = jsonElemMsgs.GetRawText();
                        var msgs = JsonSerializer.Deserialize<List<Shared.Message>>(jsonMsgs);
                        if (msgs.Any())
                        {
                            int chatId = msgs.First().ChatId;
                            Console.WriteLine($"Получен список сообщений для чата {chatId}, всего {msgs.Count}");
                            messages[chatId] = msgs;
                            if (currentChat != null && chatId == currentChat.Id)
                                DisplayMessages();
                        }
                        else
                        {
                            Console.WriteLine("Получен пустой список сообщений");
                        }
                        break;

                    case Shared.CommandType.NewMessage:
                        var jsonElemNewMsg = (JsonElement)packet.Data;
                        string jsonNewMsg = jsonElemNewMsg.GetRawText();
                        var newMsg = JsonSerializer.Deserialize<Shared.Message>(jsonNewMsg);
                        Console.WriteLine($"Получено новое сообщение: '{newMsg.Text}' в чат {newMsg.ChatId}");
                        HandleNewMessage(newMsg);
                        break;

                    case Shared.CommandType.UserStatusChanged:
                        var jsonElemUser = (JsonElement)packet.Data;
                        string jsonUser = jsonElemUser.GetRawText();
                        var user = JsonSerializer.Deserialize<User>(jsonUser);
                        Console.WriteLine($"UserStatusChanged: {user.FullName} is {user.IsOnline}");

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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки: {ex.Message}");
            }
        }

        private void OnDisconnected()
        {
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
                    lblChatInfo.Text = "";
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
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.Clear(Color.FromArgb(63, 81, 181));

                string text = "?";
                if (currentChat.Type == ChatType.Private && currentChat.Participants != null)
                {
                    var other = currentChat.Participants.FirstOrDefault(p => p.Id != currentUser.Id);
                    if (other != null && !string.IsNullOrWhiteSpace(other.FullName))
                    {
                        var parts = other.FullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 1)
                            text = parts[0][0].ToString().ToUpper();
                        else if (parts.Length >= 2)
                            text = (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
                    }
                }
                else if (!string.IsNullOrWhiteSpace(currentChat.Name))
                {
                    text = currentChat.Name[0].ToString().ToUpper();
                }

                using (Font font = new Font("Segoe UI", 20, FontStyle.Bold))
                {
                    SizeF textSize = g.MeasureString(text, font);
                    float x = (size - textSize.Width) / 2;
                    float y = (size - textSize.Height) / 2;
                    g.DrawString(text, font, Brushes.White, x, y);
                }
            }
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
            Console.WriteLine($"UpdateUserStatus: получили статус для {user.FullName} (Id={user.Id}) - онлайн={user.IsOnline}");
            bool updated = false;
            foreach (var chat in chats)
            {
                var p = chat.Participants?.FirstOrDefault(x => x.Id == user.Id);
                if (p != null)
                {
                    Console.WriteLine($"  найден в чате {chat.Name}, старый статус {p.IsOnline}, новый {user.IsOnline}");
                    p.IsOnline = user.IsOnline;
                    p.LastSeen = user.LastSeen;
                    updated = true;
                }
            }
            if (updated)
            {
                Console.WriteLine("  Обновляем список чатов...");
                UpdateChatsList();
            }
            else
            {
                Console.WriteLine("  Участник не найден ни в одном чате");
            }

            if (currentChat?.Type == ChatType.Private)
            {
                var other = currentChat.Participants?.FirstOrDefault(p => p.Id == user.Id);
                if (other != null)
                {
                    string status = other.IsOnline ? "● Онлайн" : "● Офлайн";
                    lblChatInfo.Text = $"Личный чат • {status}";
                }
            }
            else if (currentChat?.Type == ChatType.Group || currentChat?.Type == ChatType.Department)
            {
                int onlineCount = currentChat.Participants?.Count(p => p.IsOnline) ?? 0;
                if (currentChat.Type == ChatType.Group)
                    lblChatInfo.Text = $"Групповой чат • {currentChat.Participants.Count} уч. • {onlineCount} онлайн";
                else
                    lblChatInfo.Text = $"Чат • {currentChat.Participants.Count} уч. • {onlineCount} онлайн";
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
            using (var form = new AdminDashboardForm(networkClient, currentUser.Id))
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
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(avatarRect);
                using (var brush = new SolidBrush(Color.FromArgb(63, 81, 181)))
                    e.Graphics.FillPath(brush, path);
            }

            // Текст аватара: первая буква названия чата
            string avatarText = chat.Name.Length > 0 ? chat.Name[0].ToString().ToUpper() : "?";
            if (chat.Type == ChatType.Private && chat.Participants != null)
            {
                var other = chat.Participants.FirstOrDefault(p => p.Id != currentUser?.Id);
                if (other != null) avatarText = GetInitials(other.FullName);
            }
            using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
            using (var sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                e.Graphics.DrawString(avatarText, font, Brushes.White, avatarRect, sf);

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
                e.Graphics.DrawString(chat.Name, font, Brushes.White, x, yTitle);

            // Последнее сообщение
            if (!string.IsNullOrEmpty(chat.LastMessage))
            {
                string last = chat.LastMessage.Length > 35 ? chat.LastMessage.Substring(0, 32) + "..." : chat.LastMessage;
                using (var font = new Font("Segoe UI", 9))
                    e.Graphics.DrawString(last, font, Brushes.Gray, x, yMessage);
            }

            // Время последнего сообщения
            if (chat.LastMessageTime > DateTime.MinValue)
            {
                string timeStr = chat.LastMessageTime.ToString("HH:mm");
                SizeF timeSize = e.Graphics.MeasureString(timeStr, new Font("Segoe UI", 8));
                float timeX = e.Bounds.Right - timeSize.Width - 20;
                e.Graphics.DrawString(timeStr, new Font("Segoe UI", 8), Brushes.Gray, timeX, yTitle);
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
            if (e.Index < 0) return;
            if (currentUser == null) return;

            object item = lstMessages.Items[e.Index];
            if (item == null) return;

            // Разделитель даты (строка)
            if (item is string dateStr)
            {
                // Рисуем дату на нейтральном фоне
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (var brush = new SolidBrush(Color.FromArgb(180, 180, 200)))
                {
                    e.Graphics.DrawString(dateStr, dateFont, brush, e.Bounds, sf);
                }
                return;
            }

            if (!(item is Shared.Message msg)) return;

            bool isMy = msg.SenderId == currentUser.Id;
            bool isSelected = selectedMessageIndices.Contains(e.Index);

            const int maxWidth = 400;
            const int padding = 10;
            int x = isMy ? e.Bounds.Right - maxWidth - 20 : e.Bounds.Left + 20;
            int textWidth = maxWidth - 2 * padding;

            Rectangle bubbleRect = new Rectangle(x, e.Bounds.Y + 2, maxWidth, e.Bounds.Height - 4);

            // Цвета пузыря в зависимости от выделения
            Color bgColor;
            if (isMy)
            {
                bgColor = isSelected ? Color.FromArgb(0, 120, 215) : Color.FromArgb(0, 229, 255, 80);
            }
            else
            {
                bgColor = isSelected ? Color.FromArgb(0, 100, 200) : Color.FromArgb(60, 60, 80);
            }
            Color borderColor = isSelected ? Color.White : (isMy ? Color.FromArgb(0, 229, 255) : Color.Gray);

            using (var path = GetRoundedRect(bubbleRect, 10))
            using (var brush = new SolidBrush(bgColor))
            using (var pen = new Pen(borderColor, isSelected ? 2 : 1))
            {
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }

            int currentY = e.Bounds.Y + 5;

            // Имя отправителя
            string senderDisplay = string.IsNullOrEmpty(msg.SenderDepartment)
                ? msg.SenderName
                : $"{msg.SenderName} ({msg.SenderDepartment})";
            using (var brush = new SolidBrush(Color.White))
            {
                e.Graphics.DrawString(senderDisplay, messageSenderFont,
                    brush, new RectangleF(x + padding, currentY, textWidth, 100));
            }
            SizeF senderSize = e.Graphics.MeasureString(senderDisplay, messageSenderFont, textWidth);
            currentY += (int)senderSize.Height + 5;

            // Текст сообщения
            using (var brush = new SolidBrush(Color.White))
            {
                e.Graphics.DrawString(msg.Text, messageTextFont,
                    brush, new RectangleF(x + padding, currentY, textWidth, 1000));
            }
            SizeF textSize = e.Graphics.MeasureString(msg.Text, messageTextFont, textWidth);
            currentY += (int)textSize.Height + 5;

            // Время
            string timeStr = msg.SentAt.ToString("HH:mm");
            if (msg.EditedAt.HasValue)
                timeStr += " (ред.)";
            using (var brush = new SolidBrush(Color.Gray))
            {
                SizeF timeSize = e.Graphics.MeasureString(timeStr, messageTimeFont);
                float timeX = bubbleRect.Right - padding - timeSize.Width;
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

            string chatName = currentChat.Name;
            string chatDepartment = "";

            if (currentChat.Type == ChatType.Private)
            {
                var other = currentChat.Participants?.FirstOrDefault(p => p.Id != currentUser.Id);
                if (other != null)
                {
                    chatName = string.IsNullOrEmpty(other.Position) ? other.FullName : $"{other.FullName} ({other.Position})";
                    chatDepartment = other.Department ?? "";
                    string status = other.IsOnline ? "● Онлайн" : "● Офлайн";
                    lblChatInfo.Text = $"Личный чат • {status}";
                }
                else
                {
                    lblChatInfo.Text = "Личный чат";
                }
            }
            else if (currentChat.Type == ChatType.Group)
            {
                int onlineCount = currentChat.Participants?.Count(p => p.IsOnline) ?? 0;
                int totalParticipants = currentChat.Participants?.Count ?? 0;
                lblChatInfo.Text = $"Групповой чат • {totalParticipants} уч. • {onlineCount} онлайн";
            }
            else // Department
            {
                int onlineCount = currentChat.Participants?.Count(p => p.IsOnline) ?? 0;
                int totalParticipants = currentChat.Participants?.Count ?? 0;
                lblChatInfo.Text = $"Чат отдела • {totalParticipants} уч. • {onlineCount} онлайн";
            }

            btnViewParticipants.Visible = (currentChat.Type != ChatType.Private);

            btnManageParticipants.Visible = (currentUser != null && currentUser.IsAdmin &&
                (currentChat.Type == ChatType.Department || currentChat.Type == ChatType.Group));

            lblChatName.Text = chatName;
            lblChatDepartment.Text = chatDepartment;
        }

        private void BtnManageParticipants_Click(object sender, EventArgs e)
        {
            if (currentChat?.Type == ChatType.Department || currentChat?.Type == ChatType.Group)
            {
                using (var form = new ManageParticipantsForm(currentChat.Id, currentUser.Id, networkClient))
                {
                    form.ShowDialog();
                }
            }
        }

        private void LstMessages_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                selectedMessageIndices.Clear();
                for (int i = 0; i < lstMessages.Items.Count; i++)
                    if (lstMessages.Items[i] is Shared.Message)
                        selectedMessageIndices.Add(i);
                lstMessages.Invalidate();
                e.SuppressKeyPress = true;
                e.Handled = true;
                return;
            }

            // Delete – удалить выделенные сообщения
            if (e.KeyCode == Keys.Delete && selectedMessageIndices.Count > 0)
            {
                var selectedMessages = selectedMessageIndices
                    .Select(i => lstMessages.Items[i] as Shared.Message)
                    .Where(m => m != null)
                    .ToList();

                if (selectedMessages.Count == 0)
                {
                    MessageBox.Show("Выберите хотя бы одно сообщение.", "Подсказка",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.Handled = true;
                    return;
                }

                // Проверка прав: только свои сообщения или админ
                bool canDeleteAll = selectedMessages.All(msg => msg.SenderId == currentUser.Id || currentUser.IsAdmin);
                if (!canDeleteAll)
                {
                    MessageBox.Show("Вы можете удалять только свои сообщения.", "Доступ запрещён",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handled = true;
                    return;
                }

                // Подтверждение удаления
                string confirmMsg = selectedMessages.Count == 1
                    ? "Удалить выбранное сообщение?"
                    : $"Удалить {selectedMessages.Count} сообщений?";
                if (MessageBox.Show(confirmMsg, "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    e.Handled = true;
                    return;
                }

                // Отправляем команду на удаление для каждого сообщения
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
            // Escape – сброс выделения
            else if (e.KeyCode == Keys.Escape)
            {
                lstMessages.ClearSelected();
                lstMessages.SelectedIndex = -1;
                e.Handled = true;
                txtMessage.Focus();
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

            // Разделитель даты (строка)
            if (lstMessages.Items[e.Index] is string)
            {
                e.ItemHeight = 30;
                return;
            }

            // Сообщение
            if (lstMessages.Items[e.Index] is Shared.Message msg)
            {
                const int maxWidth = 400;          // ширина "пузыря"
                const int padding = 10;             // отступы внутри пузыря
                int textWidth = maxWidth - 2 * padding;

                int y = 5; // верхний отступ

                // Имя отправителя
                string senderDisplay = string.IsNullOrEmpty(msg.SenderDepartment)
                    ? msg.SenderName
                    : $"{msg.SenderName} ({msg.SenderDepartment})";
                SizeF senderSize = e.Graphics.MeasureString(senderDisplay, messageSenderFont, textWidth);
                y += (int)senderSize.Height + 5;

                // Текст сообщения
                SizeF textSize = e.Graphics.MeasureString(msg.Text, messageTextFont, textWidth);
                y += (int)textSize.Height + 5;

                // Время
                string timeStr = msg.SentAt.ToString("HH:mm");
                SizeF timeSize = e.Graphics.MeasureString(timeStr, messageTimeFont);
                y += (int)timeSize.Height + 5;

                e.ItemHeight = y;
            }
        }

        private void TxtMessage_TextChanged(object sender, EventArgs e)
        {
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
        }

        private void LstMessages_MouseDown(object sender, MouseEventArgs e)
        {
            int index = lstMessages.IndexFromPoint(e.Location);
            bool clickOnEmpty = false;

            if (index == -1)
                clickOnEmpty = true;
            else
            {
                Rectangle rect = lstMessages.GetItemRectangle(index);
                if (!rect.Contains(e.Location))
                    clickOnEmpty = true;
            }

            // Клик на пустом месте или на разделителе даты -> сброс выделения
            if (clickOnEmpty || (index >= 0 && lstMessages.Items[index] is string))
            {
                selectedMessageIndices.Clear();
                lastSelectedIndex = -1;
                lstMessages.Invalidate();
                return;
            }

            // Клик на сообщении
            if (index < 0 || !(lstMessages.Items[index] is Shared.Message))
                return;

            if (ModifierKeys.HasFlag(Keys.Control))
            {
                // Ctrl: инвертируем выделение текущего сообщения
                if (selectedMessageIndices.Contains(index))
                    selectedMessageIndices.Remove(index);
                else
                    selectedMessageIndices.Add(index);
                lastSelectedIndex = index;
            }
            else if (ModifierKeys.HasFlag(Keys.Shift) && lastSelectedIndex != -1)
            {
                // Shift: выделяем диапазон от lastSelectedIndex до index
                int start = Math.Min(lastSelectedIndex, index);
                int end = Math.Max(lastSelectedIndex, index);
                for (int i = start; i <= end; i++)
                {
                    if (lstMessages.Items[i] is Shared.Message && !selectedMessageIndices.Contains(i))
                        selectedMessageIndices.Add(i);
                }
                lastSelectedIndex = index;
            }
            else
            {
                // Без модификаторов: сбрасываем всё и выделяем только текущее сообщение
                selectedMessageIndices.Clear();
                selectedMessageIndices.Add(index);
                lastSelectedIndex = index;
            }
            lstMessages.Invalidate();
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

        // Создание скруглённой области для контрола
        private void SetRoundedRegion(Control ctrl, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(ctrl.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(ctrl.Width - radius, ctrl.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, ctrl.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            ctrl.Region = new Region(path);
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
    }
}
