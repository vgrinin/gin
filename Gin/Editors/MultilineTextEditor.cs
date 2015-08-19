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
    public partial class MultilineTextEditor : UserControl, IEditor, ITemplatedEditor
    {

        private PropertyChangedDelegate _onChange;
        private PropertyActivatedDelegate _onActivate;
        private string _propertyName;

        public MultilineTextEditor(string title, string value, int maxLength, string propertyName, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            _propertyName = propertyName;
            _onChange = onChange;
            _onActivate = onActivate;
            InitializeComponent();
            labelTitle.Text = title;
            textValue.Text = value;
            if (maxLength > 0)
            {
                textValue.MaxLength = maxLength;
            }
        }

        public MultilineTextEditor()
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

        private void MultilineTextEditor_Enter(object sender, EventArgs e)
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
