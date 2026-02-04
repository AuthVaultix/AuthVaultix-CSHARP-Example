namespace Client
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.userDataField = new System.Windows.Forms.ListBox();
            this.onlineUsersField = new System.Windows.Forms.ListBox();
            this.banBtn = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.fileExtensionField = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.filePathField = new System.Windows.Forms.TextBox();
            this.downloadFileBtn = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.varDataField = new System.Windows.Forms.TextBox();
            this.varField = new System.Windows.Forms.TextBox();
            this.fetchUserVarBtn = new System.Windows.Forms.Button();
            this.setUserVarBtn = new System.Windows.Forms.Button();
            this.globalVariableField = new System.Windows.Forms.TextBox();
            this.fetchGlobalVariableBtn = new System.Windows.Forms.Button();
            this.checkSessionBtn = new System.Windows.Forms.Button();
            this.sendLogDataBtn = new System.Windows.Forms.Button();
            this.logDataField = new System.Windows.Forms.TextBox();
            this.sendMsgBtn = new System.Windows.Forms.Button();
            this.chatMsgField = new System.Windows.Forms.TextBox();
            this.chatroomGrid = new System.Windows.Forms.DataGridView();
            this.Sender = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CheackBlacklistBtn = new System.Windows.Forms.Button();
            this.minBtn = new System.Windows.Forms.Button();
            this.closeBtn = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.chatroomGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // userDataField
            // 
            this.userDataField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(61)))), ((int)(((byte)(79)))));
            this.userDataField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.userDataField.ForeColor = System.Drawing.Color.White;
            this.userDataField.FormattingEnabled = true;
            this.userDataField.Location = new System.Drawing.Point(3, 534);
            this.userDataField.Name = "userDataField";
            this.userDataField.Size = new System.Drawing.Size(433, 106);
            this.userDataField.TabIndex = 63;
            // 
            // onlineUsersField
            // 
            this.onlineUsersField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(61)))), ((int)(((byte)(79)))));
            this.onlineUsersField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.onlineUsersField.ForeColor = System.Drawing.Color.White;
            this.onlineUsersField.FormattingEnabled = true;
            this.onlineUsersField.Items.AddRange(new object[] {
            "Online Users:",
            ""});
            this.onlineUsersField.Location = new System.Drawing.Point(442, 534);
            this.onlineUsersField.Name = "onlineUsersField";
            this.onlineUsersField.Size = new System.Drawing.Size(336, 106);
            this.onlineUsersField.TabIndex = 65;
            // 
            // banBtn
            // 
            this.banBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(100)))), ((int)(((byte)(242)))));
            this.banBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(39)))));
            this.banBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.banBtn.ForeColor = System.Drawing.Color.White;
            this.banBtn.Location = new System.Drawing.Point(4, 424);
            this.banBtn.Name = "banBtn";
            this.banBtn.Size = new System.Drawing.Size(323, 30);
            this.banBtn.TabIndex = 130;
            this.banBtn.Text = "Ban Account";
            this.banBtn.UseVisualStyleBackColor = false;
            this.banBtn.Click += new System.EventHandler(this.banBtn_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label10.Location = new System.Drawing.Point(-1, 187);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(117, 15);
            this.label10.TabIndex = 125;
            this.label10.Text = "File Name/Extension";
            // 
            // fileExtensionField
            // 
            this.fileExtensionField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(61)))), ((int)(((byte)(79)))));
            this.fileExtensionField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fileExtensionField.ForeColor = System.Drawing.Color.White;
            this.fileExtensionField.Location = new System.Drawing.Point(2, 205);
            this.fileExtensionField.Name = "fileExtensionField";
            this.fileExtensionField.Size = new System.Drawing.Size(323, 20);
            this.fileExtensionField.TabIndex = 124;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label9.Location = new System.Drawing.Point(-1, 143);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 15);
            this.label9.TabIndex = 123;
            this.label9.Text = "File Path";
            // 
            // filePathField
            // 
            this.filePathField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(61)))), ((int)(((byte)(79)))));
            this.filePathField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.filePathField.ForeColor = System.Drawing.Color.White;
            this.filePathField.Location = new System.Drawing.Point(2, 161);
            this.filePathField.Name = "filePathField";
            this.filePathField.Size = new System.Drawing.Size(323, 20);
            this.filePathField.TabIndex = 122;
            // 
            // downloadFileBtn
            // 
            this.downloadFileBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(100)))), ((int)(((byte)(242)))));
            this.downloadFileBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(39)))));
            this.downloadFileBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.downloadFileBtn.ForeColor = System.Drawing.Color.White;
            this.downloadFileBtn.Location = new System.Drawing.Point(2, 231);
            this.downloadFileBtn.Name = "downloadFileBtn";
            this.downloadFileBtn.Size = new System.Drawing.Size(323, 30);
            this.downloadFileBtn.TabIndex = 121;
            this.downloadFileBtn.Text = "Download File";
            this.downloadFileBtn.UseVisualStyleBackColor = false;
            this.downloadFileBtn.Click += new System.EventHandler(this.downloadFileBtn_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label6.Location = new System.Drawing.Point(-1, 264);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(130, 15);
            this.label6.TabIndex = 115;
            this.label6.Text = "Global Variable Name:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label5.Location = new System.Drawing.Point(0, 344);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 15);
            this.label5.TabIndex = 114;
            this.label5.Text = "Data To Send In Log:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label3.Location = new System.Drawing.Point(-1, 61);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(210, 15);
            this.label3.TabIndex = 113;
            this.label3.Text = "User Variable Data: (For Setting Only)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label4.Location = new System.Drawing.Point(-1, 17);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 15);
            this.label4.TabIndex = 112;
            this.label4.Text = "User Variable Name:";
            // 
            // varDataField
            // 
            this.varDataField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(61)))), ((int)(((byte)(79)))));
            this.varDataField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.varDataField.ForeColor = System.Drawing.Color.White;
            this.varDataField.Location = new System.Drawing.Point(2, 79);
            this.varDataField.Name = "varDataField";
            this.varDataField.Size = new System.Drawing.Size(323, 20);
            this.varDataField.TabIndex = 111;
            // 
            // varField
            // 
            this.varField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(61)))), ((int)(((byte)(79)))));
            this.varField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.varField.ForeColor = System.Drawing.Color.White;
            this.varField.Location = new System.Drawing.Point(2, 35);
            this.varField.Name = "varField";
            this.varField.Size = new System.Drawing.Size(323, 20);
            this.varField.TabIndex = 110;
            // 
            // fetchUserVarBtn
            // 
            this.fetchUserVarBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(100)))), ((int)(((byte)(242)))));
            this.fetchUserVarBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(39)))));
            this.fetchUserVarBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fetchUserVarBtn.ForeColor = System.Drawing.Color.White;
            this.fetchUserVarBtn.Location = new System.Drawing.Point(172, 105);
            this.fetchUserVarBtn.Name = "fetchUserVarBtn";
            this.fetchUserVarBtn.Size = new System.Drawing.Size(155, 30);
            this.fetchUserVarBtn.TabIndex = 109;
            this.fetchUserVarBtn.Text = "Fetch User Variable";
            this.fetchUserVarBtn.UseVisualStyleBackColor = false;
            this.fetchUserVarBtn.Click += new System.EventHandler(this.fetchUserVarBtn_Click);
            // 
            // setUserVarBtn
            // 
            this.setUserVarBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(100)))), ((int)(((byte)(242)))));
            this.setUserVarBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(39)))));
            this.setUserVarBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.setUserVarBtn.ForeColor = System.Drawing.Color.White;
            this.setUserVarBtn.Location = new System.Drawing.Point(2, 105);
            this.setUserVarBtn.Name = "setUserVarBtn";
            this.setUserVarBtn.Size = new System.Drawing.Size(155, 30);
            this.setUserVarBtn.TabIndex = 108;
            this.setUserVarBtn.Text = "Set User Variable";
            this.setUserVarBtn.UseVisualStyleBackColor = false;
            this.setUserVarBtn.Click += new System.EventHandler(this.setUserVarBtn_Click);
            // 
            // globalVariableField
            // 
            this.globalVariableField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(61)))), ((int)(((byte)(79)))));
            this.globalVariableField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.globalVariableField.ForeColor = System.Drawing.Color.White;
            this.globalVariableField.Location = new System.Drawing.Point(2, 282);
            this.globalVariableField.Name = "globalVariableField";
            this.globalVariableField.Size = new System.Drawing.Size(323, 20);
            this.globalVariableField.TabIndex = 107;
            // 
            // fetchGlobalVariableBtn
            // 
            this.fetchGlobalVariableBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(100)))), ((int)(((byte)(242)))));
            this.fetchGlobalVariableBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(39)))));
            this.fetchGlobalVariableBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fetchGlobalVariableBtn.ForeColor = System.Drawing.Color.White;
            this.fetchGlobalVariableBtn.Location = new System.Drawing.Point(2, 308);
            this.fetchGlobalVariableBtn.Name = "fetchGlobalVariableBtn";
            this.fetchGlobalVariableBtn.Size = new System.Drawing.Size(323, 30);
            this.fetchGlobalVariableBtn.TabIndex = 106;
            this.fetchGlobalVariableBtn.Text = "Fetch Global Variable";
            this.fetchGlobalVariableBtn.UseVisualStyleBackColor = false;
            this.fetchGlobalVariableBtn.Click += new System.EventHandler(this.fetchGlobalVariableBtn_Click);
            // 
            // checkSessionBtn
            // 
            this.checkSessionBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(100)))), ((int)(((byte)(242)))));
            this.checkSessionBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(39)))));
            this.checkSessionBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkSessionBtn.ForeColor = System.Drawing.Color.White;
            this.checkSessionBtn.Location = new System.Drawing.Point(4, 460);
            this.checkSessionBtn.Name = "checkSessionBtn";
            this.checkSessionBtn.Size = new System.Drawing.Size(323, 30);
            this.checkSessionBtn.TabIndex = 105;
            this.checkSessionBtn.Text = "Check Session";
            this.checkSessionBtn.UseVisualStyleBackColor = false;
            this.checkSessionBtn.Click += new System.EventHandler(this.checkSessionBtn_Click);
            // 
            // sendLogDataBtn
            // 
            this.sendLogDataBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(100)))), ((int)(((byte)(242)))));
            this.sendLogDataBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(39)))));
            this.sendLogDataBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sendLogDataBtn.ForeColor = System.Drawing.Color.White;
            this.sendLogDataBtn.Location = new System.Drawing.Point(3, 388);
            this.sendLogDataBtn.Name = "sendLogDataBtn";
            this.sendLogDataBtn.Size = new System.Drawing.Size(323, 30);
            this.sendLogDataBtn.TabIndex = 104;
            this.sendLogDataBtn.Text = "Send Log";
            this.sendLogDataBtn.UseVisualStyleBackColor = false;
            this.sendLogDataBtn.Click += new System.EventHandler(this.sendLogDataBtn_Click);
            // 
            // logDataField
            // 
            this.logDataField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(61)))), ((int)(((byte)(79)))));
            this.logDataField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logDataField.ForeColor = System.Drawing.Color.White;
            this.logDataField.Location = new System.Drawing.Point(3, 362);
            this.logDataField.Name = "logDataField";
            this.logDataField.Size = new System.Drawing.Size(323, 20);
            this.logDataField.TabIndex = 103;
            // 
            // sendMsgBtn
            // 
            this.sendMsgBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(100)))), ((int)(((byte)(242)))));
            this.sendMsgBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(39)))));
            this.sendMsgBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sendMsgBtn.ForeColor = System.Drawing.Color.White;
            this.sendMsgBtn.Location = new System.Drawing.Point(688, 495);
            this.sendMsgBtn.Name = "sendMsgBtn";
            this.sendMsgBtn.Size = new System.Drawing.Size(94, 36);
            this.sendMsgBtn.TabIndex = 133;
            this.sendMsgBtn.Text = "Send";
            this.sendMsgBtn.UseVisualStyleBackColor = false;
            this.sendMsgBtn.Click += new System.EventHandler(this.sendMsgBtn_Click);
            // 
            // chatMsgField
            // 
            this.chatMsgField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(61)))), ((int)(((byte)(79)))));
            this.chatMsgField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chatMsgField.Location = new System.Drawing.Point(330, 505);
            this.chatMsgField.Name = "chatMsgField";
            this.chatMsgField.Size = new System.Drawing.Size(352, 20);
            this.chatMsgField.TabIndex = 132;
            // 
            // chatroomGrid
            // 
            this.chatroomGrid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(61)))), ((int)(((byte)(79)))));
            this.chatroomGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.chatroomGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Sender,
            this.Message,
            this.Time});
            this.chatroomGrid.GridColor = System.Drawing.Color.DodgerBlue;
            this.chatroomGrid.Location = new System.Drawing.Point(330, 37);
            this.chatroomGrid.Name = "chatroomGrid";
            this.chatroomGrid.ReadOnly = true;
            this.chatroomGrid.Size = new System.Drawing.Size(452, 454);
            this.chatroomGrid.TabIndex = 135;
            // 
            // Sender
            // 
            this.Sender.HeaderText = "Sender";
            this.Sender.MinimumWidth = 6;
            this.Sender.Name = "Sender";
            this.Sender.ReadOnly = true;
            // 
            // Message
            // 
            this.Message.HeaderText = "Message";
            this.Message.Name = "Message";
            this.Message.ReadOnly = true;
            this.Message.Width = 200;
            // 
            // Time
            // 
            this.Time.HeaderText = "Time";
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            // 
            // CheackBlacklistBtn
            // 
            this.CheackBlacklistBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(100)))), ((int)(((byte)(242)))));
            this.CheackBlacklistBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(39)))));
            this.CheackBlacklistBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CheackBlacklistBtn.ForeColor = System.Drawing.Color.White;
            this.CheackBlacklistBtn.Location = new System.Drawing.Point(4, 496);
            this.CheackBlacklistBtn.Name = "CheackBlacklistBtn";
            this.CheackBlacklistBtn.Size = new System.Drawing.Size(323, 30);
            this.CheackBlacklistBtn.TabIndex = 136;
            this.CheackBlacklistBtn.Text = "Cheack Blacklist";
            this.CheackBlacklistBtn.UseVisualStyleBackColor = false;
            this.CheackBlacklistBtn.Click += new System.EventHandler(this.CheackBlacklistBtn_Click);
            // 
            // minBtn
            // 
            this.minBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minBtn.ForeColor = System.Drawing.Color.White;
            this.minBtn.Location = new System.Drawing.Point(692, 5);
            this.minBtn.Name = "minBtn";
            this.minBtn.Size = new System.Drawing.Size(43, 23);
            this.minBtn.TabIndex = 138;
            this.minBtn.Text = "-";
            this.minBtn.UseVisualStyleBackColor = true;
            // 
            // closeBtn
            // 
            this.closeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeBtn.ForeColor = System.Drawing.Color.White;
            this.closeBtn.Location = new System.Drawing.Point(741, 5);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(43, 23);
            this.closeBtn.TabIndex = 137;
            this.closeBtn.Text = "X";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(61)))), ((int)(((byte)(79)))));
            this.ClientSize = new System.Drawing.Size(790, 648);
            this.Controls.Add(this.minBtn);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.CheackBlacklistBtn);
            this.Controls.Add(this.sendMsgBtn);
            this.Controls.Add(this.chatMsgField);
            this.Controls.Add(this.chatroomGrid);
            this.Controls.Add(this.banBtn);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.fileExtensionField);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.filePathField);
            this.Controls.Add(this.downloadFileBtn);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.varDataField);
            this.Controls.Add(this.varField);
            this.Controls.Add(this.fetchUserVarBtn);
            this.Controls.Add(this.setUserVarBtn);
            this.Controls.Add(this.globalVariableField);
            this.Controls.Add(this.fetchGlobalVariableBtn);
            this.Controls.Add(this.checkSessionBtn);
            this.Controls.Add(this.sendLogDataBtn);
            this.Controls.Add(this.logDataField);
            this.Controls.Add(this.onlineUsersField);
            this.Controls.Add(this.userDataField);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chatroomGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox userDataField;
        private System.Windows.Forms.ListBox onlineUsersField;
        private System.Windows.Forms.Button banBtn;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox fileExtensionField;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox filePathField;
        private System.Windows.Forms.Button downloadFileBtn;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox varDataField;
        private System.Windows.Forms.TextBox varField;
        private System.Windows.Forms.Button fetchUserVarBtn;
        private System.Windows.Forms.Button setUserVarBtn;
        private System.Windows.Forms.TextBox globalVariableField;
        private System.Windows.Forms.Button fetchGlobalVariableBtn;
        private System.Windows.Forms.Button checkSessionBtn;
        private System.Windows.Forms.Button sendLogDataBtn;
        private System.Windows.Forms.TextBox logDataField;
        private System.Windows.Forms.Button sendMsgBtn;
        private System.Windows.Forms.TextBox chatMsgField;
        private System.Windows.Forms.DataGridView chatroomGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sender;
        private System.Windows.Forms.DataGridViewTextBoxColumn Message;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.Button CheackBlacklistBtn;
        private System.Windows.Forms.Button minBtn;
        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.Timer timer1;
    }
}

