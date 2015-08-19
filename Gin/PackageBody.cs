using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin.Attributes;
using Gin.Commands;
using Gin.PackageContent;


namespace Gin
{
    public class PackageBody
    {
        public const int DEFAULT_WIDTH = 475;
        public const int DEFAULT_HEIGHT = 300;
        public const int AUTO_ROLLBACK = 300;  // 5 минут

        public PackageBody()
        {
            PackageId = Guid.NewGuid().ToString("N");
            Height = DEFAULT_HEIGHT;
            Width = DEFAULT_WIDTH;
            AutoRollback = AUTO_ROLLBACK;
        }

        [GinArgumentText(Name = "ID пакета", Description = "ID пакета")]
        public string PackageId { get; set; }

        [GinArgumentText(Name = "Наименование ПО", Description = "Наименование ПО")]
        public string SoftwareName { get; set; }

        [GinArgumentInt(Name = "Автооткат пакета, сек", Description = "Время после запуска пакета, по истечении которого произойдет автоматический откат пакета, если пользователь не зафиксировал его")]
        public int AutoRollback { get; set; }

        public Command Command { get; set; }

        [GinArgumentInt(Name = "Ширина", Description = "Ширина")]
        public int Width { get; set; }

        [GinArgumentInt(Name = "Высота", Description = "Высота")]
        public int Height { get; set; }

        [GinArgumentText(Name = "Имя пакета", Description = "Имя пакета")]
        public string PackageName { get; set; }

        [GinArgumentBrowseFolder(Name = "Путь к файлам пакета", Description = "Путь к файлам, включаемым в пакет")]
        public string IncludedPath { get; set; }

        //[GinArgumentEnum(Name = "Тип содержимого", Description = "Способ содержимого контентных команд", ListEnum = typeof(PackageContentType))]
        public PackageContentType ContentType { get; set; }

        public List<UserInfoEmbedded> GetUserInfos()
        {
            List<UserInfoEmbedded> result = new List<UserInfoEmbedded>();

            if (Command == null)
            {
                return result;
            }

            UserInfoVisitor visitor = new UserInfoVisitor();
            Command.Visit(visitor);

            return visitor.UserInfos;
        }

        public List<ResultInfo> GetResultInfos()
        {
            List<ResultInfo> result = new List<ResultInfo>();

            if (Command == null)
            {
                return result;
            }

            ResultInfoVisitor visitor = new ResultInfoVisitor(Command);
            Command.Visit(visitor);

            return visitor.UserInfos;
        }

    }
}
