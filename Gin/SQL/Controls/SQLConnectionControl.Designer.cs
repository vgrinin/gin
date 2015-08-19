using System.Windows.Forms;


namespace Gin.SQL.Controls
{
    partial class SQLConnectionControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.captionInstanceName = new System.Windows.Forms.Label();
            this.captionSqlAuthentication = new System.Windows.Forms.Label();
            this.textPassword = new System.Windows.Forms.TextBox();
            this.captionPassword = new System.Windows.Forms.Label();
            this.textUserName = new System.Windows.Forms.TextBox();
            this.captionUserName = new System.Windows.Forms.Label();
            this.captionDBName = new System.Windows.Forms.Label();
            this.comboDBName = new System.Windows.Forms.ComboBox();
            this.comboAuthentification = new System.Windows.Forms.ComboBox();
            this.panelSqlAuthentication = new System.Windows.Forms.Panel();
            this.comboInstanceName = new System.Windows.Forms.TextBox();
            this.panelSqlAuthentication.SuspendLayout();
            this.SuspendLayout();
            // 
            // captionInstanceName
            // 
            this.captionInstanceName.AutoSize = true;
            this.captionInstanceName.BackColor = System.Drawing.SystemColors.Control;
            this.captionInstanceName.Location = new System.Drawing.Point(4, 12);
            this.captionInstanceName.Name = "captionInstanceName";
            this.captionInstanceName.Size = new System.Drawing.Size(77, 13);
            this.captionInstanceName.TabIndex = 0;
            this.captionInstanceName.Text = "Имя сервера:";
            // 
            // captionSqlAuthentication
            // 
            this.captionSqlAuthentication.AutoSize = true;
            this.captionSqlAuthentication.Location = new System.Drawing.Point(3, 49);
            this.captionSqlAuthentication.Name = "captionSqlAuthentication";
            this.captionSqlAuthentication.Size = new System.Drawing.Size(94, 13);
            this.captionSqlAuthentication.TabIndex = 4;
            this.captionSqlAuthentication.Text = "Аутентификация:";
            // 
            // textPassword
            // 
            this.textPassword.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textPassword.Location = new System.Drawing.Point(0, 33);
            this.textPassword.Name = "textPassword";
            this.textPassword.PasswordChar = '*';
            this.textPassword.Size = new System.Drawing.Size(242, 20);
            this.textPassword.TabIndex = 8;
            this.textPassword.TextChanged += new System.EventHandler(this.textControls_TextChanged);
            // 
            // captionPassword
            // 
            this.captionPassword.AutoSize = true;
            this.captionPassword.Location = new System.Drawing.Point(25, 112);
            this.captionPassword.Name = "captionPassword";
            this.captionPassword.Size = new System.Drawing.Size(48, 13);
            this.captionPassword.TabIndex = 7;
            this.captionPassword.Text = "Пароль:";
            // 
            // textUserName
            // 
            this.textUserName.Dock = System.Windows.Forms.DockStyle.Top;
            this.textUserName.Location = new System.Drawing.Point(0, 0);
            this.textUserName.Name = "textUserName";
            this.textUserName.Size = new System.Drawing.Size(242, 20);
            this.textUserName.TabIndex = 6;
            this.textUserName.TextChanged += new System.EventHandler(this.textControls_TextChanged);
            // 
            // captionUserName
            // 
            this.captionUserName.AutoSize = true;
            this.captionUserName.Location = new System.Drawing.Point(25, 81);
            this.captionUserName.Name = "captionUserName";
            this.captionUserName.Size = new System.Drawing.Size(106, 13);
            this.captionUserName.TabIndex = 0;
            this.captionUserName.Text = "Имя пользователя:";
            // 
            // captionDBName
            // 
            this.captionDBName.AutoSize = true;
            this.captionDBName.Location = new System.Drawing.Point(3, 146);
            this.captionDBName.Name = "captionDBName";
            this.captionDBName.Size = new System.Drawing.Size(72, 13);
            this.captionDBName.TabIndex = 6;
            this.captionDBName.Text = "База данных";
            // 
            // comboDBName
            // 
            this.comboDBName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboDBName.DisplayMember = "Name";
            this.comboDBName.FormattingEnabled = true;
            this.comboDBName.Location = new System.Drawing.Point(126, 143);
            this.comboDBName.Name = "comboDBName";
            this.comboDBName.Size = new System.Drawing.Size(258, 21);
            this.comboDBName.TabIndex = 7;
            this.comboDBName.ValueMember = "Name";
            this.comboDBName.DropDown += new System.EventHandler(this.comboDBName_DropDown);
            // 
            // comboAuthentification
            // 
            this.comboAuthentification.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboAuthentification.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAuthentification.FormattingEnabled = true;
            this.comboAuthentification.Items.AddRange(new object[] {
            "Пользователь SQL Server",
            "Пользователь Windows"});
            this.comboAuthentification.Location = new System.Drawing.Point(126, 45);
            this.comboAuthentification.Name = "comboAuthentification";
            this.comboAuthentification.Size = new System.Drawing.Size(258, 21);
            this.comboAuthentification.TabIndex = 9;
            this.comboAuthentification.SelectedIndexChanged += new System.EventHandler(this.comboAuthentification_SelectedIndexChanged);
            this.comboAuthentification.TextChanged += new System.EventHandler(this.textControls_TextChanged);
            // 
            // panelSqlAuthentication
            // 
            this.panelSqlAuthentication.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSqlAuthentication.Controls.Add(this.textPassword);
            this.panelSqlAuthentication.Controls.Add(this.textUserName);
            this.panelSqlAuthentication.Location = new System.Drawing.Point(142, 77);
            this.panelSqlAuthentication.Name = "panelSqlAuthentication";
            this.panelSqlAuthentication.Size = new System.Drawing.Size(242, 53);
            this.panelSqlAuthentication.TabIndex = 10;
            // 
            // comboInstanceName
            // 
            this.comboInstanceName.Location = new System.Drawing.Point(126, 12);
            this.comboInstanceName.Name = "comboInstanceName";
            this.comboInstanceName.Size = new System.Drawing.Size(258, 20);
            this.comboInstanceName.TabIndex = 11;
            this.comboInstanceName.TextChanged += new System.EventHandler(this.textControls_TextChanged);
            // 
            // SQLConnectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboInstanceName);
            this.Controls.Add(this.panelSqlAuthentication);
            this.Controls.Add(this.captionPassword);
            this.Controls.Add(this.captionUserName);
            this.Controls.Add(this.comboAuthentification);
            this.Controls.Add(this.comboDBName);
            this.Controls.Add(this.captionDBName);
            this.Controls.Add(this.captionSqlAuthentication);
            this.Controls.Add(this.captionInstanceName);
            this.Name = "SQLConnectionControl";
            this.Size = new System.Drawing.Size(392, 171);
            this.Resize += new System.EventHandler(this.SQLConnectionControl_Resize);
            this.panelSqlAuthentication.ResumeLayout(false);
            this.panelSqlAuthentication.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label captionInstanceName;
        private Label captionSqlAuthentication;
        private TextBox textPassword;
        private Label captionPassword;
        private TextBox textUserName;
        private Label captionUserName;
        private Label captionDBName;
        private ComboBox comboDBName;
        private ComboBox comboAuthentification;
        private Panel panelSqlAuthentication;
        private TextBox comboInstanceName;
    }
}
