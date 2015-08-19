namespace Gin.Attributes
{
    /// <summary>
    /// Delegate invoked when property changed
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <param name="value">New value of peroperty</param>
    public delegate void PropertyChangedDelegate(string propertyName, object value);

    /// <summary>
    /// Delegate invoked when property is activated (focused)
    /// </summary>
    /// <param name="propertyName"></param>
    public delegate void PropertyActivatedDelegate(string propertyName);
}