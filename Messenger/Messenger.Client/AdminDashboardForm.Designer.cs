using System.Drawing;
using System.Windows.Forms;

namespace Messenger.Client
{
    partial class AdminDashboardForm
    {
        private System.ComponentModel.IContainer components = null;

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
        private CheckBox chkAllChats;
        private Button btnLoadHistory;
        private ListBox lstHistoryMessages;   // <-- вместо DataGridView
        private Button btnExport;
        private Button btnClearHistory;

        // Вкладка Чаты
        private Panel panelChatsMain;
        private Label lblTitleChats;
        private DataGridView dgvChats;
        private Button btnDeleteChat;
        private Button btnRefreshChats;
        private Button btnViewParticipants;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl = new TabControl();
            this.tabUsers = new TabPage();
            this.panelUserMain = new Panel();
            this.lblTitleUsers = new Label();
            this.txtSearch = new TextBox();
            this.btnSearch = new Button();
            this.tvUsers = new TreeView();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.btnRefreshUsers = new Button();
            this.tabHistory = new TabPage();
            this.panelHistoryMain = new Panel();
            this.lblTitleHistory = new Label();
            this.lblChat = new Label();
            this.cmbChat = new ComboBox();
            this.lblPeriod = new Label();
            this.dtpStart = new DateTimePicker();
            this.dtpEnd = new DateTimePicker();
            this.chkAllChats = new CheckBox();
            this.btnLoadHistory = new Button();
            this.lstHistoryMessages = new ListBox();
            this.btnExport = new Button();
            this.btnClearHistory = new Button();
            this.tabChats = new TabPage();
            this.panelChatsMain = new Panel();
            this.lblTitleChats = new Label();
            this.dgvChats = new DataGridView();
            this.btnDeleteChat = new Button();
            this.btnRefreshChats = new Button();
            this.btnViewParticipants = new Button();

            this.tabControl.SuspendLayout();
            this.tabUsers.SuspendLayout();
            this.panelUserMain.SuspendLayout();
            this.tabHistory.SuspendLayout();
            this.panelHistoryMain.SuspendLayout();
            this.tabChats.SuspendLayout();
            this.panelChatsMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChats)).BeginInit();
            this.SuspendLayout();

            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabUsers);
            this.tabControl.Controls.Add(this.tabHistory);
            this.tabControl.Controls.Add(this.tabChats);
            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.Location = new Point(0, 0);
            this.tabControl.Size = new Size(1000, 700);
            this.tabControl.TabIndex = 0;

            // 
            // tabUsers
            // 
            this.tabUsers.Controls.Add(this.panelUserMain);
            this.tabUsers.Text = "Пользователи";

            // 
            // panelUserMain
            // 
            this.panelUserMain.BackColor = Color.FromArgb(45, 45, 58);
            this.panelUserMain.Controls.Add(this.lblTitleUsers);
            this.panelUserMain.Controls.Add(this.txtSearch);
            this.panelUserMain.Controls.Add(this.btnSearch);
            this.panelUserMain.Controls.Add(this.tvUsers);
            this.panelUserMain.Controls.Add(this.btnAdd);
            this.panelUserMain.Controls.Add(this.btnEdit);
            this.panelUserMain.Controls.Add(this.btnDelete);
            this.panelUserMain.Controls.Add(this.btnRefreshUsers);
            this.panelUserMain.Dock = DockStyle.Fill;
            this.panelUserMain.Padding = new Padding(10);
            this.panelUserMain.Size = new Size(1000, 700);

            this.lblTitleUsers.AutoSize = true;
            this.lblTitleUsers.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblTitleUsers.ForeColor = Color.FromArgb(0, 229, 255);
            this.lblTitleUsers.Location = new Point(10, 10);
            this.lblTitleUsers.Text = "Управление пользователями";

            this.txtSearch.BackColor = Color.FromArgb(60, 60, 80);
            this.txtSearch.BorderStyle = BorderStyle.FixedSingle;
            this.txtSearch.ForeColor = Color.White;
            this.txtSearch.Location = new Point(10, 50);
            this.txtSearch.Size = new Size(200, 23);

            this.btnSearch.BackColor = Color.FromArgb(0, 229, 255);
            this.btnSearch.FlatStyle = FlatStyle.Flat;
            this.btnSearch.ForeColor = Color.Black;
            this.btnSearch.Location = new Point(215, 49);
            this.btnSearch.Size = new Size(75, 27);
            this.btnSearch.Text = "Поиск";
            this.btnSearch.UseVisualStyleBackColor = false;

            this.tvUsers.BackColor = Color.FromArgb(60, 60, 80);
            this.tvUsers.BorderStyle = BorderStyle.FixedSingle;
            this.tvUsers.ForeColor = Color.White;
            this.tvUsers.Location = new Point(10, 85);
            this.tvUsers.Size = new Size(580, 500);

            this.btnAdd.BackColor = Color.FromArgb(0, 229, 255);
            this.btnAdd.FlatStyle = FlatStyle.Flat;
            this.btnAdd.ForeColor = Color.Black;
            this.btnAdd.Location = new Point(610, 85);
            this.btnAdd.Size = new Size(170, 40);
            this.btnAdd.Text = "➕ Добавить";

            this.btnEdit.BackColor = Color.Transparent;
            this.btnEdit.FlatAppearance.BorderColor = Color.FromArgb(0, 229, 255);
            this.btnEdit.FlatStyle = FlatStyle.Flat;
            this.btnEdit.ForeColor = Color.FromArgb(0, 229, 255);
            this.btnEdit.Location = new Point(610, 135);
            this.btnEdit.Size = new Size(170, 40);
            this.btnEdit.Text = "✏️ Редактировать";

            this.btnDelete.BackColor = Color.Transparent;
            this.btnDelete.FlatAppearance.BorderColor = Color.FromArgb(255, 80, 80);
            this.btnDelete.FlatStyle = FlatStyle.Flat;
            this.btnDelete.ForeColor = Color.FromArgb(255, 80, 80);
            this.btnDelete.Location = new Point(610, 185);
            this.btnDelete.Size = new Size(170, 40);
            this.btnDelete.Text = "🗑️ Удалить";

            this.btnRefreshUsers.BackColor = Color.Transparent;
            this.btnRefreshUsers.FlatAppearance.BorderColor = Color.FromArgb(180, 180, 200);
            this.btnRefreshUsers.FlatStyle = FlatStyle.Flat;
            this.btnRefreshUsers.ForeColor = Color.FromArgb(180, 180, 200);
            this.btnRefreshUsers.Location = new Point(610, 500);
            this.btnRefreshUsers.Size = new Size(170, 40);
            this.btnRefreshUsers.Text = "🔄 Обновить";

            // 
            // tabHistory
            // 
            this.tabHistory.Controls.Add(this.panelHistoryMain);
            this.tabHistory.Text = "История сообщений";

            // 
            // panelHistoryMain
            // 
            this.panelHistoryMain.BackColor = Color.FromArgb(45, 45, 58);
            this.panelHistoryMain.Controls.Add(this.lblTitleHistory);
            this.panelHistoryMain.Controls.Add(this.lblChat);
            this.panelHistoryMain.Controls.Add(this.cmbChat);
            this.panelHistoryMain.Controls.Add(this.lblPeriod);
            this.panelHistoryMain.Controls.Add(this.dtpStart);
            this.panelHistoryMain.Controls.Add(this.dtpEnd);
            this.panelHistoryMain.Controls.Add(this.chkAllChats);
            this.panelHistoryMain.Controls.Add(this.btnLoadHistory);
            this.panelHistoryMain.Controls.Add(this.lstHistoryMessages);
            this.panelHistoryMain.Controls.Add(this.btnExport);
            this.panelHistoryMain.Controls.Add(this.btnClearHistory);
            this.panelHistoryMain.Dock = DockStyle.Fill;
            this.panelHistoryMain.Padding = new Padding(10);
            this.panelHistoryMain.Size = new Size(1000, 700);

            this.lblTitleHistory.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblTitleHistory.ForeColor = Color.FromArgb(0, 229, 255);
            this.lblTitleHistory.Location = new Point(10, 10);
            this.lblTitleHistory.Size = new Size(960, 30);
            this.lblTitleHistory.Text = "История сообщений";

            this.lblChat.AutoSize = true;
            this.lblChat.ForeColor = Color.White;
            this.lblChat.Location = new Point(10, 60);
            this.lblChat.Text = "Чат:";

            this.cmbChat.BackColor = Color.FromArgb(60, 60, 80);
            this.cmbChat.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbChat.FlatStyle = FlatStyle.Flat;
            this.cmbChat.ForeColor = Color.White;
            this.cmbChat.Location = new Point(60, 57);
            this.cmbChat.Size = new Size(250, 23);

            this.lblPeriod.AutoSize = true;
            this.lblPeriod.ForeColor = Color.White;
            this.lblPeriod.Location = new Point(330, 60);
            this.lblPeriod.Text = "Период:";

            this.dtpStart.Format = DateTimePickerFormat.Short;
            this.dtpStart.Location = new Point(390, 57);
            this.dtpStart.Size = new Size(120, 23);

            this.dtpEnd.Format = DateTimePickerFormat.Short;
            this.dtpEnd.Location = new Point(520, 57);
            this.dtpEnd.Size = new Size(120, 23);

            this.chkAllChats.AutoSize = true;
            this.chkAllChats.ForeColor = Color.White;
            this.chkAllChats.Location = new Point(660, 59);
            this.chkAllChats.Text = "Все чаты";

            this.btnLoadHistory.BackColor = Color.FromArgb(0, 229, 255);
            this.btnLoadHistory.FlatStyle = FlatStyle.Flat;
            this.btnLoadHistory.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnLoadHistory.ForeColor = Color.Black;
            this.btnLoadHistory.Location = new Point(760, 55);
            this.btnLoadHistory.Size = new Size(120, 30);
            this.btnLoadHistory.Text = "Загрузить";

            // 
            // lstHistoryMessages
            // 
            this.lstHistoryMessages.BackColor = Color.FromArgb(30, 30, 46);
            this.lstHistoryMessages.BorderStyle = BorderStyle.None;
            this.lstHistoryMessages.DrawMode = DrawMode.OwnerDrawVariable;
            this.lstHistoryMessages.ForeColor = Color.White;
            this.lstHistoryMessages.IntegralHeight = false;
            this.lstHistoryMessages.Location = new Point(10, 100);
            this.lstHistoryMessages.Name = "lstHistoryMessages";
            this.lstHistoryMessages.Size = new Size(960, 450);
            this.lstHistoryMessages.TabIndex = 8;
            this.lstHistoryMessages.MeasureItem += new MeasureItemEventHandler(this.LstHistory_MeasureItem);
            this.lstHistoryMessages.DrawItem += new DrawItemEventHandler(this.LstHistory_DrawItem);

            // 
            // btnExport
            // 
            this.btnExport.BackColor = Color.Transparent;
            this.btnExport.FlatAppearance.BorderColor = Color.FromArgb(0, 229, 255);
            this.btnExport.FlatStyle = FlatStyle.Flat;
            this.btnExport.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnExport.ForeColor = Color.FromArgb(0, 229, 255);
            this.btnExport.Location = new Point(10, 570);
            this.btnExport.Size = new Size(120, 40);
            this.btnExport.Text = "📁 Экспорт";

            // 
            // btnClearHistory
            // 
            this.btnClearHistory.BackColor = Color.Transparent;
            this.btnClearHistory.FlatAppearance.BorderColor = Color.FromArgb(255, 80, 80);
            this.btnClearHistory.FlatStyle = FlatStyle.Flat;
            this.btnClearHistory.ForeColor = Color.FromArgb(255, 80, 80);
            this.btnClearHistory.Location = new Point(140, 570);
            this.btnClearHistory.Size = new Size(120, 40);
            this.btnClearHistory.Text = "🗑️ Очистить";

            // 
            // tabChats
            // 
            this.tabChats.Controls.Add(this.panelChatsMain);
            this.tabChats.Text = "Управление чатами";

            // 
            // panelChatsMain
            // 
            this.panelChatsMain.BackColor = Color.FromArgb(45, 45, 58);
            this.panelChatsMain.Controls.Add(this.lblTitleChats);
            this.panelChatsMain.Controls.Add(this.dgvChats);
            this.panelChatsMain.Controls.Add(this.btnDeleteChat);
            this.panelChatsMain.Controls.Add(this.btnRefreshChats);
            this.panelChatsMain.Controls.Add(this.btnViewParticipants);
            this.panelChatsMain.Dock = DockStyle.Fill;
            this.panelChatsMain.Padding = new Padding(10);
            this.panelChatsMain.Size = new Size(1000, 700);

            this.lblTitleChats.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblTitleChats.ForeColor = Color.FromArgb(0, 229, 255);
            this.lblTitleChats.Location = new Point(10, 10);
            this.lblTitleChats.Size = new Size(960, 30);
            this.lblTitleChats.Text = "Список всех чатов";

            this.dgvChats.BackgroundColor = Color.White;
            this.dgvChats.BorderStyle = BorderStyle.Fixed3D;
            this.dgvChats.Location = new Point(10, 50);
            this.dgvChats.Size = new Size(800, 500);
            this.dgvChats.ReadOnly = true;
            this.dgvChats.RowHeadersVisible = false;
            this.dgvChats.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvChats.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            this.btnDeleteChat.BackColor = Color.Transparent;
            this.btnDeleteChat.FlatAppearance.BorderColor = Color.FromArgb(255, 80, 80);
            this.btnDeleteChat.FlatStyle = FlatStyle.Flat;
            this.btnDeleteChat.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnDeleteChat.ForeColor = Color.FromArgb(255, 80, 80);
            this.btnDeleteChat.Location = new Point(830, 100);
            this.btnDeleteChat.Size = new Size(150, 40);
            this.btnDeleteChat.Text = "🗑️ Удалить чат";

            this.btnRefreshChats.BackColor = Color.Transparent;
            this.btnRefreshChats.FlatAppearance.BorderColor = Color.FromArgb(0, 229, 255);
            this.btnRefreshChats.FlatStyle = FlatStyle.Flat;
            this.btnRefreshChats.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnRefreshChats.ForeColor = Color.FromArgb(0, 229, 255);
            this.btnRefreshChats.Location = new Point(830, 50);
            this.btnRefreshChats.Size = new Size(150, 40);
            this.btnRefreshChats.Text = "🔄 Обновить";

            this.btnViewParticipants.BackColor = Color.Transparent;
            this.btnViewParticipants.FlatAppearance.BorderColor = Color.FromArgb(0, 229, 255);
            this.btnViewParticipants.FlatStyle = FlatStyle.Flat;
            this.btnViewParticipants.Font = new Font("Segoe UI", 9F);
            this.btnViewParticipants.ForeColor = Color.FromArgb(0, 229, 255);
            this.btnViewParticipants.Location = new Point(830, 150);
            this.btnViewParticipants.Size = new Size(150, 40);
            this.btnViewParticipants.Text = "👥 Участники";

            // 
            // AdminDashboardForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(30, 30, 46);
            this.ClientSize = new Size(1000, 700);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "AdminDashboardForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Панель администратора";

            this.tabControl.ResumeLayout(false);
            this.tabUsers.ResumeLayout(false);
            this.panelUserMain.ResumeLayout(false);
            this.panelUserMain.PerformLayout();
            this.tabHistory.ResumeLayout(false);
            this.panelHistoryMain.ResumeLayout(false);
            this.panelHistoryMain.PerformLayout();
            this.tabChats.ResumeLayout(false);
            this.panelChatsMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChats)).EndInit();
            this.ResumeLayout(false);
        }
    }
}