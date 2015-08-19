using System;

namespace Gin.Attributes
{
    /// <summary>
    /// Metadata which describes the Command derived classes. 
    /// Is used by package builders.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class GinNameAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
    }
}