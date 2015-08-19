using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Gin.Attributes;
using Gin.Commands;
using Gin.CommandTree;


namespace Gin.Editors
{

    public delegate void PropertyHelpCallback(string propertyProgramName, string propertyName, string propertyDescription, bool allowTemplates, ITemplatedEditor editor);

    public static class FormsHelper
    {
        public static void CreateNodeEditor(CommandTreeNode node, object editedObject, Control container, PackageBody body, PropertyHelpCallback helpCallback)
        {
            container.Controls.Clear();
            FlowLayoutPanel flowPanel;
            if (container is FlowLayoutPanel)
            {
                flowPanel = (FlowLayoutPanel)container;
            }
            else
            {
                flowPanel = new FlowLayoutPanel();
                container.Controls.Add(flowPanel);
                flowPanel.FlowDirection = FlowDirection.TopDown;
                flowPanel.Dock = DockStyle.Fill;
            }
            Type type = editedObject.GetType();
            var properties = type.GetProperties();
            foreach (var currentProperty in properties)
            {
                GinArgumentAttribute argumentAttr = (GinArgumentAttribute)currentProperty.GetCustomAttributes(typeof(GinArgumentAttribute), false).FirstOrDefault();
                bool isArgument = argumentAttr != null;
                if (isArgument)
                {
                    object value = currentProperty.GetValue(editedObject, null);
                    Control control = null;
                    control = argumentAttr.GetEditor(value, currentProperty.Name, body, new PropertyChangedDelegate(
                         (propertyName, propertyValue) =>
                         {
                             PropertyInfo changedProperty = type.GetProperty(propertyName);
                             if (changedProperty != null)
                             {
                                 changedProperty.SetValue(editedObject, propertyValue, null);
                                 if (editedObject is Command)
                                 {
                                     string name = ((Command)editedObject).GetHumanReadableName();
                                     if (name != node.NodeName)
                                     {
                                         node.NodeName = name;
                                     }
                                 }
                             }
                         }),
                         new PropertyActivatedDelegate(propertyName =>
                         {
                             if (helpCallback != null)
                             {
                                 helpCallback(propertyName, argumentAttr.Name, argumentAttr.Description, argumentAttr.AllowTemplates, control as ITemplatedEditor);
                             }
                         }));
                    if (control != null)
                    {
                        control.Tag = currentProperty;
                        flowPanel.Controls.Add(control);
                    }
                }
            }
        }

        public static object GetEditorValue(Type valueType, Control container)
        {
            object result = valueType.GetConstructor(new Type[0]).Invoke(null);

            foreach (Control control in container.Controls)
            {
                if (control is IEditor)
                {
                    PropertyInfo propInfo = (PropertyInfo)control.Tag;
                    if (propInfo != null)
                    {
                        IEditor iEditor = (IEditor)control;
                        object value = iEditor.Value;
                        propInfo.SetValue(result, value, null);
                    }
                }
            }

            return result;
        }
    }
}
