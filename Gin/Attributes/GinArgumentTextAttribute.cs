using System.Windows.Forms;

namespace Gin.Attributes
{
    /// <summary>
    /// Attribute which marks property with text editor, maybe multiline, or with max length limit.
    /// </summary>
    public class GinArgumentTextAttribute : GinArgumentAttribute
    {
        public int MaxLength { get; set; }
        public bool Multiline { get; set; }

        public override Control GetEditor(object value, string propertyName, PackageBody body, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            Control control;
            if (Multiline)
            {
                control = new Editors.MultilineTextEditor(Name, (string)value, MaxLength, propertyName, onChange, onActivate);
            }
            else
            {
                control = new Editors.TextEditor(Name, (string)value, MaxLength, propertyName, onChange, onActivate);
            }
            return control;
        }
    }
}