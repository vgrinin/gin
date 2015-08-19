using System;

namespace Gin.Attributes
{
    /// <summary>
    /// Attribute which marks the result properties.
    /// There are properties which set the name of result in execution context.
    /// This attribute points the type and kind of result of command and used by package builder to automatically fill the list of possibly variables in current execution context.
    /// </summary>
    public class GinResultAttribute: Attribute
    {
        public Type Result { get; set; }
        public CommandResultKind Kind { get; set; }
        public string Description { get; set; }
    }
}
