using System;
using System.Windows.Forms;
using Gin.Attributes;


namespace Gin.Editors
{
    public partial class ComplexValueEditor : UserControl, IEditor
    {

        private readonly string _propertyName;
        private readonly PropertyChangedDelegate _onChange;
        private readonly PropertyActivatedDelegate _onActivate;
        private readonly Type _valueType;
        private readonly PackageBody _body;
        private object _value;

        public ComplexValueEditor(string title, object value, Type valueType, string propertyName, PackageBody body, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            if (value == null)
            {
                value = GetDefaultValueOrNull(valueType);
            }
            _onChange = onChange;
            _onActivate = onActivate;
            _propertyName = propertyName;
            _valueType = valueType;
            _value = value;
            _body = body;
            InitializeComponent();
            labelTitle.Text = title;
            if (value != null)
            {
                labelValue.Text = value.ToString();
            }
        }

        public ComplexValueEditor()
        {
            InitializeComponent();
        }

        #region IEditor Members

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        #endregion

        private object GetDefaultValueOrNull(Type itemType)
        {
            object newValue;
            try
            {
                newValue = itemType.GetConstructor(new Type[0]).Invoke(null);
            }
            catch
            {
                newValue = null;
            }
            return newValue;
        }

        private void SendChangeEvent()
        {
            if (_onChange != null)
            {
                _onChange(_propertyName, Value);
            }
        }

        private void ListEditorEnter(object sender, EventArgs e)
        {
            if (_onActivate != null)
            {
                _onActivate(_propertyName);
            }
        }

        private void ButtonEditClick(object sender, EventArgs e)
        {
            var form = new EditComplexValueForm();
            form.InitEditors(_value, _body);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _value = form.ReadEditorValue(_valueType);
                if (_value != null)
                {
                    labelValue.Text = _value.ToString();
                }
                SendChangeEvent();
            }
        }

        private void ComplexValueEditorResize(object sender, EventArgs e)
        {
            labelValue.Width = this.Width - labelValue.Padding.Right - labelValue.Padding.Left;
        }
    }
}
