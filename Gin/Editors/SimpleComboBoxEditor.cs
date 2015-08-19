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
    public partial class SimpleComboBoxEditor : UserControl, IEditor
    {

        private string _propertyName;
        private PropertyChangedDelegate _onChange;
        private PropertyActivatedDelegate _onActivate;

        public SimpleComboBoxEditor(string title, object list, bool dropDownListOnly, string displayField, string valueField, string propertyName, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            _onChange = onChange;
            _onActivate = onActivate;
            _propertyName = propertyName;
            InitializeComponent();
            labelTitle.Text = title;
            comboBox.DropDownStyle = dropDownListOnly ? ComboBoxStyle.DropDownList : ComboBoxStyle.DropDown;
            comboBox.ValueMember = valueField;
            comboBox.DisplayMember = displayField;
            comboBox.DataSource = list;
            comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
        }

        public SimpleComboBoxEditor()
        {
            InitializeComponent();
        }

        #region IEditor Members

        public object Value
        {
            get
            {
                return comboBox.SelectedValue;
            }
            set
            {
                comboBox.SelectedValue = value;
            }
        }

        #endregion

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_onChange != null)
            {
                _onChange(_propertyName, comboBox.SelectedValue);
            }

        }

        private void ComboBoxEditor_Enter(object sender, EventArgs e)
        {
            if (_onActivate != null)
            {
                _onActivate(_propertyName);
            }
        }
    }
}
