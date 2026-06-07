using System.Windows.Forms;

namespace Messenger.Client
{
    partial class NewChatForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelMain;
        private Panel panelHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private TabControl tabControl;
        private TabPage tabPrivate;
        private TabPage tabGroup;
        private TreeView tvPrivateUsers;
        private TreeView tvGroupUsers;
        private Label lblChatName;
        private TextBox txtChatName;
        private Button btnCreate;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPrivate = new System.Windows.Forms.TabPage();
            this.tvPrivateUsers = new System.Windows.Forms.TreeView();
            this.tabGroup = new System.Windows.Forms.TabPage();
            this.lblChatName = new System.Windows.Forms.Label();
            this.txtChatName = new System.Windows.Forms.TextBox();
            this.tvGroupUsers = new System.Windows.Forms.TreeView();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panelMain.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPrivate.SuspendLayout();
            this.tabGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.panelMain.Controls.Add(this.btnCancel);
            this.panelMain.Controls.Add(this.panelHeader);
            this.panelMain.Controls.Add(this.btnCreate);
            this.panelMain.Controls.Add(this.tabControl);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(24);
            this.panelMain.Size = new System.Drawing.Size(580, 620);
            this.panelMain.TabIndex = 0;
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Controls.Add(this.lblSubtitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(24, 24);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(532, 80);
            this.panelHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(532, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Создать новый чат";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(200)))));
            this.lblSubtitle.Location = new System.Drawing.Point(0, 48);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(532, 25);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Выберите тип чата и участников";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPrivate);
            this.tabControl.Controls.Add(this.tabGroup);
            this.tabControl.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tabControl.Location = new System.Drawing.Point(24, 120);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(532, 420);
            this.tabControl.TabIndex = 1;
            // 
            // tabPrivate
            // 
            this.tabPrivate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.tabPrivate.Controls.Add(this.tvPrivateUsers);
            this.tabPrivate.Location = new System.Drawing.Point(4, 26);
            this.tabPrivate.Name = "tabPrivate";
            this.tabPrivate.Padding = new System.Windows.Forms.Padding(15);
            this.tabPrivate.Size = new System.Drawing.Size(524, 390);
            this.tabPrivate.TabIndex = 1;
            this.tabPrivate.Text = "Личный";
            // 
            // tvPrivateUsers
            // 
            this.tvPrivateUsers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(80)))));
            this.tvPrivateUsers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvPrivateUsers.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tvPrivateUsers.ForeColor = System.Drawing.Color.White;
            this.tvPrivateUsers.Location = new System.Drawing.Point(15, 15);
            this.tvPrivateUsers.Name = "tvPrivateUsers";
            this.tvPrivateUsers.Size = new System.Drawing.Size(494, 360);
            this.tvPrivateUsers.TabIndex = 0;
            // 
            // tabGroup
            // 
            this.tabGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.tabGroup.Controls.Add(this.lblChatName);
            this.tabGroup.Controls.Add(this.txtChatName);
            this.tabGroup.Controls.Add(this.tvGroupUsers);
            this.tabGroup.Location = new System.Drawing.Point(4, 26);
            this.tabGroup.Name = "tabGroup";
            this.tabGroup.Padding = new System.Windows.Forms.Padding(15);
            this.tabGroup.Size = new System.Drawing.Size(524, 390);
            this.tabGroup.TabIndex = 2;
            this.tabGroup.Text = "Групповой";
            // 
            // lblChatName
            // 
            this.lblChatName.AutoSize = true;
            this.lblChatName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblChatName.ForeColor = System.Drawing.Color.White;
            this.lblChatName.Location = new System.Drawing.Point(15, 18);
            this.lblChatName.Name = "lblChatName";
            this.lblChatName.Size = new System.Drawing.Size(93, 15);
            this.lblChatName.TabIndex = 2;
            this.lblChatName.Text = "Название чата:";
            // 
            // txtChatName
            // 
            this.txtChatName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(80)))));
            this.txtChatName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtChatName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtChatName.ForeColor = System.Drawing.Color.White;
            this.txtChatName.Location = new System.Drawing.Point(15, 38);
            this.txtChatName.Name = "txtChatName";
            this.txtChatName.Size = new System.Drawing.Size(494, 25);
            this.txtChatName.TabIndex = 1;
            // 
            // tvGroupUsers
            // 
            this.tvGroupUsers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(80)))));
            this.tvGroupUsers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvGroupUsers.CheckBoxes = true;
            this.tvGroupUsers.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tvGroupUsers.ForeColor = System.Drawing.Color.White;
            this.tvGroupUsers.Location = new System.Drawing.Point(15, 75);
            this.tvGroupUsers.Name = "tvGroupUsers";
            this.tvGroupUsers.Size = new System.Drawing.Size(494, 300);
            this.tvGroupUsers.TabIndex = 0;
            // 
            // btnCreate
            // 
            this.btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnCreate.FlatAppearance.BorderSize = 0;
            this.btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCreate.ForeColor = System.Drawing.Color.Black;
            this.btnCreate.Location = new System.Drawing.Point(169, 558);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(180, 35);
            this.btnCreate.TabIndex = 1;
            this.btnCreate.Text = "Создать чат";
            this.btnCreate.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnCancel.Location = new System.Drawing.Point(376, 558);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(180, 35);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // NewChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.ClientSize = new System.Drawing.Size(580, 620);
            this.Controls.Add(this.panelMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewChatForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Создать чат";
            this.panelMain.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPrivate.ResumeLayout(false);
            this.tabGroup.ResumeLayout(false);
            this.tabGroup.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}