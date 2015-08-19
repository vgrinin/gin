namespace Gin.Editors
{
    partial class EditComplexValueForm
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
            this.splitMainForm = new System.Windows.Forms.SplitContainer();
            this.splitContainerHelp = new System.Windows.Forms.SplitContainer();
            this.propertiesPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.argumentHelpControl = new Gin.Editors.ArgumentHelpControl();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.splitMainForm.Panel1.SuspendLayout();
            this.splitMainForm.Panel2.SuspendLayout();
            this.splitMainForm.SuspendLayout();
            this.splitContainerHelp.Panel1.SuspendLayout();
            this.splitContainerHelp.Panel2.SuspendLayout();
            this.splitContainerHelp.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitMainForm
            // 
            this.splitMainForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMainForm.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitMainForm.IsSplitterFixed = true;
            this.splitMainForm.Location = new System.Drawing.Point(0, 0);
            this.splitMainForm.Name = "splitMainForm";
            this.splitMainForm.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitMainForm.Panel1
            // 
            this.splitMainForm.Panel1.Controls.Add(this.splitContainerHelp);
            // 
            // splitMainForm.Panel2
            // 
            this.splitMainForm.Panel2.Controls.Add(this.buttonOK);
            this.splitMainForm.Panel2.Controls.Add(this.buttonCancel);
            this.splitMainForm.Size = new System.Drawing.Size(390, 513);
            this.splitMainForm.SplitterDistance = 473;
            this.splitMainForm.TabIndex = 0;
            // 
            // splitContainerHelp
            // 
            this.splitContainerHelp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerHelp.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerHelp.Location = new System.Drawing.Point(0, 0);
            this.splitContainerHelp.Name = "splitContainerHelp";
            this.splitContainerHelp.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerHelp.Panel1
            // 
            this.splitContainerHelp.Panel1.Controls.Add(this.propertiesPanel);
            // 
            // splitContainerHelp.Panel2
            // 
            this.splitContainerHelp.Panel2.Controls.Add(this.argumentHelpControl);
            this.splitContainerHelp.Size = new System.Drawing.Size(390, 473);
            this.splitContainerHelp.SplitterDistance = 249;
            this.splitContainerHelp.TabIndex = 1;
            // 
            // propertiesPanel
            // 
            this.propertiesPanel.AutoScroll = true;
            this.propertiesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesPanel.Location = new System.Drawing.Point(0, 0);
            this.propertiesPanel.Name = "propertiesPanel";
            this.propertiesPanel.Size = new System.Drawing.Size(388, 247);
            this.propertiesPanel.TabIndex = 0;
            this.propertiesPanel.Resize += new System.EventHandler(this.propertiesPanel_Resize);
            // 
            // argumentHelpControl
            // 
            this.argumentHelpControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.argumentHelpControl.Location = new System.Drawing.Point(0, 0);
            this.argumentHelpControl.Name = "argumentHelpControl";
            this.argumentHelpControl.Size = new System.Drawing.Size(388, 218);
            this.argumentHelpControl.TabIndex = 0;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonOK.Location = new System.Drawing.Point(222, 7);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonCancel.Location = new System.Drawing.Point(308, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // EditComplexValueForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 513);
            this.Controls.Add(this.splitMainForm);
            this.Name = "EditComplexValueForm";
            this.Text = "Редактор комплексных значений";
            this.splitMainForm.Panel1.ResumeLayout(false);
            this.splitMainForm.Panel2.ResumeLayout(false);
            this.splitMainForm.ResumeLayout(false);
            this.splitContainerHelp.Panel1.ResumeLayout(false);
            this.splitContainerHelp.Panel2.ResumeLayout(false);
            this.splitContainerHelp.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitMainForm;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.FlowLayoutPanel propertiesPanel;
        private System.Windows.Forms.SplitContainer splitContainerHelp;
        private ArgumentHelpControl argumentHelpControl;
    }
}