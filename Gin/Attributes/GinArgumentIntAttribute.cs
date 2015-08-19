using System.Windows.Forms;

namespace Gin.Attributes
{
    /// <summary>
    /// Attribute which marks the properties with integer number editor
    /// </summary>
    public class GinArgumentIntAttribute : GinArgumentAttribute
    {
        public override Control GetEditor(object value, string propertyName, PackageBody body, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            Control control = new Editors.IntEditor(Name, (int)value, AllowTemplates, propertyName, onChange, onActivate);
            return control;
        }
    }
}