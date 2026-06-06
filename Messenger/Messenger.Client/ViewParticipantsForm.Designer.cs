namespace Messenger.Client
{
    partial class ViewParticipantsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblChatName;
        private System.Windows.Forms.Label lblParticipantCount;
        private System.Windows.Forms.ListView listViewParticipants;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colPosition;
        private System.Windows.Forms.ColumnHeader colStatus;
        private System.Windows.Forms.Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelMain = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblChatName = new System.Windows.Forms.Label();
            this.lblParticipantCount = new System.Windows.Forms.Label();
            this.listViewParticipants = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPosition = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnClose = new System.Windows.Forms.Button();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(45, 45, 58);
            this.panelMain.Controls.Add(this.lblTitle);
            this.panelMain.Controls.Add(this.lblChatName);
            this.panelMain.Controls.Add(this.lblParticipantCount);
            this.panelMain.Controls.Add(this.listViewParticipants);
            this.panelMain.Controls.Add(this.btnClose);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(20);
            this.panelMain.Size = new System.Drawing.Size(540, 500);
            this.panelMain.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(0, 229, 255);
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(500, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Участники чата";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblChatName
            // 
            this.lblChatName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblChatName.ForeColor = System.Drawing.Color.White;
            this.lblChatName.Location = new System.Drawing.Point(20, 50);
            this.lblChatName.Name = "lblChatName";
            this.lblChatName.Size = new System.Drawing.Size(300, 25);
            this.lblChatName.TabIndex = 1;
            this.lblChatName.Text = "Чат";
            // 
            // lblParticipantCount
            // 
            this.lblParticipantCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblParticipantCount.ForeColor = System.Drawing.Color.FromArgb(180, 180, 200);
            this.lblParticipantCount.Location = new System.Drawing.Point(350, 55);
            this.lblParticipantCount.Name = "lblParticipantCount";
            this.lblParticipantCount.Size = new System.Drawing.Size(170, 18);
            this.lblParticipantCount.TabIndex = 2;
            this.lblParticipantCount.Text = "Участников: 0";
            this.lblParticipantCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // listViewParticipants
            // 
            this.listViewParticipants.BackColor = System.Drawing.Color.White;
            this.listViewParticipants.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewParticipants.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.colName,
                this.colPosition,
                this.colStatus});
            this.listViewParticipants.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.listViewParticipants.ForeColor = System.Drawing.Color.Black;
            this.listViewParticipants.FullRowSelect = true;
            this.listViewParticipants.GridLines = true;
            this.listViewParticipants.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewParticipants.HideSelection = false;
            this.listViewParticipants.Location = new System.Drawing.Point(20, 85);
            this.listViewParticipants.Name = "listViewParticipants";
            this.listViewParticipants.OwnerDraw = true;
            this.listViewParticipants.Size = new System.Drawing.Size(500, 320);
            this.listViewParticipants.TabIndex = 3;
            this.listViewParticipants.UseCompatibleStateImageBehavior = false;
            this.listViewParticipants.View = System.Windows.Forms.View.Details;

            // Явно задаём ширину колонок, чтобы они заняли всю ширину
            this.colName.Width = 180;
            this.colPosition.Width = 200;
            this.colStatus.Width = 100;   // остаток – 500 - 20поле - 180-200-100 = 0, ничего лишнего

            // Подписываем события отрисовки
            this.listViewParticipants.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listViewParticipants_DrawColumnHeader);
            this.listViewParticipants.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listViewParticipants_DrawSubItem);
            // 
            // colName
            // 
            this.colName.Text = "Участник";
            this.colName.Width = 180;
            // 
            // colPosition
            // 
            this.colPosition.Text = "Должность";
            this.colPosition.Width = 200;
            // 
            // colStatus
            // 
            this.colStatus.Text = "Статус";
            this.colStatus.Width = 100;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(0, 229, 255);
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(0, 229, 255);
            this.btnClose.Location = new System.Drawing.Point(410, 430);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(110, 34);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Закрыть";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // ViewParticipantsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 46);
            this.ClientSize = new System.Drawing.Size(540, 500);
            this.Controls.Add(this.panelMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewParticipantsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Участники чата";
            this.panelMain.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}