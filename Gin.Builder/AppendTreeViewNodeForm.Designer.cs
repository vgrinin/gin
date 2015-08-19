namespace Gin.Builder
{
    partial class AppendTreeViewNodeForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioCreateSequence = new System.Windows.Forms.RadioButton();
            this.radioReplaceNode = new System.Windows.Forms.RadioButton();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioCreateSequence);
            this.groupBox1.Controls.Add(this.radioReplaceNode);
            this.groupBox1.Location = new System.Drawing.Point(4, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(187, 70);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Куда добавить узел?";
            // 
            // radioCreateSequence
            // 
            this.radioCreateSequence.AutoSize = true;
            this.radioCreateSequence.Checked = true;
            this.radioCreateSequence.Location = new System.Drawing.Point(3, 47);
            this.radioCreateSequence.Name = "radioCreateSequence";
            this.radioCreateSequence.Size = new System.Drawing.Size(175, 17);
            this.radioCreateSequence.TabIndex = 2;
            this.radioCreateSequence.TabStop = true;
            this.radioCreateSequence.Text = "создать последовательность";
            this.radioCreateSequence.UseVisualStyleBackColor = true;
            // 
            // radioReplaceNode
            // 
            this.radioReplaceNode.AutoSize = true;
            this.radioReplaceNode.Location = new System.Drawing.Point(3, 21);
            this.radioReplaceNode.Name = "radioReplaceNode";
            this.radioReplaceNode.Size = new System.Drawing.Size(141, 17);
            this.radioReplaceNode.TabIndex = 1;
            this.radioReplaceNode.Text = "заменить содержимое";
            this.radioReplaceNode.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(4, 82);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "Принять";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(107, 82);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Отменить";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // AppendTreeViewNodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(190, 110);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AppendTreeViewNodeForm";
            this.Text = "Добавление узла";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioCreateSequence;
        private System.Windows.Forms.RadioButton radioReplaceNode;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;

    }
}