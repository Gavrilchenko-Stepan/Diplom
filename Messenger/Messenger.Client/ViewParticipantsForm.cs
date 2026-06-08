using Messenger.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Messenger.Client
{
    public partial class ViewParticipantsForm : Form
    {
        private int chatId;
        private int currentUserId;
        private NetworkClient networkClient;
        private Chat currentChat;

        public ViewParticipantsForm(int chatId, int currentUserId, NetworkClient networkClient)
        {
            InitializeComponent();
            this.Load += (s, e) => SetRoundedRegion(this, 20);
            this.chatId = chatId;
            this.currentUserId = currentUserId;
            this.networkClient = networkClient;
            this.networkClient.OnPacketReceived += OnPacketReceived;
            this.Load += ViewParticipantsForm_Load;
        }

        private void ViewParticipantsForm_Load(object sender, EventArgs e)
        {
            networkClient.SendPacket(new NetworkPacket
            {
                Command = Shared.CommandType.GetChatInfo,
                Data = chatId
            });
        }

        private void OnPacketReceived(NetworkPacket packet)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<NetworkPacket>(OnPacketReceived), packet);
                return;
            }

            if (packet.Command == Shared.CommandType.ChatInfo)
            {
                var jsonElem = (JsonElement)packet.Data;
                string json = jsonElem.GetRawText();
                Chat chat = null;
                try
                {
                    chat = JsonSerializer.Deserialize<Chat>(json);
                }
                catch (JsonException ex)
                {
                    MessageBox.Show($"Ошибка загрузки участников: {ex.Message}");
                    return;
                }
                currentChat = chat;
                if (currentChat != null)
                {
                    currentChat.Participants = currentChat.Participants ?? new List<User>();
                }
                UpdateParticipantsList(); // вызов ПЕРЕНЕСЁН СЮДА
            }
            else if (packet.Command == Shared.CommandType.UserStatusChanged)
            {
                // Обновление статуса в реальном времени
                var jsonElem = (JsonElement)packet.Data;
                string json = jsonElem.GetRawText();
                User updatedUser = null;
                try
                {
                    updatedUser = JsonSerializer.Deserialize<User>(json);
                }
                catch (JsonException) { return; }
                if (updatedUser != null && currentChat != null)
                {
                    UpdateUserStatusInList(updatedUser);
                }
            }
        }

        // Обновление статуса конкретного пользователя в ListView
        private void UpdateUserStatusInList(User updatedUser)
        {
            for (int i = 0; i < listViewParticipants.Items.Count; i++)
            {
                var item = listViewParticipants.Items[i];
                if (item.Tag is User user && user.Id == updatedUser.Id)
                {
                    user.IsOnline = updatedUser.IsOnline;
                    user.LastSeen = updatedUser.LastSeen;
                    item.SubItems[2].Text = updatedUser.IsOnline ? "● Онлайн" : "● Офлайн";
                    item.ForeColor = updatedUser.IsOnline ? Color.Green : Color.Red;
                    break;
                }
            }
        }

        private void UpdateParticipantsList()
        {
            if (currentChat == null) return; // ЗАЩИТА ОТ null

            listViewParticipants.Items.Clear();
            foreach (var user in currentChat.Participants.OrderBy(u => u.FullName))
            {
                ListViewItem item = new ListViewItem(user.FullName);
                item.SubItems.Add(user.Position ?? "");
                item.SubItems.Add(user.IsOnline ? "● Онлайн" : "● Офлайн");
                item.Tag = user;
                item.ForeColor = user.IsOnline ? Color.Green : Color.Red;
                listViewParticipants.Items.Add(item);
            }
            lblChatName.Text = currentChat.Name;
            lblParticipantCount.Text = $"Участников: {currentChat.Participants.Count}";
        }

        private void BtnClose_Click(object sender, EventArgs e) => Close();

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            networkClient.OnPacketReceived -= OnPacketReceived;
            base.OnFormClosing(e);
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
    }
}
