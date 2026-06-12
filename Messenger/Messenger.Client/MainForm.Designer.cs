using System.Drawing;
using System.Windows.Forms;

namespace Messenger.Client
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.PictureBox picUserAvatar;
        private System.Windows.Forms.PictureBox picOnlineIndicator;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblUserDepartment;
        private System.Windows.Forms.Label lblUserPosition;
        private System.Windows.Forms.Button btnNewChat;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.ContextMenuStrip cmsSettingsMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiAdminPanel;
        private System.Windows.Forms.ToolStripMenuItem tsmiLogout;

        // Левая панель
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelLeftHeader;
        private System.Windows.Forms.Label lblChats;
        private System.Windows.Forms.TextBox txtSearchChats;
        private System.Windows.Forms.PictureBox picSearch;
        private System.Windows.Forms.Button btnClearSearch;
        private System.Windows.Forms.ListBox lstChats;
        private System.Windows.Forms.Panel panelLeftFooter;
        private System.Windows.Forms.Label lblTotalUsers;
        private System.Windows.Forms.Label lblOnlineCount;
        private System.Windows.Forms.Label lblSearchIcon;
        private System.Windows.Forms.Label lblClearSearch;

        // Правая панель
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelChatHeader;
        private System.Windows.Forms.PictureBox picChatAvatar;
        private System.Windows.Forms.Label lblChatName;
        private System.Windows.Forms.Button btnViewParticipants;
        private System.Windows.Forms.ListBox lstMessages;
        private System.Windows.Forms.Panel panelMessageInput;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSend;

        // Статус-бар
        private System.Windows.Forms.Panel panelStatusBar;
        private System.Windows.Forms.Label lblConnectionStatus;
        private System.Windows.Forms.Label lblServerInfo;
        private System.Windows.Forms.Label lblEditingHint;
        private System.Windows.Forms.Label lblLastSeen;
        private System.Windows.Forms.Label lblTyping;

        private System.Windows.Forms.Button btnHideChat;       // кнопка скрытия чата
        private System.Windows.Forms.Label lblWelcomePlaceholder;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            messageSenderFont?.Dispose();
            messageTextFont?.Dispose();
            messageTimeFont?.Dispose();
            dateFont?.Dispose();
            boldFont11?.Dispose();
            normalFont9?.Dispose();
            smallFont8?.Dispose();
            iconFont?.Dispose();
            typingTimer?.Dispose();
            clearTypingTimer?.Dispose();
            refreshTimer?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelTop = new System.Windows.Forms.Panel();
            this.picUserAvatar = new System.Windows.Forms.PictureBox();
            this.picOnlineIndicator = new System.Windows.Forms.PictureBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblUserDepartment = new System.Windows.Forms.Label();
            this.lblUserPosition = new System.Windows.Forms.Label();
            this.btnNewChat = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.cmsSettingsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAdminPanel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLogout = new System.Windows.Forms.ToolStripMenuItem();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelLeftHeader = new System.Windows.Forms.Panel();
            this.lblChats = new System.Windows.Forms.Label();
            this.txtSearchChats = new System.Windows.Forms.TextBox();
            this.picSearch = new System.Windows.Forms.PictureBox();
            this.btnClearSearch = new System.Windows.Forms.Button();
            this.lblSearchIcon = new System.Windows.Forms.Label();
            this.lblClearSearch = new System.Windows.Forms.Label();
            this.lstChats = new System.Windows.Forms.ListBox();
            this.panelLeftFooter = new System.Windows.Forms.Panel();
            this.lblTotalUsers = new System.Windows.Forms.Label();
            this.lblOnlineCount = new System.Windows.Forms.Label();
            this.panelRight = new System.Windows.Forms.Panel();
            this.panelChatHeader = new System.Windows.Forms.Panel();
            this.picChatAvatar = new System.Windows.Forms.PictureBox();
            this.lblChatName = new System.Windows.Forms.Label();
            this.btnViewParticipants = new System.Windows.Forms.Button();
            this.lblLastSeen = new System.Windows.Forms.Label();
            this.btnHideChat = new System.Windows.Forms.Button();
            this.lstMessages = new System.Windows.Forms.ListBox();
            this.panelMessageInput = new System.Windows.Forms.Panel();
            this.lblTyping = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.lblEditingHint = new System.Windows.Forms.Label();
            this.lblWelcomePlaceholder = new System.Windows.Forms.Label();
            this.panelStatusBar = new System.Windows.Forms.Panel();
            this.lblConnectionStatus = new System.Windows.Forms.Label();
            this.lblServerInfo = new System.Windows.Forms.Label();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUserAvatar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picOnlineIndicator)).BeginInit();
            this.cmsSettingsMenu.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelLeftHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSearch)).BeginInit();
            this.panelLeftFooter.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.panelChatHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picChatAvatar)).BeginInit();
            this.panelMessageInput.SuspendLayout();
            this.panelStatusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.Transparent;
            this.panelTop.Controls.Add(this.picUserAvatar);
            this.panelTop.Controls.Add(this.picOnlineIndicator);
            this.panelTop.Controls.Add(this.lblUserName);
            this.panelTop.Controls.Add(this.lblUserDepartment);
            this.panelTop.Controls.Add(this.lblUserPosition);
            this.panelTop.Controls.Add(this.btnNewChat);
            this.panelTop.Controls.Add(this.btnSettings);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1200, 100);
            this.panelTop.TabIndex = 2;
            this.panelTop.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelTop_Paint);
            // 
            // picUserAvatar
            // 
            this.picUserAvatar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picUserAvatar.Location = new System.Drawing.Point(24, 24);
            this.picUserAvatar.Name = "picUserAvatar";
            this.picUserAvatar.Size = new System.Drawing.Size(48, 48);
            this.picUserAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picUserAvatar.TabIndex = 0;
            this.picUserAvatar.TabStop = false;
            // 
            // picOnlineIndicator
            // 
            this.picOnlineIndicator.BackColor = System.Drawing.Color.Gray;
            this.picOnlineIndicator.Location = new System.Drawing.Point(58, 58);
            this.picOnlineIndicator.Name = "picOnlineIndicator";
            this.picOnlineIndicator.Size = new System.Drawing.Size(14, 14);
            this.picOnlineIndicator.TabIndex = 1;
            this.picOnlineIndicator.TabStop = false;
            // 
            // lblUserName
            // 
            this.lblUserName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblUserName.ForeColor = System.Drawing.Color.White;
            this.lblUserName.Location = new System.Drawing.Point(90, 18);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(350, 20);
            this.lblUserName.TabIndex = 2;
            this.lblUserName.Text = "👤 Загрузка...";
            // 
            // lblUserDepartment
            // 
            this.lblUserDepartment.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblUserDepartment.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblUserDepartment.Location = new System.Drawing.Point(94, 42);
            this.lblUserDepartment.Name = "lblUserDepartment";
            this.lblUserDepartment.Size = new System.Drawing.Size(400, 18);
            this.lblUserDepartment.TabIndex = 3;
            this.lblUserDepartment.Text = "🏢 Отдел: —";
            // 
            // lblUserPosition
            // 
            this.lblUserPosition.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblUserPosition.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(200)))));
            this.lblUserPosition.Location = new System.Drawing.Point(94, 62);
            this.lblUserPosition.Name = "lblUserPosition";
            this.lblUserPosition.Size = new System.Drawing.Size(400, 18);
            this.lblUserPosition.TabIndex = 4;
            this.lblUserPosition.Text = "💼 Должность: —";
            // 
            // btnNewChat
            // 
            this.btnNewChat.BackColor = System.Drawing.Color.Transparent;
            this.btnNewChat.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnNewChat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewChat.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnNewChat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnNewChat.Location = new System.Drawing.Point(1200, 0);
            this.btnNewChat.Name = "btnNewChat";
            this.btnNewChat.Size = new System.Drawing.Size(110, 36);
            this.btnNewChat.TabIndex = 5;
            this.btnNewChat.Text = "➕ Новый чат";
            this.btnNewChat.UseVisualStyleBackColor = false;
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.btnSettings.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.btnSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnSettings.Location = new System.Drawing.Point(3340, 30);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(40, 36);
            this.btnSettings.TabIndex = 6;
            this.btnSettings.Text = "⚙";
            this.btnSettings.UseVisualStyleBackColor = false;
            // 
            // cmsSettingsMenu
            // 
            this.cmsSettingsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAdminPanel,
            this.tsmiLogout});
            this.cmsSettingsMenu.Name = "cmsSettingsMenu";
            this.cmsSettingsMenu.Size = new System.Drawing.Size(125, 48);
            // 
            // tsmiAdminPanel
            // 
            this.tsmiAdminPanel.Name = "tsmiAdminPanel";
            this.tsmiAdminPanel.Size = new System.Drawing.Size(124, 22);
            this.tsmiAdminPanel.Text = "🛡️ Админ";
            this.tsmiAdminPanel.Visible = false;
            this.tsmiAdminPanel.Click += new System.EventHandler(this.BtnAdminPanel_Click);
            // 
            // tsmiLogout
            // 
            this.tsmiLogout.Name = "tsmiLogout";
            this.tsmiLogout.Size = new System.Drawing.Size(124, 22);
            this.tsmiLogout.Text = "🚪 Выход";
            this.tsmiLogout.Click += new System.EventHandler(this.BtnLogout_Click);
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(48)))));
            this.panelLeft.Controls.Add(this.panelLeftHeader);
            this.panelLeft.Controls.Add(this.lstChats);
            this.panelLeft.Controls.Add(this.panelLeftFooter);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 100);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(350, 575);
            this.panelLeft.TabIndex = 1;
            // 
            // panelLeftHeader
            // 
            this.panelLeftHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.panelLeftHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLeftHeader.Controls.Add(this.lblChats);
            this.panelLeftHeader.Controls.Add(this.txtSearchChats);
            this.panelLeftHeader.Controls.Add(this.picSearch);
            this.panelLeftHeader.Controls.Add(this.btnClearSearch);
            this.panelLeftHeader.Controls.Add(this.lblSearchIcon);
            this.panelLeftHeader.Controls.Add(this.lblClearSearch);
            this.panelLeftHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLeftHeader.Location = new System.Drawing.Point(0, 0);
            this.panelLeftHeader.Name = "panelLeftHeader";
            this.panelLeftHeader.Size = new System.Drawing.Size(350, 100);
            this.panelLeftHeader.TabIndex = 0;
            // 
            // lblChats
            // 
            this.lblChats.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblChats.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.lblChats.Location = new System.Drawing.Point(15, 10);
            this.lblChats.Name = "lblChats";
            this.lblChats.Size = new System.Drawing.Size(150, 30);
            this.lblChats.TabIndex = 0;
            this.lblChats.Text = "💬 ЧАТЫ";
            this.lblChats.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtSearchChats
            // 
            this.txtSearchChats.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(80)))));
            this.txtSearchChats.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearchChats.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtSearchChats.ForeColor = System.Drawing.Color.White;
            this.txtSearchChats.Location = new System.Drawing.Point(45, 55);
            this.txtSearchChats.Name = "txtSearchChats";
            this.txtSearchChats.Size = new System.Drawing.Size(255, 27);
            this.txtSearchChats.TabIndex = 1;
            this.txtSearchChats.Text = "Поиск чатов...";
            // 
            // picSearch
            // 
            this.picSearch.Location = new System.Drawing.Point(0, 0);
            this.picSearch.Name = "picSearch";
            this.picSearch.Size = new System.Drawing.Size(100, 50);
            this.picSearch.TabIndex = 2;
            this.picSearch.TabStop = false;
            // 
            // btnClearSearch
            // 
            this.btnClearSearch.Location = new System.Drawing.Point(0, 0);
            this.btnClearSearch.Name = "btnClearSearch";
            this.btnClearSearch.Size = new System.Drawing.Size(75, 23);
            this.btnClearSearch.TabIndex = 3;
            // 
            // lblSearchIcon
            // 
            this.lblSearchIcon.BackColor = System.Drawing.Color.Transparent;
            this.lblSearchIcon.Font = new System.Drawing.Font("Segoe UI Emoji", 12F);
            this.lblSearchIcon.ForeColor = System.Drawing.Color.Gray;
            this.lblSearchIcon.Location = new System.Drawing.Point(18, 56);
            this.lblSearchIcon.Name = "lblSearchIcon";
            this.lblSearchIcon.Size = new System.Drawing.Size(25, 25);
            this.lblSearchIcon.TabIndex = 4;
            this.lblSearchIcon.Text = "🔍";
            this.lblSearchIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblClearSearch
            // 
            this.lblClearSearch.BackColor = System.Drawing.Color.Transparent;
            this.lblClearSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblClearSearch.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblClearSearch.ForeColor = System.Drawing.Color.Gray;
            this.lblClearSearch.Location = new System.Drawing.Point(300, 55);
            this.lblClearSearch.Name = "lblClearSearch";
            this.lblClearSearch.Size = new System.Drawing.Size(25, 25);
            this.lblClearSearch.TabIndex = 5;
            this.lblClearSearch.Text = "✖";
            this.lblClearSearch.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblClearSearch.Visible = false;
            // 
            // lstChats
            // 
            this.lstChats.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(48)))));
            this.lstChats.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstChats.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstChats.ForeColor = System.Drawing.Color.White;
            this.lstChats.IntegralHeight = false;
            this.lstChats.ItemHeight = 70;
            this.lstChats.Location = new System.Drawing.Point(0, 100);
            this.lstChats.Name = "lstChats";
            this.lstChats.Size = new System.Drawing.Size(350, 449);
            this.lstChats.TabIndex = 1;
            // 
            // panelLeftFooter
            // 
            this.panelLeftFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.panelLeftFooter.Controls.Add(this.lblTotalUsers);
            this.panelLeftFooter.Controls.Add(this.lblOnlineCount);
            this.panelLeftFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelLeftFooter.Location = new System.Drawing.Point(0, 545);
            this.panelLeftFooter.Name = "panelLeftFooter";
            this.panelLeftFooter.Size = new System.Drawing.Size(350, 30);
            this.panelLeftFooter.TabIndex = 2;
            // 
            // lblTotalUsers
            // 
            this.lblTotalUsers.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTotalUsers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(200)))));
            this.lblTotalUsers.Location = new System.Drawing.Point(15, 5);
            this.lblTotalUsers.Name = "lblTotalUsers";
            this.lblTotalUsers.Size = new System.Drawing.Size(140, 20);
            this.lblTotalUsers.TabIndex = 0;
            this.lblTotalUsers.Text = "👥 Всего: 0";
            // 
            // lblOnlineCount
            // 
            this.lblOnlineCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblOnlineCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(200)))));
            this.lblOnlineCount.Location = new System.Drawing.Point(160, 5);
            this.lblOnlineCount.Name = "lblOnlineCount";
            this.lblOnlineCount.Size = new System.Drawing.Size(140, 20);
            this.lblOnlineCount.TabIndex = 1;
            this.lblOnlineCount.Text = "🟢 Онлайн: 0";
            // 
            // panelRight
            // 
            this.panelRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.panelRight.Controls.Add(this.panelChatHeader);
            this.panelRight.Controls.Add(this.lstMessages);
            this.panelRight.Controls.Add(this.panelMessageInput);
            this.panelRight.Controls.Add(this.lblWelcomePlaceholder);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(350, 100);
            this.panelRight.Name = "panelRight";
            this.panelRight.Padding = new System.Windows.Forms.Padding(10);
            this.panelRight.Size = new System.Drawing.Size(850, 575);
            this.panelRight.TabIndex = 0;
            // 
            // panelChatHeader
            // 
            this.panelChatHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.panelChatHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelChatHeader.Controls.Add(this.picChatAvatar);
            this.panelChatHeader.Controls.Add(this.lblChatName);
            this.panelChatHeader.Controls.Add(this.btnViewParticipants);
            this.panelChatHeader.Controls.Add(this.lblLastSeen);
            this.panelChatHeader.Controls.Add(this.btnHideChat);
            this.panelChatHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelChatHeader.Location = new System.Drawing.Point(10, 10);
            this.panelChatHeader.Name = "panelChatHeader";
            this.panelChatHeader.Size = new System.Drawing.Size(830, 70);
            this.panelChatHeader.TabIndex = 0;
            this.panelChatHeader.Visible = false;
            // 
            // picChatAvatar
            // 
            this.picChatAvatar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.picChatAvatar.Location = new System.Drawing.Point(15, 8);
            this.picChatAvatar.Name = "picChatAvatar";
            this.picChatAvatar.Size = new System.Drawing.Size(50, 50);
            this.picChatAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picChatAvatar.TabIndex = 0;
            this.picChatAvatar.TabStop = false;
            // 
            // lblChatName
            // 
            this.lblChatName.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblChatName.ForeColor = System.Drawing.Color.White;
            this.lblChatName.Location = new System.Drawing.Point(75, 8);
            this.lblChatName.Name = "lblChatName";
            this.lblChatName.Size = new System.Drawing.Size(564, 25);
            this.lblChatName.TabIndex = 1;
            this.lblChatName.Text = "Выберите чат";
            // 
            // btnViewParticipants
            // 
            this.btnViewParticipants.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnViewParticipants.BackColor = System.Drawing.Color.Transparent;
            this.btnViewParticipants.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnViewParticipants.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewParticipants.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnViewParticipants.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnViewParticipants.Location = new System.Drawing.Point(679, -183);
            this.btnViewParticipants.Name = "btnViewParticipants";
            this.btnViewParticipants.Size = new System.Drawing.Size(100, 30);
            this.btnViewParticipants.TabIndex = 5;
            this.btnViewParticipants.Text = "👥 Участники";
            this.btnViewParticipants.UseVisualStyleBackColor = false;
            // 
            // lblLastSeen
            // 
            this.lblLastSeen.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLastSeen.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(200)))));
            this.lblLastSeen.Location = new System.Drawing.Point(75, 45);
            this.lblLastSeen.Name = "lblLastSeen";
            this.lblLastSeen.Size = new System.Drawing.Size(300, 20);
            this.lblLastSeen.TabIndex = 6;
            // 
            // btnHideChat
            // 
            this.btnHideChat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHideChat.BackColor = System.Drawing.Color.Transparent;
            this.btnHideChat.FlatAppearance.BorderSize = 0;
            this.btnHideChat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHideChat.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnHideChat.ForeColor = System.Drawing.Color.White;
            this.btnHideChat.Location = new System.Drawing.Point(795, 3);
            this.btnHideChat.Name = "btnHideChat";
            this.btnHideChat.Size = new System.Drawing.Size(30, 30);
            this.btnHideChat.TabIndex = 7;
            this.btnHideChat.Text = "✖";
            this.btnHideChat.UseVisualStyleBackColor = false;
            this.btnHideChat.Click += new System.EventHandler(this.HideCurrentChat);
            // 
            // lstMessages
            // 
            this.lstMessages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.lstMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstMessages.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lstMessages.ForeColor = System.Drawing.Color.White;
            this.lstMessages.IntegralHeight = false;
            this.lstMessages.Location = new System.Drawing.Point(10, 79);
            this.lstMessages.Name = "lstMessages";
            this.lstMessages.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstMessages.Size = new System.Drawing.Size(830, 406);
            this.lstMessages.TabIndex = 1;
            this.lstMessages.Visible = false;
            // 
            // panelMessageInput
            // 
            this.panelMessageInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.panelMessageInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMessageInput.Controls.Add(this.lblTyping);
            this.panelMessageInput.Controls.Add(this.txtMessage);
            this.panelMessageInput.Controls.Add(this.btnSend);
            this.panelMessageInput.Controls.Add(this.lblEditingHint);
            this.panelMessageInput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelMessageInput.Location = new System.Drawing.Point(10, 485);
            this.panelMessageInput.Name = "panelMessageInput";
            this.panelMessageInput.Size = new System.Drawing.Size(830, 80);
            this.panelMessageInput.TabIndex = 2;
            this.panelMessageInput.Visible = false;
            // 
            // lblTyping
            // 
            this.lblTyping.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.lblTyping.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.lblTyping.Location = new System.Drawing.Point(15, 0);
            this.lblTyping.Name = "lblTyping";
            this.lblTyping.Size = new System.Drawing.Size(300, 15);
            this.lblTyping.TabIndex = 7;
            this.lblTyping.Visible = false;
            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(80)))));
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMessage.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtMessage.ForeColor = System.Drawing.Color.White;
            this.txtMessage.Location = new System.Drawing.Point(15, 16);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(655, 50);
            this.txtMessage.TabIndex = 0;
            // 
            // btnSend
            // 
            this.btnSend.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnSend.FlatAppearance.BorderSize = 0;
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSend.ForeColor = System.Drawing.Color.Black;
            this.btnSend.Location = new System.Drawing.Point(679, 16);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(140, 50);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "Отправить";
            this.btnSend.UseVisualStyleBackColor = false;
            // 
            // lblEditingHint
            // 
            this.lblEditingHint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(180)))));
            this.lblEditingHint.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblEditingHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.lblEditingHint.Location = new System.Drawing.Point(15, -2);
            this.lblEditingHint.Name = "lblEditingHint";
            this.lblEditingHint.Size = new System.Drawing.Size(655, 19);
            this.lblEditingHint.TabIndex = 2;
            this.lblEditingHint.Text = "Редактирование сообщения (Esc – отмена)";
            this.lblEditingHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblEditingHint.Visible = false;
            // 
            // lblWelcomePlaceholder
            // 
            this.lblWelcomePlaceholder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.lblWelcomePlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWelcomePlaceholder.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblWelcomePlaceholder.ForeColor = System.Drawing.Color.White;
            this.lblWelcomePlaceholder.Location = new System.Drawing.Point(10, 10);
            this.lblWelcomePlaceholder.Name = "lblWelcomePlaceholder";
            this.lblWelcomePlaceholder.Size = new System.Drawing.Size(830, 555);
            this.lblWelcomePlaceholder.TabIndex = 3;
            this.lblWelcomePlaceholder.Text = "Выберите чат, чтобы начать общение.\n\n• Создавайте новые чаты\n• Участвуйте в групп" +
    "овых обсуждениях\n• Просматривайте историю";
            this.lblWelcomePlaceholder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelStatusBar
            // 
            this.panelStatusBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panelStatusBar.Controls.Add(this.lblConnectionStatus);
            this.panelStatusBar.Controls.Add(this.lblServerInfo);
            this.panelStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelStatusBar.Location = new System.Drawing.Point(0, 675);
            this.panelStatusBar.Name = "panelStatusBar";
            this.panelStatusBar.Size = new System.Drawing.Size(1200, 25);
            this.panelStatusBar.TabIndex = 3;
            // 
            // lblConnectionStatus
            // 
            this.lblConnectionStatus.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblConnectionStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblConnectionStatus.Location = new System.Drawing.Point(10, 5);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new System.Drawing.Size(200, 15);
            this.lblConnectionStatus.TabIndex = 0;
            this.lblConnectionStatus.Text = "● Не подключено";
            // 
            // lblServerInfo
            // 
            this.lblServerInfo.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblServerInfo.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblServerInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(200)))));
            this.lblServerInfo.Location = new System.Drawing.Point(980, 5);
            this.lblServerInfo.Name = "lblServerInfo";
            this.lblServerInfo.Size = new System.Drawing.Size(200, 15);
            this.lblServerInfo.TabIndex = 1;
            this.lblServerInfo.Text = "Сервер: не подключен";
            this.lblServerInfo.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelStatusBar);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "MainForm";
            this.Text = "Корпоративный мессенджер";
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picUserAvatar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picOnlineIndicator)).EndInit();
            this.cmsSettingsMenu.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.panelLeftHeader.ResumeLayout(false);
            this.panelLeftHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSearch)).EndInit();
            this.panelLeftFooter.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.panelChatHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picChatAvatar)).EndInit();
            this.panelMessageInput.ResumeLayout(false);
            this.panelMessageInput.PerformLayout();
            this.panelStatusBar.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}