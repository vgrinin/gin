using System.Windows.Forms;


namespace Avicomp.Installer
{
    partial class SQLOSBBConnectionControl
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
            this.comboInstanceName = new System.Windows.Forms.ComboBox();
            this.textPassword = new System.Windows.Forms.TextBox();
            this.captionPassword = new System.Windows.Forms.Label();
            this.textUserName = new System.Windows.Forms.TextBox();
            this.captionUserName = new System.Windows.Forms.Label();
            this.panelSqlAuthentication = new System.Windows.Forms.Panel();
            this.checkCreateBackup = new System.Windows.Forms.CheckBox();
            this.panelSqlAuthentication.SuspendLayout();
            this.SuspendLayout();
            // 
            // captionInstanceName
            // 
            this.captionInstanceName.AutoSize = true;
            this.captionInstanceName.BackColor = System.Drawing.SystemColors.Control;
            this.captionInstanceName.Location = new System.Drawing.Point(5, 12);
            this.captionInstanceName.Name = "captionInstanceName";
            this.captionInstanceName.Size = new System.Drawing.Size(161, 13);
            this.captionInstanceName.TabIndex = 0;
            this.captionInstanceName.Text = "Наименование конфигурации:";
            // 
            // comboInstanceName
            // 
            this.comboInstanceName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboInstanceName.DisplayMember = "Name";
            this.comboInstanceName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInstanceName.FormattingEnabled = true;
            this.comboInstanceName.Location = new System.Drawing.Point(170, 8);
            this.comboInstanceName.Name = "comboInstanceName";
            this.comboInstanceName.Size = new System.Drawing.Size(213, 21);
            this.comboInstanceName.TabIndex = 1;
            this.comboInstanceName.ValueMember = "Name";
            this.comboInstanceName.SelectedIndexChanged += new System.EventHandler(this.comboInstanceName_SelectedIndexChanged);
            // 
            // textPassword
            // 
            this.textPassword.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textPassword.Location = new System.Drawing.Point(0, 33);
            this.textPassword.Name = "textPassword";
            this.textPassword.PasswordChar = '*';
            this.textPassword.Size = new System.Drawing.Size(213, 20);
            this.textPassword.TabIndex = 8;
            // 
            // captionPassword
            // 
            this.captionPassword.AutoSize = true;
            this.captionPassword.Location = new System.Drawing.Point(5, 80);
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
            // 
            // captionUserName
            // 
            this.captionUserName.AutoSize = true;
            this.captionUserName.Location = new System.Drawing.Point(5, 49);
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
            this.panelSqlAuthentication.Location = new System.Drawing.Point(170, 45);
            this.panelSqlAuthentication.Name = "panelSqlAuthentication";
            this.panelSqlAuthentication.Size = new System.Drawing.Size(213, 53);
            this.panelSqlAuthentication.TabIndex = 10;
            // 
            // checkCreateBackup
            // 
            this.checkCreateBackup.AutoSize = true;
            this.checkCreateBackup.Location = new System.Drawing.Point(8, 106);
            this.checkCreateBackup.Name = "checkCreateBackup";
            this.checkCreateBackup.Size = new System.Drawing.Size(284, 17);
            this.checkCreateBackup.TabIndex = 11;
            this.checkCreateBackup.Text = "Создать резервную копию БД перед обновлением";
            this.checkCreateBackup.UseVisualStyleBackColor = true;
            // 
            // SQLOSBBConnectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkCreateBackup);
            this.Controls.Add(this.panelSqlAuthentication);
            this.Controls.Add(this.captionPassword);
            this.Controls.Add(this.captionUserName);
            this.Controls.Add(this.comboInstanceName);
            this.Controls.Add(this.captionInstanceName);
            this.Name = "SQLOSBBConnectionControl";
            this.Size = new System.Drawing.Size(392, 130);
            this.Resize += new System.EventHandler(this.SQLConnectionControl_Resize);
            this.panelSqlAuthentication.ResumeLayout(false);
            this.panelSqlAuthentication.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label captionInstanceName;
        private ComboBox comboInstanceName;
        private TextBox textPassword;
        private Label captionPassword;
        private TextBox textUserName;
        private Label captionUserName;
        private Panel panelSqlAuthentication;
        private CheckBox checkCreateBackup;
    }
}
