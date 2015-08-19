using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections;
using Gin.Editors;

namespace Gin.Attributes
{
    /// <summary>
    /// Attribute which marks the properties with custom ComboBox editor editor
    /// </summary>
    public class GinArgumentListAttribute : GinArgumentAttribute
    {
        public Type ListType { get; set; }

        public override Control GetEditor(object value, string propertyName, PackageBody body, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate)
        {
            Control control = null;
            var iList = value as IList;
            if (iList != null)
            {
                var list = new List<ComboBoxItem>();
                foreach (var item in iList)
                {
                    list.Add(new ComboBoxItem
                                 {
                                     Display = item.ToString(),
                                     Value = item
                                 });
                }
                control = new ListEditor(Name, list, ListType, propertyName, body, onChange, onActivate);
            }
            return control;
        }
    }
}