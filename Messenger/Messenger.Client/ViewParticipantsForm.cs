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
                currentChat = JsonSerializer.Deserialize<Chat>(json);
                if (currentChat != null)
                {
                    currentChat.Participants = currentChat.Participants ?? new List<User>();
                }
            }
            UpdateParticipantsList();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

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

        private void listViewParticipants_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            // Рисуем только если индекс колонки 0,1,2
            if (e.ColumnIndex < 0 || e.ColumnIndex > 2) return;

            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(230, 230, 230)), e.Bounds);
            using (Pen borderPen = new Pen(Color.FromArgb(150, 150, 150), 1))
            {
                e.Graphics.DrawLine(borderPen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }
            using (Font boldFont = new Font(e.Font, FontStyle.Bold))
            {
                TextRenderer.DrawText(e.Graphics, e.Header.Text, boldFont, e.Bounds, Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            }
        }

        private void listViewParticipants_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            // Рисуем ячейки только для колонок 0,1,2
            if (e.ColumnIndex < 0 || e.ColumnIndex > 2) return;

            e.Graphics.FillRectangle(Brushes.White, e.Bounds);
            Color textColor = Color.Black;

            if (e.ColumnIndex == 2) // статус
            {
                string status = e.SubItem.Text;
                if (status.Contains("Онлайн"))
                    textColor = Color.Green;
                else if (status.Contains("Офлайн"))
                    textColor = Color.Red;
            }

            TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, e.Bounds, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
        }

        private void UpdateParticipantsList()
        {
            listViewParticipants.Items.Clear();
            foreach (var user in currentChat.Participants.OrderBy(u => u.FullName))
            {
                ListViewItem item = new ListViewItem(user.FullName);
                item.SubItems.Add(user.Position ?? "");
                item.SubItems.Add(user.IsOnline ? "● Онлайн" : "● Офлайн");
                item.Tag = user;
                listViewParticipants.Items.Add(item);
            }
            lblChatName.Text = currentChat.Name;
            lblParticipantCount.Text = $"Участников: {currentChat.Participants.Count}";
        }
    }
}
