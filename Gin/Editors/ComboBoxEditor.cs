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
    public partial class ComboBoxEditor : UserControl, IEditor
    {

        private string _propertyName;
        private PropertyChangedDelegate _onChange;
        private PropertyActivatedDelegate _onActivate;

        public ComboBoxEditor(string title, object value, bool dropDownListOnly, IEnumerable<ComboBoxItem> list, string propertyName, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            _onChange = onChange;
            _onActivate = onActivate;
            _propertyName = propertyName;
            InitializeComponent();
            labelTitle.Text = title;
            comboBox.DropDownStyle = dropDownListOnly ? ComboBoxStyle.DropDownList : ComboBoxStyle.DropDown;
            comboBox.ValueMember = "Value";
            comboBox.DisplayMember = "Display";
            comboBox.DataSource = list;
            foreach (var item in comboBox.Items)
            {
                if (((ComboBoxItem)item).Value.ToString() == value.ToString())
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
            comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
        }

        public ComboBoxEditor()
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
                ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
                _onChange(_propertyName, selectedItem.Value);
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
