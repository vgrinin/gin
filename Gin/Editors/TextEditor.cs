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
    public partial class TextEditor : UserControl, IEditor, ITemplatedEditor
    {

        private PropertyChangedDelegate _onChange;
        private PropertyActivatedDelegate _onActivate;
        private string _propertyName;

        public TextEditor(string title, string value, int maxLength, string propertyName, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            _onChange = onChange;
            _onActivate = onActivate;
            _propertyName = propertyName;
            InitializeComponent();
            labelTitle.Text = title;
            textValue.Text = value;
            if (maxLength > 0)
            {
                textValue.MaxLength = maxLength;
            }
        }

        public TextEditor()
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

        private void TextEditor_Enter(object sender, EventArgs e)
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
