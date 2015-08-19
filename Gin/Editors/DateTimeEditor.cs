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
    public partial class DateTimeEditor : UserControl, IEditor
    {

        private string _propertyName; 
        private PropertyChangedDelegate _onChange;
        private PropertyActivatedDelegate _onActivate;

        public DateTimeEditor(string title, DateTime value, bool allowTemplates, string propertyName, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            _propertyName = propertyName;
            _onChange = onChange;
            _onActivate = onActivate; 
            InitializeComponent();
            labelTitle.Text = title;
            dateTimePicker.Value = value;
        }

        public DateTimeEditor()
        {
            InitializeComponent();
        }

        #region IEditor Members

        public object Value
        {
            get
            {
                return dateTimePicker.Value;
            }
            set
            {
                if (value is DateTime)
                {
                    dateTimePicker.Value = (DateTime)value;
                }
            }
        }

        #endregion

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if (_onChange != null)
            {
                _onChange(_propertyName, dateTimePicker.Value);
            }
        }

        private void DateTimeEditor_Enter(object sender, EventArgs e)
        {
            if (_onActivate != null)
            {
                _onActivate(_propertyName);
            }
        }
    }
}
