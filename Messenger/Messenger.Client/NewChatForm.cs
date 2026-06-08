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
    public partial class NewChatForm : Form
    {
        private int currentUserId;
        private string currentDepartment;
        private NetworkClient networkClient;
        private List<User> availableUsers = new List<User>();

        public NewChatForm(int userId, string department, NetworkClient client, bool isAdmin)
        {
            InitializeComponent();
            this.Load += (s, e) => SetRoundedRegion(this, 20); // скругление углов
            currentUserId = userId;
            currentDepartment = department;
            networkClient = client;
            networkClient.OnPacketReceived += OnPacketReceived;
            this.Load += NewChatForm_Load;

            btnCreate.Click += BtnCreate_Click;
            btnCancel.Click += BtnCancel_Click;
            tvPrivateUsers.NodeMouseDoubleClick += TvPrivateUsers_NodeMouseDoubleClick;
            tvPrivateUsers.AfterSelect += TvPrivateUsers_AfterSelect;
            tvGroupUsers.AfterCheck += TvGroupUsers_AfterCheck;
            txtChatName.TextChanged += TxtChatName_TextChanged;
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
        }

        // Скругление углов формы (единый стиль)
        private void SetRoundedRegion(Control ctrl, int radius)
        {
            using (var path = new GraphicsPath())
            {
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(ctrl.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(ctrl.Width - radius, ctrl.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, ctrl.Height - radius, radius, radius, 90, 90);
                path.CloseFigure();
                ctrl.Region = new Region(path);
            }
        }

        private void NewChatForm_Load(object sender, EventArgs e)
        {
            networkClient.SendPacket(new NetworkPacket { Command = Shared.CommandType.GetAvailableUsers, Data = currentUserId });
        }

        private void OnPacketReceived(NetworkPacket packet)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<NetworkPacket>(OnPacketReceived), packet);
                return;
            }

            Console.WriteLine($"NewChatForm получил: {packet.Command}");

            try
            {
                switch (packet.Command)
                {
                    case Shared.CommandType.AvailableUsersList:
                        var jsonUsers = (JsonElement)packet.Data;
                        string usersJson = jsonUsers.GetRawText();
                        availableUsers = JsonSerializer.Deserialize<List<User>>(usersJson);
                        UpdateUsersList();
                        break;
                    case Shared.CommandType.ChatCreated:
                        DialogResult = DialogResult.OK;
                        Close();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обработки пакета: {ex.Message}");
            }
        }

        private void UpdateUsersList()
        {
            // Личное дерево
            tvPrivateUsers.Nodes.Clear();
            var privateUsers = availableUsers.Where(u => u.Id != currentUserId).ToList();
            var privateGroups = privateUsers
                .GroupBy(u => u.Department ?? "Без отдела")
                .OrderBy(g => g.Key == "Без отдела" ? 0 : 1)
                .ThenBy(g => g.Key);

            foreach (var group in privateGroups)
            {
                TreeNode deptNode = new TreeNode(group.Key);
                deptNode.ForeColor = Color.White;
                foreach (var user in group.OrderBy(u => u.FullName))
                {
                    string display = user.FullName;
                    if (!string.IsNullOrEmpty(user.Position))
                        display += $" ({user.Position})";
                    TreeNode userNode = new TreeNode(display);
                    userNode.Tag = user;
                    userNode.ForeColor = Color.White;
                    deptNode.Nodes.Add(userNode);
                }
                tvPrivateUsers.Nodes.Add(deptNode);
            }
            tvPrivateUsers.ExpandAll();

            // Групповое дерево (с чекбоксами)
            tvGroupUsers.Nodes.Clear();
            var groupUsers = availableUsers.Where(u => u.Id != currentUserId).ToList();
            var groupGroups = groupUsers
                .GroupBy(u => u.Department ?? "Без отдела")
                .OrderBy(g => g.Key);

            foreach (var group in groupGroups)
            {
                TreeNode deptNode = new TreeNode(group.Key);
                deptNode.ForeColor = Color.White;
                foreach (var user in group.OrderBy(u => u.FullName))
                {
                    string display = user.FullName;
                    if (!string.IsNullOrEmpty(user.Position))
                        display += $" ({user.Position})";
                    TreeNode userNode = new TreeNode(display);
                    userNode.Tag = user;
                    userNode.ForeColor = Color.White;
                    userNode.Checked = false; // для группового чата
                    deptNode.Nodes.Add(userNode);
                }
                tvGroupUsers.Nodes.Add(deptNode);
            }
            tvGroupUsers.ExpandAll();

            UpdateCreateButton();
        }

        private void UpdateCreateButton()
        {
            if (tabControl.SelectedTab == tabPrivate)
                btnCreate.Enabled = tvPrivateUsers.SelectedNode?.Tag is User;
            else // tabGroup
                btnCreate.Enabled = !string.IsNullOrWhiteSpace(txtChatName.Text) && GetCheckedGroupUsers().Count > 0;
        }

        private List<User> GetCheckedGroupUsers()
        {
            var users = new List<User>();
            foreach (TreeNode deptNode in tvGroupUsers.Nodes)
            {
                foreach (TreeNode userNode in deptNode.Nodes)
                {
                    if (userNode.Checked && userNode.Tag is User user)
                        users.Add(user);
                }
            }
            return users;
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e) => UpdateCreateButton();
        private void LstDepartments_SelectedIndexChanged(object sender, EventArgs e) => UpdateCreateButton();
        private void TvPrivateUsers_AfterSelect(object sender, TreeViewEventArgs e) => UpdateCreateButton();
        private void TvGroupUsers_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
                UpdateCreateButton();
        }
        private void TxtChatName_TextChanged(object sender, EventArgs e) => UpdateCreateButton();

        private void TvPrivateUsers_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag is User user)
            {
                networkClient.SendPacket(new NetworkPacket
                {
                    Command = Shared.CommandType.CreatePrivateChat,
                    Data = new { otherUserId = user.Id }
                });
            }
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabPrivate && tvPrivateUsers.SelectedNode?.Tag is User selectedUser)
            {
                networkClient.SendPacket(new NetworkPacket
                {
                    Command = Shared.CommandType.CreatePrivateChat,
                    Data = new { otherUserId = selectedUser.Id }
                });
            }
            // Групповой чат
            else if (tabControl.SelectedTab == tabGroup)
            {
                var selectedUsers = GetCheckedGroupUsers();
                var participants = selectedUsers.Select(u => u.Id).ToList();
                participants.Add(currentUserId);
                networkClient.SendPacket(new NetworkPacket
                {
                    Command = Shared.CommandType.CreateGroupChat,
                    Data = new { name = txtChatName.Text, participants }
                });
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e) => Close();

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            networkClient.OnPacketReceived -= OnPacketReceived;
            base.OnFormClosing(e);
        }
    }
}
