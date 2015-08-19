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
    public partial class IntEditor : UserControl, IEditor
    {

        private PropertyChangedDelegate _onChange;
        private PropertyActivatedDelegate _onActivate;
        private string _propertyName;

        public IntEditor(string title, int value, bool allowTemplates, string propertyName, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            _onChange = onChange;
            _onActivate = onActivate;
            _propertyName = propertyName;
            InitializeComponent();
            labelTitle.Text = title;
            textValue.Text = value.ToString();
        }

        public IntEditor()
        {
            InitializeComponent();
        }

        #region IEditor Members

        public object Value
        {
            get
            {
                int res = 0;
                if (!Int32.TryParse(textValue.Text, out res))
                {
                    MessageBox.Show("Неверное целое значение");
                }
                return res;
            }
            set
            {
                if (value is int)
                {
                    textValue.Text = ((int)value).ToString();
                }
            }
        }

        #endregion

        private void textValue_TextChanged(object sender, EventArgs e)
        {
            if (_onChange != null)
            {
                int res = 0;
                if (Int32.TryParse(textValue.Text, out res))
                {
                    _onChange(_propertyName, res);
                }
            }
        }

        private void IntEditor_Enter(object sender, EventArgs e)
        {
            if (_onActivate != null)
            {
                _onActivate(_propertyName);
            }
        }
    }
}
