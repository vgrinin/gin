namespace Gin.Attributes
{
    public enum CommandResultKind
    {
        Primitive = 0, // primitive types (bool, string)
        Complex ,   // complex types (например SqlParameterClass)
        Dynamic    // some variables will exists in context after execution (for example CMParseResult command)
    }
}