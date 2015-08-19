using System;
using System.Windows.Forms;

namespace Gin.Attributes
{
    /// <summary>
    /// Attribute which marks the properties with DateTimePicker editor
    /// </summary>
    public class GinArgumentDateTimeAttribute : GinArgumentAttribute
    {
        public override Control GetEditor(object value, string propertyName, PackageBody body, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            var dateTime = (DateTime)value;
            if (dateTime <= DateTime.MinValue || dateTime >= DateTime.MaxValue)
            {
                dateTime = DateTime.Now;
            }
            Control control = new Editors.DateTimeEditor(Name, dateTime, AllowTemplates, propertyName, onChange, onActivate);
            return control;
        }
    }
}