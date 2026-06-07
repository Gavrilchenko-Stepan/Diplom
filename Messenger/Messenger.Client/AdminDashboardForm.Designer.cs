using System.Drawing;
using System.Windows.Forms;

namespace Messenger.Client
{
    partial class AdminDashboardForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelMain;
        private TabControl tabControl;
        private TabPage tabUsers;
        private TabPage tabHistory;
        private TabPage tabChats;

        // Вкладка Пользователи
        private Panel panelUserMain;
        private Label lblTitleUsers;
        private TextBox txtSearch;
        private Button btnSearch;
        private TreeView tvUsers;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefreshUsers;

        // Вкладка История
        private Panel panelHistoryMain;
        private Label lblTitleHistory;
        private Label lblChat;
        private ComboBox cmbChat;
        private Label lblPeriod;
        private DateTimePicker dtpStart;
        private DateTimePicker dtpEnd;
        private Button btnLoadHistory;
        private ListBox lstHistoryMessages;
        private Button btnExport;
        private Button btnClearHistory;

        // Вкладка Чаты
        private Panel panelChatsMain;
        private Label lblTitleChats;
        private TreeView tvChats;
        private Button btnDeleteChat;
        private Button btnRefreshChats;
        private Button btnViewParticipants;
        private Button btnJoinChat;
        private Button btnLeaveChat;

        // Отделы
        private TabPage tabDepartments;
        private Panel panelDepartmentsMain;
        private DataGridView dgvDepartments;
        private Button btnAddDepartment;
        private Button btnEditDepartment;
        private Button btnDeleteDepartment;
        private Button btnRefreshDepartments;
        private Label lblTitleDepartments;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelMain = new System.Windows.Forms.Panel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabUsers = new System.Windows.Forms.TabPage();
            this.panelUserMain = new System.Windows.Forms.Panel();
            this.lblTitleUsers = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.tvUsers = new System.Windows.Forms.TreeView();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRefreshUsers = new System.Windows.Forms.Button();
            this.tabHistory = new System.Windows.Forms.TabPage();
            this.panelHistoryMain = new System.Windows.Forms.Panel();
            this.lblTitleHistory = new System.Windows.Forms.Label();
            this.lblChat = new System.Windows.Forms.Label();
            this.cmbChat = new System.Windows.Forms.ComboBox();
            this.lblPeriod = new System.Windows.Forms.Label();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.btnLoadHistory = new System.Windows.Forms.Button();
            this.lstHistoryMessages = new System.Windows.Forms.ListBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnClearHistory = new System.Windows.Forms.Button();
            this.tabChats = new System.Windows.Forms.TabPage();
            this.panelChatsMain = new System.Windows.Forms.Panel();
            this.lblTitleChats = new System.Windows.Forms.Label();
            this.tvChats = new System.Windows.Forms.TreeView();
            this.btnDeleteChat = new System.Windows.Forms.Button();
            this.btnRefreshChats = new System.Windows.Forms.Button();
            this.btnViewParticipants = new System.Windows.Forms.Button();
            this.btnJoinChat = new System.Windows.Forms.Button();
            this.btnLeaveChat = new System.Windows.Forms.Button();
            this.tabDepartments = new System.Windows.Forms.TabPage();
            this.panelDepartmentsMain = new System.Windows.Forms.Panel();
            this.lblTitleDepartments = new System.Windows.Forms.Label();
            this.dgvDepartments = new System.Windows.Forms.DataGridView();
            this.btnAddDepartment = new System.Windows.Forms.Button();
            this.btnEditDepartment = new System.Windows.Forms.Button();
            this.btnDeleteDepartment = new System.Windows.Forms.Button();
            this.btnRefreshDepartments = new System.Windows.Forms.Button();
            this.panelMain.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabUsers.SuspendLayout();
            this.panelUserMain.SuspendLayout();
            this.tabHistory.SuspendLayout();
            this.panelHistoryMain.SuspendLayout();
            this.tabChats.SuspendLayout();
            this.panelChatsMain.SuspendLayout();
            this.tabDepartments.SuspendLayout();
            this.panelDepartmentsMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDepartments)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.panelMain.Controls.Add(this.tabControl);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(20);
            this.panelMain.Size = new System.Drawing.Size(900, 650);
            this.panelMain.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabUsers);
            this.tabControl.Controls.Add(this.tabHistory);
            this.tabControl.Controls.Add(this.tabChats);
            this.tabControl.Controls.Add(this.tabDepartments);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(20, 20);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(860, 610);
            this.tabControl.TabIndex = 0;
            // 
            // tabUsers
            // 
            this.tabUsers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.tabUsers.Controls.Add(this.panelUserMain);
            this.tabUsers.Location = new System.Drawing.Point(4, 24);
            this.tabUsers.Name = "tabUsers";
            this.tabUsers.Size = new System.Drawing.Size(852, 582);
            this.tabUsers.TabIndex = 0;
            this.tabUsers.Text = "Пользователи";
            // 
            // panelUserMain
            // 
            this.panelUserMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.panelUserMain.Controls.Add(this.lblTitleUsers);
            this.panelUserMain.Controls.Add(this.txtSearch);
            this.panelUserMain.Controls.Add(this.btnSearch);
            this.panelUserMain.Controls.Add(this.tvUsers);
            this.panelUserMain.Controls.Add(this.btnAdd);
            this.panelUserMain.Controls.Add(this.btnEdit);
            this.panelUserMain.Controls.Add(this.btnDelete);
            this.panelUserMain.Controls.Add(this.btnRefreshUsers);
            this.panelUserMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelUserMain.Location = new System.Drawing.Point(0, 0);
            this.panelUserMain.Name = "panelUserMain";
            this.panelUserMain.Padding = new System.Windows.Forms.Padding(20);
            this.panelUserMain.Size = new System.Drawing.Size(852, 582);
            this.panelUserMain.TabIndex = 0;
            // 
            // lblTitleUsers
            // 
            this.lblTitleUsers.AutoSize = true;
            this.lblTitleUsers.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitleUsers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.lblTitleUsers.Location = new System.Drawing.Point(20, 20);
            this.lblTitleUsers.Name = "lblTitleUsers";
            this.lblTitleUsers.Size = new System.Drawing.Size(327, 30);
            this.lblTitleUsers.TabIndex = 0;
            this.lblTitleUsers.Text = "Управление пользователями";
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(80)))));
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSearch.ForeColor = System.Drawing.Color.White;
            this.txtSearch.Location = new System.Drawing.Point(20, 65);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(200, 23);
            this.txtSearch.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.Black;
            this.btnSearch.Location = new System.Drawing.Point(230, 62);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 28);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Поиск";
            this.btnSearch.UseVisualStyleBackColor = false;
            // 
            // tvUsers
            // 
            this.tvUsers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(80)))));
            this.tvUsers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvUsers.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tvUsers.ForeColor = System.Drawing.Color.White;
            this.tvUsers.Location = new System.Drawing.Point(20, 105);
            this.tvUsers.Name = "tvUsers";
            this.tvUsers.Size = new System.Drawing.Size(550, 430);
            this.tvUsers.TabIndex = 3;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.Color.Black;
            this.btnAdd.Location = new System.Drawing.Point(600, 105);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(160, 40);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "➕ Добавить";
            this.btnAdd.UseVisualStyleBackColor = false;
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.Transparent;
            this.btnEdit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnEdit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnEdit.Location = new System.Drawing.Point(600, 160);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(160, 40);
            this.btnEdit.TabIndex = 5;
            this.btnEdit.Text = "✏ Редактировать";
            this.btnEdit.UseVisualStyleBackColor = false;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.Transparent;
            this.btnDelete.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnDelete.Location = new System.Drawing.Point(600, 215);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(160, 40);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "🗑 Удалить";
            this.btnDelete.UseVisualStyleBackColor = false;
            // 
            // btnRefreshUsers
            // 
            this.btnRefreshUsers.BackColor = System.Drawing.Color.Transparent;
            this.btnRefreshUsers.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnRefreshUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshUsers.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnRefreshUsers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnRefreshUsers.Location = new System.Drawing.Point(600, 495);
            this.btnRefreshUsers.Name = "btnRefreshUsers";
            this.btnRefreshUsers.Size = new System.Drawing.Size(160, 40);
            this.btnRefreshUsers.TabIndex = 7;
            this.btnRefreshUsers.Text = "🔄 Обновить";
            this.btnRefreshUsers.UseVisualStyleBackColor = false;
            // 
            // tabHistory
            // 
            this.tabHistory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.tabHistory.Controls.Add(this.panelHistoryMain);
            this.tabHistory.Location = new System.Drawing.Point(4, 24);
            this.tabHistory.Name = "tabHistory";
            this.tabHistory.Size = new System.Drawing.Size(852, 582);
            this.tabHistory.TabIndex = 1;
            this.tabHistory.Text = "История сообщений";
            // 
            // panelHistoryMain
            // 
            this.panelHistoryMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.panelHistoryMain.Controls.Add(this.lblTitleHistory);
            this.panelHistoryMain.Controls.Add(this.lblChat);
            this.panelHistoryMain.Controls.Add(this.cmbChat);
            this.panelHistoryMain.Controls.Add(this.lblPeriod);
            this.panelHistoryMain.Controls.Add(this.dtpStart);
            this.panelHistoryMain.Controls.Add(this.dtpEnd);
            this.panelHistoryMain.Controls.Add(this.btnLoadHistory);
            this.panelHistoryMain.Controls.Add(this.lstHistoryMessages);
            this.panelHistoryMain.Controls.Add(this.btnExport);
            this.panelHistoryMain.Controls.Add(this.btnClearHistory);
            this.panelHistoryMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHistoryMain.Location = new System.Drawing.Point(0, 0);
            this.panelHistoryMain.Name = "panelHistoryMain";
            this.panelHistoryMain.Padding = new System.Windows.Forms.Padding(20);
            this.panelHistoryMain.Size = new System.Drawing.Size(852, 582);
            this.panelHistoryMain.TabIndex = 0;
            // 
            // lblTitleHistory
            // 
            this.lblTitleHistory.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitleHistory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.lblTitleHistory.Location = new System.Drawing.Point(20, 20);
            this.lblTitleHistory.Name = "lblTitleHistory";
            this.lblTitleHistory.Size = new System.Drawing.Size(812, 30);
            this.lblTitleHistory.TabIndex = 0;
            this.lblTitleHistory.Text = "История сообщений";
            // 
            // lblChat
            // 
            this.lblChat.AutoSize = true;
            this.lblChat.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblChat.ForeColor = System.Drawing.Color.White;
            this.lblChat.Location = new System.Drawing.Point(20, 70);
            this.lblChat.Name = "lblChat";
            this.lblChat.Size = new System.Drawing.Size(30, 15);
            this.lblChat.TabIndex = 1;
            this.lblChat.Text = "Чат:";
            // 
            // cmbChat
            // 
            this.cmbChat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(80)))));
            this.cmbChat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbChat.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbChat.ForeColor = System.Drawing.Color.White;
            this.cmbChat.Location = new System.Drawing.Point(60, 67);
            this.cmbChat.Name = "cmbChat";
            this.cmbChat.Size = new System.Drawing.Size(250, 23);
            this.cmbChat.TabIndex = 2;
            // 
            // lblPeriod
            // 
            this.lblPeriod.AutoSize = true;
            this.lblPeriod.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPeriod.ForeColor = System.Drawing.Color.White;
            this.lblPeriod.Location = new System.Drawing.Point(330, 70);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.Size = new System.Drawing.Size(55, 15);
            this.lblPeriod.TabIndex = 3;
            this.lblPeriod.Text = "Период:";
            // 
            // dtpStart
            // 
            this.dtpStart.CalendarForeColor = System.Drawing.Color.White;
            this.dtpStart.CalendarMonthBackground = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(80)))));
            this.dtpStart.CalendarTitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.dtpStart.CalendarTitleForeColor = System.Drawing.Color.White;
            this.dtpStart.CalendarTrailingForeColor = System.Drawing.Color.Gray;
            this.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStart.Location = new System.Drawing.Point(390, 67);
            this.dtpStart.Name = "dtpStart";
            this.dtpStart.Size = new System.Drawing.Size(110, 23);
            this.dtpStart.TabIndex = 4;
            // 
            // dtpEnd
            // 
            this.dtpEnd.CalendarForeColor = System.Drawing.Color.White;
            this.dtpEnd.CalendarMonthBackground = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(80)))));
            this.dtpEnd.CalendarTitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.dtpEnd.CalendarTitleForeColor = System.Drawing.Color.White;
            this.dtpEnd.CalendarTrailingForeColor = System.Drawing.Color.Gray;
            this.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEnd.Location = new System.Drawing.Point(510, 67);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.Size = new System.Drawing.Size(110, 23);
            this.dtpEnd.TabIndex = 5;
            // 
            // btnLoadHistory
            // 
            this.btnLoadHistory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnLoadHistory.FlatAppearance.BorderSize = 0;
            this.btnLoadHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadHistory.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnLoadHistory.ForeColor = System.Drawing.Color.Black;
            this.btnLoadHistory.Location = new System.Drawing.Point(650, 65);
            this.btnLoadHistory.Name = "btnLoadHistory";
            this.btnLoadHistory.Size = new System.Drawing.Size(110, 28);
            this.btnLoadHistory.TabIndex = 7;
            this.btnLoadHistory.Text = "Загрузить";
            this.btnLoadHistory.UseVisualStyleBackColor = false;
            // 
            // lstHistoryMessages
            // 
            this.lstHistoryMessages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.lstHistoryMessages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstHistoryMessages.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lstHistoryMessages.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lstHistoryMessages.ForeColor = System.Drawing.Color.White;
            this.lstHistoryMessages.IntegralHeight = false;
            this.lstHistoryMessages.Location = new System.Drawing.Point(20, 110);
            this.lstHistoryMessages.Name = "lstHistoryMessages";
            this.lstHistoryMessages.Size = new System.Drawing.Size(812, 390);
            this.lstHistoryMessages.TabIndex = 8;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.Transparent;
            this.btnExport.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnExport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnExport.Location = new System.Drawing.Point(20, 520);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(120, 35);
            this.btnExport.TabIndex = 9;
            this.btnExport.Text = "📁 Экспорт";
            this.btnExport.UseVisualStyleBackColor = false;
            // 
            // btnClearHistory
            // 
            this.btnClearHistory.BackColor = System.Drawing.Color.Transparent;
            this.btnClearHistory.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnClearHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearHistory.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnClearHistory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnClearHistory.Location = new System.Drawing.Point(150, 520);
            this.btnClearHistory.Name = "btnClearHistory";
            this.btnClearHistory.Size = new System.Drawing.Size(120, 35);
            this.btnClearHistory.TabIndex = 10;
            this.btnClearHistory.Text = "🗑 Очистить";
            this.btnClearHistory.UseVisualStyleBackColor = false;
            // 
            // tabChats
            // 
            this.tabChats.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.tabChats.Controls.Add(this.panelChatsMain);
            this.tabChats.Location = new System.Drawing.Point(4, 24);
            this.tabChats.Name = "tabChats";
            this.tabChats.Size = new System.Drawing.Size(852, 582);
            this.tabChats.TabIndex = 2;
            this.tabChats.Text = "Управление чатами";
            // 
            // panelChatsMain
            // 
            this.panelChatsMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.panelChatsMain.Controls.Add(this.lblTitleChats);
            this.panelChatsMain.Controls.Add(this.tvChats);
            this.panelChatsMain.Controls.Add(this.btnDeleteChat);
            this.panelChatsMain.Controls.Add(this.btnRefreshChats);
            this.panelChatsMain.Controls.Add(this.btnViewParticipants);
            this.panelChatsMain.Controls.Add(this.btnJoinChat);
            this.panelChatsMain.Controls.Add(this.btnLeaveChat);
            this.panelChatsMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelChatsMain.Location = new System.Drawing.Point(0, 0);
            this.panelChatsMain.Name = "panelChatsMain";
            this.panelChatsMain.Padding = new System.Windows.Forms.Padding(20);
            this.panelChatsMain.Size = new System.Drawing.Size(852, 582);
            this.panelChatsMain.TabIndex = 0;
            // 
            // lblTitleChats
            // 
            this.lblTitleChats.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitleChats.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.lblTitleChats.Location = new System.Drawing.Point(20, 20);
            this.lblTitleChats.Name = "lblTitleChats";
            this.lblTitleChats.Size = new System.Drawing.Size(812, 30);
            this.lblTitleChats.TabIndex = 0;
            this.lblTitleChats.Text = "Список всех чатов";
            // 
            // tvChats
            // 
            this.tvChats.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(80)))));
            this.tvChats.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvChats.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tvChats.ForeColor = System.Drawing.Color.White;
            this.tvChats.FullRowSelect = true;
            this.tvChats.HideSelection = false;
            this.tvChats.Location = new System.Drawing.Point(20, 65);
            this.tvChats.Name = "tvChats";
            this.tvChats.Size = new System.Drawing.Size(550, 460);
            this.tvChats.TabIndex = 1;
            // 
            // btnDeleteChat
            // 
            this.btnDeleteChat.BackColor = System.Drawing.Color.Transparent;
            this.btnDeleteChat.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnDeleteChat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteChat.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnDeleteChat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnDeleteChat.Location = new System.Drawing.Point(600, 120);
            this.btnDeleteChat.Name = "btnDeleteChat";
            this.btnDeleteChat.Size = new System.Drawing.Size(160, 40);
            this.btnDeleteChat.TabIndex = 2;
            this.btnDeleteChat.Text = "🗑 Удалить чат";
            this.btnDeleteChat.UseVisualStyleBackColor = false;
            // 
            // btnRefreshChats
            // 
            this.btnRefreshChats.BackColor = System.Drawing.Color.Transparent;
            this.btnRefreshChats.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnRefreshChats.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshChats.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnRefreshChats.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnRefreshChats.Location = new System.Drawing.Point(600, 485);
            this.btnRefreshChats.Name = "btnRefreshChats";
            this.btnRefreshChats.Size = new System.Drawing.Size(160, 40);
            this.btnRefreshChats.TabIndex = 3;
            this.btnRefreshChats.Text = "🔄 Обновить";
            this.btnRefreshChats.UseVisualStyleBackColor = false;
            // 
            // btnViewParticipants
            // 
            this.btnViewParticipants.BackColor = System.Drawing.Color.Transparent;
            this.btnViewParticipants.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnViewParticipants.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewParticipants.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnViewParticipants.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnViewParticipants.Location = new System.Drawing.Point(600, 65);
            this.btnViewParticipants.Name = "btnViewParticipants";
            this.btnViewParticipants.Size = new System.Drawing.Size(160, 40);
            this.btnViewParticipants.TabIndex = 4;
            this.btnViewParticipants.Text = "👥 Участники";
            this.btnViewParticipants.UseVisualStyleBackColor = false;
            // 
            // btnJoinChat
            // 
            this.btnJoinChat.BackColor = System.Drawing.Color.Transparent;
            this.btnJoinChat.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnJoinChat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJoinChat.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnJoinChat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnJoinChat.Location = new System.Drawing.Point(600, 294);
            this.btnJoinChat.Name = "btnJoinChat";
            this.btnJoinChat.Size = new System.Drawing.Size(160, 40);
            this.btnJoinChat.TabIndex = 5;
            this.btnJoinChat.Text = "🔌 Присоединиться";
            this.btnJoinChat.UseVisualStyleBackColor = false;
            // 
            // btnLeaveChat
            // 
            this.btnLeaveChat.BackColor = System.Drawing.Color.Transparent;
            this.btnLeaveChat.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnLeaveChat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeaveChat.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnLeaveChat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnLeaveChat.Location = new System.Drawing.Point(600, 353);
            this.btnLeaveChat.Name = "btnLeaveChat";
            this.btnLeaveChat.Size = new System.Drawing.Size(160, 40);
            this.btnLeaveChat.TabIndex = 6;
            this.btnLeaveChat.Text = "🚪 Покинуть чат";
            this.btnLeaveChat.UseVisualStyleBackColor = false;
            // 
            // tabDepartments
            // 
            this.tabDepartments.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.tabDepartments.Controls.Add(this.panelDepartmentsMain);
            this.tabDepartments.Location = new System.Drawing.Point(4, 24);
            this.tabDepartments.Name = "tabDepartments";
            this.tabDepartments.Size = new System.Drawing.Size(852, 582);
            this.tabDepartments.TabIndex = 3;
            this.tabDepartments.Text = "Отделы";
            // 
            // panelDepartmentsMain
            // 
            this.panelDepartmentsMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.panelDepartmentsMain.Controls.Add(this.lblTitleDepartments);
            this.panelDepartmentsMain.Controls.Add(this.dgvDepartments);
            this.panelDepartmentsMain.Controls.Add(this.btnAddDepartment);
            this.panelDepartmentsMain.Controls.Add(this.btnEditDepartment);
            this.panelDepartmentsMain.Controls.Add(this.btnDeleteDepartment);
            this.panelDepartmentsMain.Controls.Add(this.btnRefreshDepartments);
            this.panelDepartmentsMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDepartmentsMain.Location = new System.Drawing.Point(0, 0);
            this.panelDepartmentsMain.Name = "panelDepartmentsMain";
            this.panelDepartmentsMain.Padding = new System.Windows.Forms.Padding(20);
            this.panelDepartmentsMain.Size = new System.Drawing.Size(852, 582);
            this.panelDepartmentsMain.TabIndex = 0;
            // 
            // lblTitleDepartments
            // 
            this.lblTitleDepartments.AutoSize = true;
            this.lblTitleDepartments.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitleDepartments.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.lblTitleDepartments.Location = new System.Drawing.Point(23, 20);
            this.lblTitleDepartments.Name = "lblTitleDepartments";
            this.lblTitleDepartments.Size = new System.Drawing.Size(253, 30);
            this.lblTitleDepartments.TabIndex = 0;
            this.lblTitleDepartments.Text = "Управление отделами";
            // 
            // dgvDepartments
            // 
            this.dgvDepartments.AllowUserToAddRows = false;
            this.dgvDepartments.AllowUserToDeleteRows = false;
            this.dgvDepartments.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDepartments.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(80)))));
            this.dgvDepartments.Location = new System.Drawing.Point(20, 65);
            this.dgvDepartments.Name = "dgvDepartments";
            this.dgvDepartments.ReadOnly = true;
            this.dgvDepartments.RowHeadersVisible = false;
            this.dgvDepartments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDepartments.Size = new System.Drawing.Size(550, 465);
            this.dgvDepartments.TabIndex = 1;
            // 
            // btnAddDepartment
            // 
            this.btnAddDepartment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnAddDepartment.FlatAppearance.BorderSize = 0;
            this.btnAddDepartment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddDepartment.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAddDepartment.ForeColor = System.Drawing.Color.Black;
            this.btnAddDepartment.Location = new System.Drawing.Point(600, 65);
            this.btnAddDepartment.Name = "btnAddDepartment";
            this.btnAddDepartment.Size = new System.Drawing.Size(160, 40);
            this.btnAddDepartment.TabIndex = 2;
            this.btnAddDepartment.Text = "➕ Добавить";
            this.btnAddDepartment.UseVisualStyleBackColor = false;
            // 
            // btnEditDepartment
            // 
            this.btnEditDepartment.BackColor = System.Drawing.Color.Transparent;
            this.btnEditDepartment.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnEditDepartment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditDepartment.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnEditDepartment.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnEditDepartment.Location = new System.Drawing.Point(600, 120);
            this.btnEditDepartment.Name = "btnEditDepartment";
            this.btnEditDepartment.Size = new System.Drawing.Size(160, 40);
            this.btnEditDepartment.TabIndex = 3;
            this.btnEditDepartment.Text = "✏ Редактировать";
            this.btnEditDepartment.UseVisualStyleBackColor = false;
            // 
            // btnDeleteDepartment
            // 
            this.btnDeleteDepartment.BackColor = System.Drawing.Color.Transparent;
            this.btnDeleteDepartment.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnDeleteDepartment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteDepartment.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnDeleteDepartment.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnDeleteDepartment.Location = new System.Drawing.Point(600, 177);
            this.btnDeleteDepartment.Name = "btnDeleteDepartment";
            this.btnDeleteDepartment.Size = new System.Drawing.Size(160, 40);
            this.btnDeleteDepartment.TabIndex = 4;
            this.btnDeleteDepartment.Text = "🗑 Удалить";
            this.btnDeleteDepartment.UseVisualStyleBackColor = false;
            // 
            // btnRefreshDepartments
            // 
            this.btnRefreshDepartments.BackColor = System.Drawing.Color.Transparent;
            this.btnRefreshDepartments.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnRefreshDepartments.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshDepartments.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnRefreshDepartments.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnRefreshDepartments.Location = new System.Drawing.Point(600, 490);
            this.btnRefreshDepartments.Name = "btnRefreshDepartments";
            this.btnRefreshDepartments.Size = new System.Drawing.Size(160, 40);
            this.btnRefreshDepartments.TabIndex = 5;
            this.btnRefreshDepartments.Text = "🔄 Обновить";
            this.btnRefreshDepartments.UseVisualStyleBackColor = false;
            // 
            // AdminDashboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.ClientSize = new System.Drawing.Size(900, 650);
            this.Controls.Add(this.panelMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdminDashboardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Панель администратора";
            this.panelMain.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabUsers.ResumeLayout(false);
            this.panelUserMain.ResumeLayout(false);
            this.panelUserMain.PerformLayout();
            this.tabHistory.ResumeLayout(false);
            this.panelHistoryMain.ResumeLayout(false);
            this.panelHistoryMain.PerformLayout();
            this.tabChats.ResumeLayout(false);
            this.panelChatsMain.ResumeLayout(false);
            this.tabDepartments.ResumeLayout(false);
            this.panelDepartmentsMain.ResumeLayout(false);
            this.panelDepartmentsMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDepartments)).EndInit();
            this.ResumeLayout(false);

        }
    }
}