namespace Gin.Editors
{
    partial class CheckBoxTemplateEditor
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
            this.checkBox = new System.Windows.Forms.CheckBox();
            this.radioStandard = new System.Windows.Forms.RadioButton();
            this.radioTemplate = new System.Windows.Forms.RadioButton();
            this.textTemplate = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // checkBox
            // 
            this.checkBox.AutoSize = true;
            this.checkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkBox.Location = new System.Drawing.Point(3, 7);
            this.checkBox.Name = "checkBox";
            this.checkBox.Size = new System.Drawing.Size(80, 17);
            this.checkBox.TabIndex = 2;
            this.checkBox.Text = "Заголовок";
            this.checkBox.UseVisualStyleBackColor = true;
            this.checkBox.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // radioStandard
            // 
            this.radioStandard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioStandard.AutoSize = true;
            this.radioStandard.Checked = true;
            this.radioStandard.Location = new System.Drawing.Point(281, 11);
            this.radioStandard.Name = "radioStandard";
            this.radioStandard.Size = new System.Drawing.Size(14, 13);
            this.radioStandard.TabIndex = 3;
            this.radioStandard.TabStop = true;
            this.radioStandard.UseVisualStyleBackColor = true;
            this.radioStandard.CheckedChanged += new System.EventHandler(this.radioStandard_CheckedChanged);
            // 
            // radioTemplate
            // 
            this.radioTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioTemplate.AutoSize = true;
            this.radioTemplate.Location = new System.Drawing.Point(281, 34);
            this.radioTemplate.Name = "radioTemplate";
            this.radioTemplate.Size = new System.Drawing.Size(14, 13);
            this.radioTemplate.TabIndex = 4;
            this.radioTemplate.UseVisualStyleBackColor = true;
            // 
            // textTemplate
            // 
            this.textTemplate.Location = new System.Drawing.Point(3, 30);
            this.textTemplate.Name = "textTemplate";
            this.textTemplate.Size = new System.Drawing.Size(272, 20);
            this.textTemplate.TabIndex = 5;
            this.textTemplate.TextChanged += new System.EventHandler(this.textTemplate_TextChanged);
            // 
            // CheckBoxTemplateEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textTemplate);
            this.Controls.Add(this.radioTemplate);
            this.Controls.Add(this.radioStandard);
            this.Controls.Add(this.checkBox);
            this.Name = "CheckBoxTemplateEditor";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(302, 57);
            this.Enter += new System.EventHandler(this.CheckBoxEditor_Enter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox;
        private System.Windows.Forms.RadioButton radioStandard;
        private System.Windows.Forms.RadioButton radioTemplate;
        private System.Windows.Forms.TextBox textTemplate;

    }
}
