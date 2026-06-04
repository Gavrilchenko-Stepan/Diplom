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

        // Данные для пользователей
        private List<User> allUsers;
        private List<Department> departments;

        // Данные для истории
        private List<Chat> allChats;
        private List<HistoryMessage> currentMessages;

        // Данные для чатов
        private List<Chat> allChatsList;

        private int? _pendingSelectedChatId = null;

        // Шрифты для сообщений
        private Font messageSenderFont;
        private Font messageTextFont;
        private Font messageTimeFont;
        private Font dateFont;

        public AdminDashboardForm(NetworkClient client, int userId)
        {
            InitializeComponent();
            cmbChat.DataSource = null;

            // Инициализация шрифтов
            messageSenderFont = new Font("Segoe UI", 9, FontStyle.Bold);
            messageTextFont = new Font("Segoe UI", 10);
            messageTimeFont = new Font("Segoe UI", 8);
            dateFont = new Font("Segoe UI", 9, FontStyle.Bold);

            networkClient = client;
            currentUserId = userId;
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
            chkAllChats.CheckedChanged += (s, e) => { if (chkAllChats.Checked) cmbChat.SelectedIndex = -1; };
            cmbChat.SelectedIndexChanged += (s, e) => chkAllChats.Checked = false;

            LoadData();
        }

        private void LoadData()
        {
            var selectedChat = cmbChat.SelectedItem as Chat;
            if (selectedChat != null)
                _pendingSelectedChatId = selectedChat.Id;
            else
                _pendingSelectedChatId = null;

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

                        cmbChat.DataSource = null;
                        cmbChat.DataSource = allChats;
                        cmbChat.DisplayMember = "Name";
                        cmbChat.ValueMember = "Id";

                        if (_pendingSelectedChatId.HasValue)
                        {
                            var chatToSelect = allChats.FirstOrDefault(c => c.Id == _pendingSelectedChatId.Value);
                            if (chatToSelect != null)
                                cmbChat.SelectedItem = chatToSelect;
                            _pendingSelectedChatId = null;
                        }

                        if (cmbChat.SelectedIndex == -1 && cmbChat.Items.Count > 0)
                        {
                            cmbChat.SelectedIndex = 0;
                        }

                        UpdateChatsGrid();
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
            int? chatId = null;
            if (!chkAllChats.Checked && cmbChat.SelectedItem != null)
                chatId = (int)cmbChat.SelectedValue;

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
            if (MessageBox.Show("Удалить сообщения старше выбранной даты? (Без возможности восстановления!)", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            int? chatId = null;
            if (!chkAllChats.Checked && cmbChat.SelectedItem != null)
                chatId = (int)cmbChat.SelectedValue;
            var olderThan = dtpEnd.Value.Date;
            var data = new { chatId = chatId, olderThan = olderThan };
            networkClient.SendPacket(new NetworkPacket
            {
                Command = Shared.CommandType.DeleteHistoryMessages,
                Data = data
            });
        }

        // ================== Управление чатами ==================
        private void UpdateChatsGrid()
        {
            if (allChatsList == null) return;

            int? selectedChatId = null;
            if (dgvChats.SelectedRows.Count > 0)
            {
                var cell = dgvChats.SelectedRows[0].Cells[0];
                if (cell.Value != null && cell.Value != DBNull.Value)
                    selectedChatId = Convert.ToInt32(cell.Value);
            }

            dgvChats.Rows.Clear();
            dgvChats.Columns.Clear();
            dgvChats.Columns.Add("Id", "ID");
            dgvChats.Columns.Add("Name", "Название");
            dgvChats.Columns.Add("Type", "Тип");
            dgvChats.Columns[0].Visible = false;

            foreach (var chat in allChatsList)
                dgvChats.Rows.Add(chat.Id, chat.Name, chat.Type);

            dgvChats.ClearSelection();
            dgvChats.CurrentCell = null;

            if (selectedChatId.HasValue)
            {
                foreach (DataGridViewRow row in dgvChats.Rows)
                {
                    if (Convert.ToInt32(row.Cells[0].Value) == selectedChatId.Value)
                    {
                        row.Selected = true;
                        if (row.Cells.Count > 1)
                            dgvChats.CurrentCell = row.Cells[1];
                        break;
                    }
                }
            }

            dgvChats.AutoResizeColumns();
        }

        private void BtnRefreshChats_Click(object sender, EventArgs e) => LoadData();

        private void BtnDeleteChat_Click(object sender, EventArgs e)
        {
            if (dgvChats.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите чат для удаления.");
                return;
            }
            int chatId = Convert.ToInt32(dgvChats.SelectedRows[0].Cells[0].Value);
            string chatName = dgvChats.SelectedRows[0].Cells[1].Value.ToString();
            if (MessageBox.Show($"Удалить чат \"{chatName}\"?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                networkClient.SendPacket(new NetworkPacket { Command = Shared.CommandType.DeleteChat, Data = chatId });
                LoadData();
            }
        }

        private void BtnViewParticipants_Click(object sender, EventArgs e)
        {
            if (dgvChats.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите чат для просмотра участников.");
                return;
            }
            int chatId = Convert.ToInt32(dgvChats.SelectedRows[0].Cells[0].Value);
            using (var form = new ManageParticipantsForm(chatId, currentUserId, networkClient))
                form.ShowDialog();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            networkClient.OnPacketReceived -= OnPacketReceived;
            base.OnFormClosing(e);
        }
    }
}