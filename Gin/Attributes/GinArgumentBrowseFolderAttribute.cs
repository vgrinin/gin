using System.Windows.Forms;

namespace Gin.Attributes
{
    /// <summary>
    /// Attribute which marks the properties with Browse Folder editor
    /// </summary>
    public class GinArgumentBrowseFolderAttribute : GinArgumentAttribute
    {
        public override Control GetEditor(object value, string propertyName, PackageBody body, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            return new Editors.BrowseFolderEditor(Name, (string)value, propertyName, onChange, onActivate);
        }
    }
}