using System;
using System.Windows.Forms;

namespace Gin.Attributes
{
    /// <summary>
    /// Base class for all GinArgument attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class GinArgumentAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool AllowTemplates { get; set; }

        /// <summary>
        /// Returns the UI-editor for this property
        /// </summary>
        /// <param name="value">Initial value for editor</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="body">The PackageBody of current package</param>
        /// <param name="onChange">PropertyChanged delegate</param>
        /// <param name="onActivate">PropertyActivated delegate</param>
        /// <returns>Windows Forms Control, which supplies the edit value function for this class property</returns>
        public abstract Control GetEditor(object value, string propertyName, PackageBody body, PropertyChangedDelegate onChange, PropertyActivatedDelegate onActivate);
    }
}