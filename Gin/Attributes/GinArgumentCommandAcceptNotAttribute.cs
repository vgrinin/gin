using System;

namespace Gin.Attributes
{
    /// <summary>
    /// Attribute which points types what can not be accepted by (set to) marked property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class GinArgumentCommandAcceptNotAttribute: Attribute
    {
        /// <summary>
        /// Type what can not be accepted by marked property
        /// </summary>
        public Type NotAcceptedType { get; set; }
    }
}
