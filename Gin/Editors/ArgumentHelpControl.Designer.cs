namespace Gin.Editors
{
    partial class ArgumentHelpControl
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
            this.listContextNames = new System.Windows.Forms.ListBox();
            this.splitContainerBottom = new System.Windows.Forms.SplitContainer();
            this.checkShowAll = new System.Windows.Forms.CheckBox();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainerTop = new System.Windows.Forms.SplitContainer();
            this.labelHelpArgumentName = new System.Windows.Forms.Label();
            this.labelHelpArgumentDescription = new System.Windows.Forms.Label();
            this.splitContainerBottom.Panel1.SuspendLayout();
            this.splitContainerBottom.Panel2.SuspendLayout();
            this.splitContainerBottom.SuspendLayout();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.splitContainerTop.Panel1.SuspendLayout();
            this.splitContainerTop.Panel2.SuspendLayout();
            this.splitContainerTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // listContextNames
            // 
            this.listContextNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listContextNames.FormattingEnabled = true;
            this.listContextNames.Location = new System.Drawing.Point(0, 0);
            this.listContextNames.Name = "listContextNames";
            this.listContextNames.ScrollAlwaysVisible = true;
            this.listContextNames.Size = new System.Drawing.Size(322, 99);
            this.listContextNames.TabIndex = 3;
            this.listContextNames.Visible = false;
            this.listContextNames.DoubleClick += new System.EventHandler(this.listContextNames_DoubleClick);
            // 
            // splitContainerBottom
            // 
            this.splitContainerBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerBottom.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerBottom.IsSplitterFixed = true;
            this.splitContainerBottom.Location = new System.Drawing.Point(0, 0);
            this.splitContainerBottom.Name = "splitContainerBottom";
            this.splitContainerBottom.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerBottom.Panel1
            // 
            this.splitContainerBottom.Panel1.Controls.Add(this.listContextNames);
            // 
            // splitContainerBottom.Panel2
            // 
            this.splitContainerBottom.Panel2.Controls.Add(this.checkShowAll);
            this.splitContainerBottom.Size = new System.Drawing.Size(322, 128);
            this.splitContainerBottom.SplitterDistance = 99;
            this.splitContainerBottom.TabIndex = 4;
            // 
            // checkShowAll
            // 
            this.checkShowAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkShowAll.AutoSize = true;
            this.checkShowAll.Checked = true;
            this.checkShowAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkShowAll.Enabled = false;
            this.checkShowAll.Location = new System.Drawing.Point(222, 4);
            this.checkShowAll.Name = "checkShowAll";
            this.checkShowAll.Size = new System.Drawing.Size(96, 17);
            this.checkShowAll.TabIndex = 0;
            this.checkShowAll.Text = "Показать все";
            this.checkShowAll.UseVisualStyleBackColor = true;
            this.checkShowAll.Visible = false;
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerTop);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.splitContainerBottom);
            this.splitContainerMain.Size = new System.Drawing.Size(322, 208);
            this.splitContainerMain.SplitterDistance = 76;
            this.splitContainerMain.TabIndex = 5;
            // 
            // splitContainerTop
            // 
            this.splitContainerTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerTop.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerTop.Location = new System.Drawing.Point(0, 0);
            this.splitContainerTop.Name = "splitContainerTop";
            this.splitContainerTop.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerTop.Panel1
            // 
            this.splitContainerTop.Panel1.Controls.Add(this.labelHelpArgumentName);
            // 
            // splitContainerTop.Panel2
            // 
            this.splitContainerTop.Panel2.Controls.Add(this.labelHelpArgumentDescription);
            this.splitContainerTop.Size = new System.Drawing.Size(322, 76);
            this.splitContainerTop.SplitterDistance = 25;
            this.splitContainerTop.TabIndex = 4;
            // 
            // labelHelpArgumentName
            // 
            this.labelHelpArgumentName.AutoEllipsis = true;
            this.labelHelpArgumentName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHelpArgumentName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelHelpArgumentName.Location = new System.Drawing.Point(0, 0);
            this.labelHelpArgumentName.Name = "labelHelpArgumentName";
            this.labelHelpArgumentName.Size = new System.Drawing.Size(322, 25);
            this.labelHelpArgumentName.TabIndex = 2;
            this.labelHelpArgumentName.Text = "[]";
            this.labelHelpArgumentName.Click += new System.EventHandler(this.labelHelpArgumentName_Click);
            // 
            // labelHelpArgumentDescription
            // 
            this.labelHelpArgumentDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHelpArgumentDescription.Location = new System.Drawing.Point(0, 0);
            this.labelHelpArgumentDescription.Name = "labelHelpArgumentDescription";
            this.labelHelpArgumentDescription.Size = new System.Drawing.Size(322, 47);
            this.labelHelpArgumentDescription.TabIndex = 3;
            this.labelHelpArgumentDescription.Text = "[]";
            // 
            // ArgumentHelpControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerMain);
            this.Name = "ArgumentHelpControl";
            this.Size = new System.Drawing.Size(322, 208);
            this.splitContainerBottom.Panel1.ResumeLayout(false);
            this.splitContainerBottom.Panel2.ResumeLayout(false);
            this.splitContainerBottom.Panel2.PerformLayout();
            this.splitContainerBottom.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerTop.Panel1.ResumeLayout(false);
            this.splitContainerTop.Panel2.ResumeLayout(false);
            this.splitContainerTop.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listContextNames;
        private System.Windows.Forms.SplitContainer splitContainerBottom;
        private System.Windows.Forms.CheckBox checkShowAll;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Label labelHelpArgumentDescription;
        private System.Windows.Forms.Label labelHelpArgumentName;
        private System.Windows.Forms.SplitContainer splitContainerTop;
    }
}
