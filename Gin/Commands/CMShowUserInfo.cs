using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gin.Attributes;
using Gin.Context;
using Gin.Logging;

namespace Gin.Commands
{
        [GinName(Name = "Показать список пользовательских событий", Description = "Отображает в главном контейнере инсталлятора список пользовательских событий, для дальнейшего добавления в него событий", Group = "События")]
    public class CMShowUserInfo: Command
    {
        public override CommandResult Do(IExecutionContext context)
        {
            context.ControlContainer.Invoke(new Action(() =>
            {
                if (context.ControlContainer == null)
                {
                    context.Log.AddLogWarning("Пакет содержит команду отображения списка пользовательских событий, однако в контексте выполнения отсутствует родительский компонент для его отображения");
                    return;
                }
                ListView list = new ListView();
                list.Dock = DockStyle.Fill;
                list.View = View.List;
                ImageList imageList = new ImageList();
                imageList.ImageSize = new System.Drawing.Size(24, 24);
                imageList.Images.Add(Properties.Resources.running);
                imageList.Images.Add(Properties.Resources.success);
                imageList.Images.Add(Properties.Resources.error);
                imageList.Images.Add(Properties.Resources.dismiss);
                list.SmallImageList = imageList;

                PackageBody body = context.ExecutedPackage.GetBody();
                List<UserInfoEmbedded> infos = body.GetUserInfos();
                foreach (UserInfoEmbedded info in infos)
                {
                    list.Items.Add(info.MessageGuid, info.MessageText, -1);
                }
                context.Log.AddLogger(new ExecutionLoggerUserInfo(list));
                context.ControlContainer.Controls.Clear();
                context.ControlContainer.Controls.Add(list);
            }));
            return CommandResult.Next;
        }
    }
}
