using Gin.Attributes;

namespace Gin.Commands
{
    public enum MSISearchType
    {
        [GinName(Name = "Код продукта")]
        ProductCode,
        [GinName(Name = "Имя продукта")]
        ProductName
    }
}
