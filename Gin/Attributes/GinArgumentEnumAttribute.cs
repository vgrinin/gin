using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Gin.Attributes
{
    /// <summary>
    /// Attribute which marks the properties with Enum editor.
    /// Enum editor is ComboBox with list populated automatically from ListEnum property definition
    /// </summary>
    public class GinArgumentEnumAttribute : GinArgumentAttribute
    {
        /// <summary>
        /// Type of enumeration. Neds to automatic population of list data.
        /// </summary>
        public Type ListEnum { get; set; }

        public override Control GetEditor(object value, string propertyName, PackageBody body, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            var fields = ListEnum.GetFields().Where(f => f.FieldType == ListEnum).ToList();
            var list = new List<Editors.ComboBoxItem>();
            foreach (var f in fields)
            {
                var nameAttr = f.GetCustomAttributes(true).OfType<GinNameAttribute>().FirstOrDefault();
                list.Add(new Editors.ComboBoxItem
                             {
                    Display = nameAttr != null ? nameAttr.Name : f.Name,
                    Value = f.GetValue(null)
                });
            }
            Control control = new Editors.ComboBoxEditor(Name, value, true, list, propertyName, onChange, onActivate);
            return control;
        }
    }
}