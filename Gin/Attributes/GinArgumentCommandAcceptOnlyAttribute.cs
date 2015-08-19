using System;

namespace Gin.Attributes
{
    /// <summary>
    /// Attribute which points types only can be accepted by (set to) marked property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class GinArgumentCommandAcceptOnlyAttribute: Attribute
    {
        /// <summary>
        /// Type what can be accepted by marked property
        /// </summary>
        public Type AcceptedType { get; set; }
    }
}
