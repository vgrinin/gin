using System.Windows.Forms;

namespace Gin.Attributes
{
    /// <summary>
    /// Attribute which marks properties of Command(s) type, this properties are nodes in Command tree
    /// </summary>
    public class GinArgumentCommandAttribute : GinArgumentAttribute
    {
        public bool IsEnumerable { get; set; }

        /// <summary>
        /// Returns null, 'cause it has no standard editor
        /// </summary>
        /// <returns>null</returns>
        public override Control GetEditor(object value, string propertyName, PackageBody body, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            return null;
        }
    }
}