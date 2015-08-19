using System.Windows.Forms;


namespace Avicomp.Installer
{
    partial class SQLOSBBConnectionControl2
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
            this.comboDBName = new System.Windows.Forms.ComboBox();
            this.textPassword = new System.Windows.Forms.TextBox();
            this.captionPassword = new System.Windows.Forms.Label();
            this.textUserName = new System.Windows.Forms.TextBox();
            this.captionUserName = new System.Windows.Forms.Label();
            this.panelSqlAuthentication = new System.Windows.Forms.Panel();
            this.checkCreateBackup = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textServerName = new System.Windows.Forms.TextBox();
            this.textBackupPath = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.panelSqlAuthentication.SuspendLayout();
            this.SuspendLayout();
            // 
            // captionInstanceName
            // 
            this.captionInstanceName.AutoSize = true;
            this.captionInstanceName.BackColor = System.Drawing.SystemColors.Control;
            this.captionInstanceName.Location = new System.Drawing.Point(4, 109);
            this.captionInstanceName.Name = "captionInstanceName";
            this.captionInstanceName.Size = new System.Drawing.Size(51, 13);
            this.captionInstanceName.TabIndex = 0;
            this.captionInstanceName.Text = "Имя БД:";
            // 
            // comboDBName
            // 
            this.comboDBName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboDBName.DisplayMember = "dbname";
            this.comboDBName.FormattingEnabled = true;
            this.comboDBName.Location = new System.Drawing.Point(117, 104);
            this.comboDBName.Name = "comboDBName";
            this.comboDBName.Size = new System.Drawing.Size(213, 21);
            this.comboDBName.TabIndex = 1;
            this.comboDBName.ValueMember = "dbname";
            this.comboDBName.DropDown += new System.EventHandler(this.comboDBName_DropDown);
            this.comboDBName.SelectedIndexChanged += new System.EventHandler(this.comboInstanceName_SelectedIndexChanged);
            // 
            // textPassword
            // 
            this.textPassword.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textPassword.Location = new System.Drawing.Point(0, 33);
            this.textPassword.Name = "textPassword";
            this.textPassword.PasswordChar = '*';
            this.textPassword.Size = new System.Drawing.Size(213, 20);
            this.textPassword.TabIndex = 8;
            this.textPassword.TextChanged += new System.EventHandler(this.textPassword_TextChanged);
            // 
            // captionPassword
            // 
            this.captionPassword.AutoSize = true;
            this.captionPassword.Location = new System.Drawing.Point(4, 78);
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
            this.textUserName.Size = new System.Drawing.Size(213, 20);
            this.textUserName.TabIndex = 6;
            this.textUserName.TextChanged += new System.EventHandler(this.textUserName_TextChanged);
            // 
            // captionUserName
            // 
            this.captionUserName.AutoSize = true;
            this.captionUserName.Location = new System.Drawing.Point(4, 47);
            this.captionUserName.Name = "captionUserName";
            this.captionUserName.Size = new System.Drawing.Size(106, 13);
            this.captionUserName.TabIndex = 0;
            this.captionUserName.Text = "Имя пользователя:";
            // 
            // panelSqlAuthentication
            // 
            this.panelSqlAuthentication.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSqlAuthentication.Controls.Add(this.textPassword);
            this.panelSqlAuthentication.Controls.Add(this.textUserName);
            this.panelSqlAuthentication.Location = new System.Drawing.Point(117, 41);
            this.panelSqlAuthentication.Name = "panelSqlAuthentication";
            this.panelSqlAuthentication.Size = new System.Drawing.Size(213, 53);
            this.panelSqlAuthentication.TabIndex = 10;
            // 
            // checkCreateBackup
            // 
            this.checkCreateBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkCreateBackup.AutoSize = true;
            this.checkCreateBackup.Location = new System.Drawing.Point(26, 138);
            this.checkCreateBackup.Name = "checkCreateBackup";
            this.checkCreateBackup.Size = new System.Drawing.Size(284, 17);
            this.checkCreateBackup.TabIndex = 11;
            this.checkCreateBackup.Text = "Создать резервную копию БД перед обновлением";
            this.checkCreateBackup.UseVisualStyleBackColor = true;
            this.checkCreateBackup.CheckedChanged += new System.EventHandler(this.checkCreateBackup_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Имя сервера";
            // 
            // textServerName
            // 
            this.textServerName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textServerName.Location = new System.Drawing.Point(117, 11);
            this.textServerName.Name = "textServerName";
            this.textServerName.Size = new System.Drawing.Size(213, 20);
            this.textServerName.TabIndex = 13;
            this.textServerName.TextChanged += new System.EventHandler(this.textServerName_TextChanged);
            // 
            // textBackupPath
            // 
            this.textBackupPath.Enabled = false;
            this.textBackupPath.Location = new System.Drawing.Point(27, 165);
            this.textBackupPath.Name = "textBackupPath";
            this.textBackupPath.Size = new System.Drawing.Size(223, 20);
            this.textBackupPath.TabIndex = 14;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.Enabled = false;
            this.buttonBrowse.Location = new System.Drawing.Point(255, 163);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowse.TabIndex = 15;
            this.buttonBrowse.Text = "Файл...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // SQLOSBBConnectionControl2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.textBackupPath);
            this.Controls.Add(this.textServerName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkCreateBackup);
            this.Controls.Add(this.panelSqlAuthentication);
            this.Controls.Add(this.captionPassword);
            this.Controls.Add(this.captionUserName);
            this.Controls.Add(this.comboDBName);
            this.Controls.Add(this.captionInstanceName);
            this.Name = "SQLOSBBConnectionControl2";
            this.Size = new System.Drawing.Size(340, 195);
            this.Resize += new System.EventHandler(this.SQLConnectionControl_Resize);
            this.panelSqlAuthentication.ResumeLayout(false);
            this.panelSqlAuthentication.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label captionInstanceName;
        private ComboBox comboDBName;
        private TextBox textPassword;
        private Label captionPassword;
        private TextBox textUserName;
        private Label captionUserName;
        private Panel panelSqlAuthentication;
        private CheckBox checkCreateBackup;
        private Label label1;
        private TextBox textServerName;
        private TextBox textBackupPath;
        private Button buttonBrowse;
    }
}
