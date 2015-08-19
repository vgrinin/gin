namespace Gin.Commands
{
    public interface ICanCreateFromFile
    {
        bool IsAssumedCommand(string fileName);
        void InitFromFile(string fileName);
    }
}
