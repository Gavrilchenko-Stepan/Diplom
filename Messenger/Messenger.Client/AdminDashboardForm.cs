using Messenger.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace Messenger.Client
{
    public partial class AdminDashboardForm : Form
    {
        private NetworkClient networkClient;
        private int currentUserId;
        private int? _currentChatId = null;

        // Данные для пользователей
        private List<User> allUsers;
        private List<Department> departments;

        // Данные для истории
        private List<Chat> allChats;
        private List<HistoryMessage> currentMessages;

        // Данные для чатов
        private List<Chat> allChatsList;

        // Шрифты для сообщений
        private Font messageSenderFont;
        private Font messageTextFont;
        private Font messageTimeFont;
        private Font dateFont;

        public AdminDashboardForm(NetworkClient client, int userId)
        {
            InitializeComponent();

            this.Load += (s, e) => SetRoundedRegion(this, 20);
            this.tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;

            cmbChat.DataSource = null;

            // Инициализация шрифтов
            messageSenderFont = new Font("Segoe UI", 9, FontStyle.Bold);
            messageTextFont = new Font("Segoe UI", 10);
            messageTimeFont = new Font("Segoe UI", 8);
            dateFont = new Font("Segoe UI", 9, FontStyle.Bold);

            networkClient = client;
            currentUserId = userId;

            cmbChat.SelectedIndexChanged += (s, e) =>
            {
                if (cmbChat.SelectedItem is Chat selectedChat)
                    _currentChatId = selectedChat.Id;
                else
                    _currentChatId = null;
            };

            networkClient.OnPacketReceived += OnPacketReceived;

            // Подписки на кнопки
            btnSearch.Click += BtnSearch_Click;
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefreshUsers.Click += BtnRefreshUsers_Click;

            btnLoadHistory.Click += BtnLoadHistory_Click;
            btnExport.Click += BtnExport_Click;
            btnClearHistory.Click += BtnClearHistory_Click;

            btnRefreshChats.Click += BtnRefreshChats_Click;
            btnDeleteChat.Click += BtnDeleteChat_Click;
            btnViewParticipants.Click += BtnViewParticipants_Click;

            txtSearch.TextChanged += TxtSearch_TextChanged;
            tvUsers.NodeMouseDoubleClick += TvUsers_NodeMouseDoubleClick;



            LoadData();
        }

        private void LoadData()
        {
            _selectedChatIdForChats = (tvChats.SelectedNode?.Tag as Chat)?.Id;

            networkClient.SendPacket(new NetworkPacket { Command = Shared.CommandType.GetAllUsers });
            networkClient.SendPacket(new NetworkPacket { Command = Shared.CommandType.GetDepartments });
            networkClient.SendPacket(new NetworkPacket { Command = Shared.CommandType.GetChatsForHistory });
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
                    case Shared.CommandType.AllUsersList:
                        var json = ((JsonElement)packet.Data).GetRawText();
                        allUsers = JsonSerializer.Deserialize<List<User>>(json);
                        UpdateTree(allUsers);
                        break;
                    case Shared.CommandType.DepartmentsList:
                        var jsonDept = ((JsonElement)packet.Data).GetRawText();
                        departments = JsonSerializer.Deserialize<List<Department>>(jsonDept);
                        break;
                    case Shared.CommandType.ChatsList:
                        var jsonChats = ((JsonElement)packet.Data).GetRawText();
                        var chats = JsonSerializer.Deserialize<List<Chat>>(jsonChats);
                        allChats = chats;
                        allChatsList = chats;

                        // Запоминаем текущий выбранный ID до пересоздания источника
                        int? previouslySelectedChatId = _currentChatId;

                        cmbChat.DataSource = null;
                        cmbChat.DataSource = allChats;
                        cmbChat.DisplayMember = "Name";
                        cmbChat.ValueMember = "Id";

                        // Восстанавливаем выбранный чат по ID
                        if (previouslySelectedChatId.HasValue && allChats.Any(c => c.Id == previouslySelectedChatId.Value))
                        {
                            cmbChat.SelectedValue = previouslySelectedChatId.Value;
                        }
                        else if (cmbChat.Items.Count > 0)
                        {
                            // Если восстановить не удалось, выбираем первый и синхронизируем _currentChatId
                            cmbChat.SelectedIndex = 0;
                            if (cmbChat.SelectedItem is Chat firstChat)
                                _currentChatId = firstChat.Id;
                        }

                        UpdateChatsTree();
                        break;
                    case Shared.CommandType.MessagesList:
                        var jsonMsgs = ((JsonElement)packet.Data).GetRawText();
                        currentMessages = JsonSerializer.Deserialize<List<HistoryMessage>>(jsonMsgs);
                        DisplayMessages();
                        break;
                    case Shared.CommandType.Error:
                        MessageBox.Show(packet.Data.ToString(), "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case Shared.CommandType.ChatDeleted:
                        LoadData();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void SetRoundedRegion(Control ctrl, int radius)
        {
            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(ctrl.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(ctrl.Width - radius, ctrl.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, ctrl.Height - radius, radius, radius, 90, 90);
                path.CloseFigure();
                ctrl.Region = new Region(path);
            }
        }

        // ================== Управление пользователями ==================
        private void UpdateTree(List<User> users)
        {
            tvUsers.Nodes.Clear();
            var usersByDept = users.GroupBy(u => u.Department).OrderBy(g => g.Key);
            foreach (var group in usersByDept)
            {
                TreeNode deptNode = new TreeNode(group.Key);
                deptNode.ForeColor = Color.White;
                foreach (var user in group.OrderBy(u => u.FullName))
                {
                    string display = user.FullName;
                    if (!string.IsNullOrEmpty(user.Position))
                        display += $" ({user.Position})";
                    if (user.IsAdmin)
                        display += " [Admin]";
                    TreeNode userNode = new TreeNode(display);
                    userNode.Tag = user;
                    userNode.ForeColor = Color.White;
                    deptNode.Nodes.Add(userNode);
                }
                tvUsers.Nodes.Add(deptNode);
            }
            tvUsers.ExpandAll();
        }

        private void FilterTree(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                UpdateTree(allUsers);
                return;
            }
            var filtered = allUsers.Where(u => u.FullName.ToLower().Contains(searchText.ToLower()) ||
                                               (u.Position != null && u.Position.ToLower().Contains(searchText.ToLower()))).ToList();
            UpdateTree(filtered);
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e) => FilterTree(txtSearch.Text);
        private void BtnSearch_Click(object sender, EventArgs e) => FilterTree(txtSearch.Text);
        private void TvUsers_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag is User user)
                EditUser(user);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new EditUserForm(null, departments, networkClient))
            {
                if (form.ShowDialog() == DialogResult.OK)
                    LoadData();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (tvUsers.SelectedNode?.Tag is User user)
                EditUser(user);
            else
                MessageBox.Show("Выберите пользователя для редактирования.");
        }

        private void EditUser(User user)
        {
            using (var form = new EditUserForm(user, departments, networkClient))
            {
                if (form.ShowDialog() == DialogResult.OK)
                    LoadData();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (tvUsers.SelectedNode?.Tag is User user)
            {
                if (user.Id == currentUserId)
                {
                    MessageBox.Show("Нельзя удалить самого себя.");
                    return;
                }
                var result = MessageBox.Show($"Удалить пользователя {user.FullName}?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    networkClient.SendPacket(new NetworkPacket { Command = Shared.CommandType.DeleteUser, Data = user.Id });
            }
            else
                MessageBox.Show("Выберите пользователя для удаления.");
        }

        private void BtnRefreshUsers_Click(object sender, EventArgs e) => LoadData();

        // ================== История сообщений ==================
        private void BtnLoadHistory_Click(object sender, EventArgs e)
        {
            if (cmbChat.SelectedItem == null)
            {
                MessageBox.Show("Выберите чат для просмотра истории.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int chatId = (int)cmbChat.SelectedValue;
            var filter = new
            {
                chatId = chatId,
                startDate = dtpStart.Value.Date,
                endDate = dtpEnd.Value.Date.AddDays(1).AddSeconds(-1)
            };
            networkClient.SendPacket(new NetworkPacket
            {
                Command = Shared.CommandType.GetHistoryMessages,
                Data = filter
            });
        }

        private void DisplayMessages()
        {
            lstHistoryMessages.BeginUpdate();
            lstHistoryMessages.Items.Clear();

            if (currentMessages == null || currentMessages.Count == 0)
            {
                lstHistoryMessages.EndUpdate();
                MessageBox.Show("Нет сообщений для отображения.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DateTime? lastDate = null;
            foreach (var msg in currentMessages)
            {
                if (lastDate == null || msg.SentAt.Date != lastDate.Value.Date)
                {
                    lstHistoryMessages.Items.Add(msg.SentAt.ToString("d MMMM yyyy"));
                    lastDate = msg.SentAt.Date;
                }
                lstHistoryMessages.Items.Add(msg);
            }

            if (lstHistoryMessages.Items.Count > 0)
                lstHistoryMessages.TopIndex = lstHistoryMessages.Items.Count - 1;

            lstHistoryMessages.EndUpdate();
        }

        private void LstHistory_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0 || lstHistoryMessages.Items[e.Index] == null) return;

            if (lstHistoryMessages.Items[e.Index] is string)
            {
                e.ItemHeight = 30;
                return;
            }

            if (lstHistoryMessages.Items[e.Index] is HistoryMessage msg)
            {
                const int maxWidth = 500;
                const int padding = 10;
                int textWidth = maxWidth - 2 * padding;
                int y = 5;

                string senderDisplay = $"{msg.UserName} ({msg.Department})";
                SizeF senderSize = e.Graphics.MeasureString(senderDisplay, messageSenderFont, textWidth);
                y += (int)senderSize.Height + 5;

                SizeF textSize = e.Graphics.MeasureString(msg.MessageText, messageTextFont, textWidth);
                y += (int)textSize.Height + 5;

                y += 20;
                e.ItemHeight = y;
            }
        }

        private void LstHistory_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            object item = lstHistoryMessages.Items[e.Index];
            if (item == null) return;

            if (item is string dateStr)
            {
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (var brush = new SolidBrush(Color.FromArgb(180, 180, 200)))
                    e.Graphics.DrawString(dateStr, dateFont, brush, e.Bounds, sf);
                return;
            }

            if (!(item is HistoryMessage msg)) return;

            const int maxWidth = 500;
            const int padding = 10;
            int x = e.Bounds.Left + 20;
            int textWidth = maxWidth - 2 * padding;
            Rectangle bubbleRect = new Rectangle(x, e.Bounds.Y + 2, maxWidth, e.Bounds.Height - 4);

            Color bgColor = Color.FromArgb(240, 240, 240);
            Color borderColor = Color.Gray;

            using (var path = GetRoundedRect(bubbleRect, 10))
            using (var brush = new SolidBrush(bgColor))
            using (var pen = new Pen(borderColor, 1))
            {
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }

            int currentY = e.Bounds.Y + 5;
            string senderDisplay = $"{msg.UserName} ({msg.Department})";
            using (var brush = new SolidBrush(Color.Black))
                e.Graphics.DrawString(senderDisplay, messageSenderFont, brush, new RectangleF(x + padding, currentY, textWidth, 100));
            SizeF senderSize = e.Graphics.MeasureString(senderDisplay, messageSenderFont, textWidth);
            currentY += (int)senderSize.Height + 5;

            using (var brush = new SolidBrush(Color.Black))
                e.Graphics.DrawString(msg.MessageText, messageTextFont, brush, new RectangleF(x + padding, currentY, textWidth, 1000));
            SizeF textSize = e.Graphics.MeasureString(msg.MessageText, messageTextFont, textWidth);
            currentY += (int)textSize.Height + 5;

            string timeStr = msg.SentAt.ToString("HH:mm");
            if (msg.EditedAt.HasValue) timeStr += " (ред.)";
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

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (currentMessages == null || currentMessages.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта.");
                return;
            }
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.FileName = $"history_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("ID;Чат;Тип;Пользователь;Логин;Отдел;Текст;Дата;Редактировано");
                    foreach (var msg in currentMessages)
                    {
                        sb.AppendLine($"{msg.Id};{msg.ChatName};{msg.ChatType};{msg.UserName};{msg.Login};{msg.Department};{EscapeCsv(msg.MessageText)};{msg.SentAt};{msg.EditedAt}");
                    }
                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show($"История сохранена в файл:\n{sfd.FileName}", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private string EscapeCsv(string text) => string.IsNullOrEmpty(text) ? "" : "\"" + text.Replace("\"", "\"\"") + "\"";

        private void BtnClearHistory_Click(object sender, EventArgs e)
        {
            if (cmbChat.SelectedItem == null)
            {
                MessageBox.Show("Выберите чат для очистки истории.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Удалить сообщения старше выбранной даты? (Без возможности восстановления!)", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            int chatId = (int)cmbChat.SelectedValue;
            var olderThan = dtpEnd.Value.Date;
            var data = new { chatId = chatId, olderThan = olderThan };
            networkClient.SendPacket(new NetworkPacket
            {
                Command = Shared.CommandType.DeleteHistoryMessages,
                Data = data
            });
        }

        // ================== Управление чатами ==================
        private void UpdateChatsTree()
        {
            if (allChatsList == null) return;

            tvChats.BeginUpdate();
            try
            {
                tvChats.Nodes.Clear();

                // Группировка с русскими названиями (см. п.4)
                var groups = allChatsList.GroupBy(c => GetChatTypeDisplayName(c.Type))
                                          .OrderBy(g => g.Key);

                foreach (var group in groups)
                {
                    TreeNode groupNode = new TreeNode(group.Key);
                    groupNode.ForeColor = Color.White;
                    groupNode.Tag = null;

                    foreach (var chat in group.OrderBy(c => c.Name))
                    {
                        TreeNode chatNode = new TreeNode(chat.Name);
                        chatNode.Tag = chat;
                        chatNode.ForeColor = Color.White;
                        groupNode.Nodes.Add(chatNode);
                    }

                    tvChats.Nodes.Add(groupNode);
                }

                tvChats.ExpandAll();

                if (_selectedChatIdForChats.HasValue)
                {
                    SelectChatNodeById(_selectedChatIdForChats.Value);
                    _selectedChatIdForChats = null;
                }
            }
            finally
            {
                tvChats.EndUpdate();
            }
        }

        private string GetChatTypeDisplayName(ChatType type)
        {
            switch (type)
            {
                case ChatType.Private: return "Личные чаты";
                case ChatType.Group: return "Групповые чаты";
                case ChatType.Department: return "Чаты отделов";
                default: return type.ToString();
            }
        }

        private int? _selectedChatIdForChats = null;

        private void SelectChatNodeById(int chatId)
        {
            foreach (TreeNode groupNode in tvChats.Nodes)
            {
                foreach (TreeNode chatNode in groupNode.Nodes)
                {
                    Chat chat = chatNode.Tag as Chat;
                    if (chat != null && chat.Id == chatId)
                    {
                        tvChats.SelectedNode = chatNode;
                        return;
                    }
                }
            }
        }

        private void BtnRefreshChats_Click(object sender, EventArgs e) => LoadData();

        private void BtnDeleteChat_Click(object sender, EventArgs e)
        {
            // Получаем выбранный узел и извлекаем Chat через as
            var selectedNode = tvChats.SelectedNode;
            var selectedChat = selectedNode?.Tag as Chat;

            if (selectedChat == null)
            {
                MessageBox.Show("Выберите чат для удаления.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить чат \"{selectedChat.Name}\"?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                networkClient.SendPacket(new NetworkPacket { Command = Shared.CommandType.DeleteChat, Data = selectedChat.Id });
                LoadData();
            }
        }

        private void BtnViewParticipants_Click(object sender, EventArgs e)
        {
            var selectedNode = tvChats.SelectedNode;
            var selectedChat = selectedNode?.Tag as Chat;

            if (selectedChat == null)
            {
                MessageBox.Show("Выберите чат для просмотра участников.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var form = new ManageParticipantsForm(selectedChat.Id, currentUserId, networkClient))
                form.ShowDialog();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            networkClient.OnPacketReceived -= OnPacketReceived;
            base.OnFormClosing(e);
        }
    }
}