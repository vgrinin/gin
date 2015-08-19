namespace Gin.SQL.Controls
{
    partial class SQLInstanceControl
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
            this.groupInstanceType = new System.Windows.Forms.GroupBox();
            this.panelCreateServer = new System.Windows.Forms.Panel();
            this.browseButton = new System.Windows.Forms.Button();
            this.textSqlDirectory = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textInstanceName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panelChooseServer = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.radioCreateInstance = new System.Windows.Forms.RadioButton();
            this.radioUseExistingInstance = new System.Windows.Forms.RadioButton();
            this.comboInstanceName = new System.Windows.Forms.TextBox();
            this.groupInstanceType.SuspendLayout();
            this.panelCreateServer.SuspendLayout();
            this.panelChooseServer.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupInstanceType
            // 
            this.groupInstanceType.Controls.Add(this.panelCreateServer);
            this.groupInstanceType.Controls.Add(this.panelChooseServer);
            this.groupInstanceType.Controls.Add(this.radioCreateInstance);
            this.groupInstanceType.Controls.Add(this.radioUseExistingInstance);
            this.groupInstanceType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupInstanceType.Location = new System.Drawing.Point(0, 0);
            this.groupInstanceType.Name = "groupInstanceType";
            this.groupInstanceType.Size = new System.Drawing.Size(394, 195);
            this.groupInstanceType.TabIndex = 0;
            this.groupInstanceType.TabStop = false;
            this.groupInstanceType.Text = "Экземпляр SQL-сервера";
            // 
            // panelCreateServer
            // 
            this.panelCreateServer.Controls.Add(this.browseButton);
            this.panelCreateServer.Controls.Add(this.textSqlDirectory);
            this.panelCreateServer.Controls.Add(this.label3);
            this.panelCreateServer.Controls.Add(this.textInstanceName);
            this.panelCreateServer.Controls.Add(this.label2);
            this.panelCreateServer.Enabled = false;
            this.panelCreateServer.Location = new System.Drawing.Point(6, 116);
            this.panelCreateServer.Name = "panelCreateServer";
            this.panelCreateServer.Size = new System.Drawing.Size(374, 66);
            this.panelCreateServer.TabIndex = 2;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(309, 37);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(62, 23);
            this.browseButton.TabIndex = 5;
            this.browseButton.Text = "Выбрать...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // textSqlDirectory
            // 
            this.textSqlDirectory.Location = new System.Drawing.Point(122, 38);
            this.textSqlDirectory.Name = "textSqlDirectory";
            this.textSqlDirectory.Size = new System.Drawing.Size(181, 20);
            this.textSqlDirectory.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Путь к файлам";
            // 
            // textInstanceName
            // 
            this.textInstanceName.Location = new System.Drawing.Point(123, 11);
            this.textInstanceName.Name = "textInstanceName";
            this.textInstanceName.Size = new System.Drawing.Size(248, 20);
            this.textInstanceName.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Экземпляр сервера";
            // 
            // panelChooseServer
            // 
            this.panelChooseServer.Controls.Add(this.comboInstanceName);
            this.panelChooseServer.Controls.Add(this.label1);
            this.panelChooseServer.Location = new System.Drawing.Point(6, 42);
            this.panelChooseServer.Name = "panelChooseServer";
            this.panelChooseServer.Size = new System.Drawing.Size(374, 30);
            this.panelChooseServer.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Экземпляр сервера";
            // 
            // radioCreateInstance
            // 
            this.radioCreateInstance.AutoSize = true;
            this.radioCreateInstance.Location = new System.Drawing.Point(6, 93);
            this.radioCreateInstance.Name = "radioCreateInstance";
            this.radioCreateInstance.Size = new System.Drawing.Size(102, 17);
            this.radioCreateInstance.TabIndex = 1;
            this.radioCreateInstance.Text = "Создать новый";
            this.radioCreateInstance.UseVisualStyleBackColor = true;
            // 
            // radioUseExistingInstance
            // 
            this.radioUseExistingInstance.AutoSize = true;
            this.radioUseExistingInstance.Checked = true;
            this.radioUseExistingInstance.Location = new System.Drawing.Point(6, 19);
            this.radioUseExistingInstance.Name = "radioUseExistingInstance";
            this.radioUseExistingInstance.Size = new System.Drawing.Size(178, 17);
            this.radioUseExistingInstance.TabIndex = 0;
            this.radioUseExistingInstance.TabStop = true;
            this.radioUseExistingInstance.Text = "Использовать существующий";
            this.radioUseExistingInstance.UseVisualStyleBackColor = true;
            this.radioUseExistingInstance.CheckedChanged += new System.EventHandler(this.radioUseExistingInstance_CheckedChanged);
            // 
            // comboInstanceName
            // 
            this.comboInstanceName.Location = new System.Drawing.Point(121, 6);
            this.comboInstanceName.Name = "comboInstanceName";
            this.comboInstanceName.Size = new System.Drawing.Size(250, 20);
            this.comboInstanceName.TabIndex = 1;
            // 
            // SQLInstanceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupInstanceType);
            this.Name = "SQLInstanceControl";
            this.Size = new System.Drawing.Size(394, 195);
            this.groupInstanceType.ResumeLayout(false);
            this.groupInstanceType.PerformLayout();
            this.panelCreateServer.ResumeLayout(false);
            this.panelCreateServer.PerformLayout();
            this.panelChooseServer.ResumeLayout(false);
            this.panelChooseServer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupInstanceType;
        private System.Windows.Forms.RadioButton radioCreateInstance;
        private System.Windows.Forms.RadioButton radioUseExistingInstance;
        private System.Windows.Forms.Panel panelChooseServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelCreateServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textInstanceName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textSqlDirectory;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TextBox comboInstanceName;
    }
}
