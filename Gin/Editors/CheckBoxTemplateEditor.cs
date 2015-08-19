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
    public partial class CheckBoxTemplateEditor : UserControl, IEditor, ITemplatedEditor
    {

        private string _propertyName;
        private PropertyChangedDelegate _onChange;
        private PropertyActivatedDelegate _onActivate;

        public CheckBoxTemplateEditor(string title, object value, string propertyName, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            _propertyName = propertyName;
            _onChange = onChange;
            _onActivate = onActivate;
            InitializeComponent();
            Value = value;
            checkBox.Text = title;
            AdjustControlState();
        }

        public CheckBoxTemplateEditor()
        {
            InitializeComponent();
        }

        #region IEditor Members

        public object Value
        {
            get
            {
                if (radioStandard.Checked)
                {
                    return checkBox.Checked;
                }
                else
                {
                    return textTemplate.Text;
                }
            }
            set
            {
                if (value is bool)
                {
                    radioStandard.Checked = true;
                    checkBox.Checked = (bool)value;
                }
                else if (value is string)
                {
                    radioTemplate.Checked = true;
                    textTemplate.Text = (string)value;
                }
                else
                {
                    radioStandard.Checked = true;
                    checkBox.Checked = false;
                }
            }
        }

        #endregion

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            HandleValueChange();
        }

        private void CheckBoxEditor_Enter(object sender, EventArgs e)
        {
            if (_onActivate != null)
            {
                _onActivate(_propertyName);
            }
        }

        private void radioStandard_CheckedChanged(object sender, EventArgs e)
        {
            AdjustControlState();
        }

        private void AdjustControlState()
        {
            checkBox.Enabled = radioStandard.Checked;
            textTemplate.Enabled = !radioStandard.Checked;
        }

        private void textTemplate_TextChanged(object sender, EventArgs e)
        {
            HandleValueChange();
        }

        private void HandleValueChange()
        {
            if (_onChange != null)
            {
                _onChange(_propertyName, Value);
            }
        }

        #region ITemplatedEditor Members

        public void InsertAtCurrent(string value)
        {
            textTemplate.Text = value;
            radioTemplate.Checked = true;
        }

        #endregion
    }
}
