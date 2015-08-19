using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gin.Attributes;

namespace Gin.Editors
{
    public partial class BrowseFileEditor : UserControl, IEditor, ITemplatedEditor
    {

        private PropertyChangedDelegate _onChange;
        private PropertyActivatedDelegate _onActivate;
        private string _propertyName;
        private bool _isNewFile;


        public BrowseFileEditor(bool isNewFile, string title, string value, string propertyName, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            _onChange = onChange;
            _onActivate = onActivate;
            _propertyName = propertyName;
            _isNewFile = isNewFile;
            InitializeComponent();
            labelTitle.Text = title;
            textValue.Text = value;
        }

        public BrowseFileEditor()
        {
            InitializeComponent();
        }

        #region IEditor Members

        public object Value
        {
            get
            {
                return textValue.Text;
            }
            set
            {
                if (value is string)
                {
                    textValue.Text = (string)value;
                }
            }
        }

        #endregion

        private void textValue_TextChanged(object sender, EventArgs e)
        {
            if (_onChange != null)
            {
                _onChange(_propertyName, textValue.Text);
            }
        }

        private void BrowseFileEditor_Resize(object sender, EventArgs e)
        {
            buttonBrowse.Left = this.Width - this.Padding.Right - buttonBrowse.Width;
            textValue.Width = this.Width - this.Padding.Left * 2 - this.Padding.Right - buttonBrowse.Width;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {

            SaveFileDialog sv = new SaveFileDialog();
            FileDialog dialog = _isNewFile ? (FileDialog)new SaveFileDialog() : (FileDialog)new OpenFileDialog();
            dialog.FileName = textValue.Text;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textValue.Text = dialog.FileName;
            }
        }

        private void BrowseFileEditor_Enter(object sender, EventArgs e)
        {
            if (_onActivate != null)
            {
                _onActivate(_propertyName);
            }
        }

        #region ITemplatedEditor Members

        public void InsertAtCurrent(string value)
        {
            string v = textValue.Text;
            v = v.Substring(0, textValue.SelectionStart) + value + v.Substring(textValue.SelectionStart + textValue.SelectionLength);
            textValue.Text = v;
        }

        #endregion
    }
}
