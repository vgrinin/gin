using System.Windows.Forms;

namespace Gin.Attributes
{
    /// <summary>
    /// Attribute which marks the properties with CheckBox editor
    /// </summary>
    public class GinArgumentCheckBoxAttribute : GinArgumentAttribute
    {

        public override Control GetEditor(object value, string propertyName, PackageBody body, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            Control control;
            if (AllowTemplates)
            {
                control = new Editors.CheckBoxTemplateEditor(Name, value, propertyName, onChange, onActivate);
            }
            else
            {
                var boolValue = false;
                if (value is bool)
                {
                    boolValue = (bool)value;
                }
                control = new Editors.CheckBoxEditor(Name, boolValue, propertyName, onChange, onActivate);
            }
            return control;
        }
    }
}