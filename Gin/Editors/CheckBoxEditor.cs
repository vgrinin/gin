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
    public partial class CheckBoxEditor : UserControl, IEditor
    {

        private string _propertyName;
        private PropertyChangedDelegate _onChange;
        private PropertyActivatedDelegate _onActivate;

        public CheckBoxEditor(string title, bool value, string propertyName, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            _propertyName = propertyName;
            _onChange = onChange;
            _onActivate = onActivate;
            InitializeComponent();
            checkBox.Checked = value;
            checkBox.Text = title;
        }

        public CheckBoxEditor()
        {
            InitializeComponent();
        }

        #region IEditor Members

        public object Value
        {
            get
            {
                return checkBox.Checked;
            }
            set
            {
                if (value is bool)
                {
                    checkBox.Checked = (bool)value;
                }
            }
        }

        #endregion

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_onChange != null)
            {
                _onChange(_propertyName, checkBox.Checked);
            }
        }

        private void CheckBoxEditor_Enter(object sender, EventArgs e)
        {
            if (_onActivate != null)
            {
                _onActivate(_propertyName);
            }
        }
    }
}
