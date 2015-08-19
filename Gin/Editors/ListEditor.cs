using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Gin.Attributes;


namespace Gin.Editors
{
    public partial class ListEditor : UserControl, IEditor
    {

        private string _propertyName;
        private PropertyChangedDelegate _onChange;
        private PropertyActivatedDelegate _onActivate;
        private Type _listType;
        private Type _itemType;
        private PackageBody _body;

        public ListEditor(string title, IEnumerable<ComboBoxItem> list, Type listType, string propertyName, PackageBody body, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            _onChange = onChange;
            _onActivate = onActivate;
            _propertyName = propertyName;
            _listType = listType;
            _itemType = _listType.GetProperty("Item").PropertyType;
            _body = body;
            InitializeComponent();
            labelTitle.Text = title;
            PopulateList(list);
        }

        private void PopulateList(IEnumerable<ComboBoxItem> list)
        {
            foreach (var item in list)
            {
                listBox.Items.Add(item);
            }
        }

        public ListEditor()
        {
            InitializeComponent();
        }

        #region IEditor Members

        public object Value
        {
            get
            {
                object result = _listType.GetConstructor(new Type[0]).Invoke(null);
                MethodInfo addMethod = _listType.GetMethod("Add");

                foreach (var item in listBox.Items)
                {
                    addMethod.Invoke(result, new object[] { ((ComboBoxItem)item).Value });
                }
                return result;
            }
            set
            {
                PopulateList((IEnumerable<ComboBoxItem>)value);
            }
        }

        #endregion

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            object newValue = GetDefaultValueOrNull(_itemType);

            EditComplexValueForm form = new EditComplexValueForm();
            form.InitEditors(newValue, _body);
            if (form.ShowDialog() == DialogResult.OK)
            {
                newValue = form.ReadEditorValue(_itemType);
                ComboBoxItem newItem = new ComboBoxItem()
                {
                    Display = newValue.ToString(),
                    Value = newValue
                };
                listBox.Items.Add(newItem);
                SendChangeEvent();
            }
        }

        private object GetDefaultValueOrNull(Type _itemType)
        {
            object newValue = null;
            try
            {
                newValue = _itemType.GetConstructor(new Type[0]).Invoke(null);
            }
            catch { }
            return newValue;
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            TryToDeleteSelectedItem();
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            ComboBoxItem selected = (ComboBoxItem)listBox.SelectedItem;
            if (selected != null)
            {
                EditComplexValueForm form = new EditComplexValueForm();
                form.InitEditors(selected.Value, _body);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    object newValue = form.ReadEditorValue(_itemType);
                    ComboBoxItem selectedItem = (ComboBoxItem)selected;
                    selectedItem.Value = newValue;
                    selectedItem.Display = newValue.ToString();
                    int selectedIndex = listBox.SelectedIndex;
                    listBox.Items.RemoveAt(selectedIndex);
                    listBox.Items.Insert(selectedIndex, selectedItem);
                    listBox.SelectedIndex = selectedIndex;
                    SendChangeEvent();
                }
            }
        }

        private void listBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
            {
                TryToDeleteSelectedItem();
            }
        }

        private void TryToDeleteSelectedItem()
        {
            object selected = listBox.SelectedItem;
            if (selected != null)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить <" + selected.ToString() + ">?", "Вы уверены", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    listBox.Items.Remove(selected);
                    SendChangeEvent();
                }
            }
        }

        private void SendChangeEvent()
        {
            if (_onChange != null)
            {
                _onChange(_propertyName, Value);
            }
        }

        private void ListEditor_Enter(object sender, EventArgs e)
        {
            if (_onActivate != null)
            {
                _onActivate(_propertyName);
            }
        }
    }
}
