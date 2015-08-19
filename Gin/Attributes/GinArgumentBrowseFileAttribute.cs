using System.Windows.Forms;

namespace Gin.Attributes
{
    /// <summary>
    /// Attribute which marks the properties with Browse File editor
    /// </summary>
    public class GinArgumentBrowseFileAttribute : GinArgumentAttribute
    {
        /// <summary>
        /// True for "Save file" dialog, False for "Open file" dialog
        /// </summary>
        public bool IsNewFile { get; set; }

        public override Control GetEditor(object value, string propertyName, PackageBody body, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            return new Editors.BrowseFileEditor(IsNewFile, Name, (string)value, propertyName, onChange, onActivate);
        }
    }
}