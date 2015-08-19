namespace Gin.Commands
{
    public interface IReversibleCommand
    {
        bool IsFirst { get; set; }
        bool IsLast { get; set; }
    }
}
