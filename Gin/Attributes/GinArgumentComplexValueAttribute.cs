using System;
using System.Windows.Forms;

namespace Gin.Attributes
{
    /// <summary>
    /// Attribute which marks properties with complex type editors. 
    /// Complex type editor is form with some nested simple editors.
    /// </summary>
    public class GinArgumentComplexValue : GinArgumentAttribute
    {
        /// <summary>
        /// Type of complex value. The properties of complex type are marks with GinArgument attributes.
        /// </summary>
        public Type ValueType { get; set; }

        public override Control GetEditor(object value, string propertyName, PackageBody body, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            Control control = new Editors.ComplexValueEditor(Name, value, ValueType, propertyName, body, onChange, onActivate);
            return control;
        }
    }
}