using System;
using Gin.Attributes;
using Gin.Context;
using Gin.Logging;
using System.Reflection;
using System.Linq;


namespace Gin.Commands
{

    public enum CommandResult
    {
        Next,
        Previous
    }

    public enum CommandErrorLevel
    {
        [GinName(Name = "Остановить выполнение")]
        StopExecution = 0,
        [GinName(Name = "Пропустить оишбку")]
        SkipError = 1,
        [GinName(Name = "Записать в журнал")]
        AppendLog = 2,
        [GinName(Name = "Спросить пользователя")]
        AskUser = 3
    }

    public abstract class Command
    {

        [GinArgumentComplexValue(ValueType = typeof(UserInfoEmbedded), Name = "Пользовательское сообщение", Description = "Пользовательское сообщение верхнего уровня")]
        public UserInfoEmbedded UserInfo { get; set; }

        [GinArgumentText(Multiline = true, Name = "Комментарий", Description = "Комментарий к команде, используется только в построителе пакетов, для человекопонятного описания предназначения команды.")]
        public string Description { get; set; }

        [GinArgumentEnum(Name = "Реакция на ошибку", Description = "Уровень реакции команды на ошибку", ListEnum = typeof(CommandErrorLevel))]
        public CommandErrorLevel ErrorLevel { get; set; }

        protected int _progressCost = 10;

        public virtual int ProgressCost
        {
            get
            {
                return _progressCost;
            }
        }

        public abstract CommandResult Do(IExecutionContext context);

        public CommandResult Execute(IExecutionContext context)
        {
            try
            {
                CheckForPendingCancel(context);
                ShowUserInfo(context, UserInfoState.Running);
                CommandResult result = Do(context);
                ShowUserInfo(context, UserInfoState.Success);
                return result;
            }
            catch (PackageExecutionCancelledException)
            {
                ShowUserInfo(context, UserInfoState.Fault);
                throw;
            }
            catch (Exception ex)
            {
                switch (ErrorLevel)
                {
                    case CommandErrorLevel.StopExecution:
                        ShowUserInfo(context, UserInfoState.Fault);
                        throw;
                    case CommandErrorLevel.SkipError:
                        break;
                    case CommandErrorLevel.AppendLog:
                        context.Log.AddLogException(ex);
                        break;
                    case CommandErrorLevel.AskUser:
                        string message = ex.Message + " при выполнении команды " + GetHumanReadableName();
                        if (!context.AskUserToSkipError(message))
                        {
                            ShowUserInfo(context, UserInfoState.Fault);
                            throw;
                        }
                        break;
                }
                ShowUserInfo(context, UserInfoState.Dismiss);
                return CommandResult.Next;
            }
        }

        protected void CheckForPendingCancel(IExecutionContext context)
        {
            QueryCancelEventArgs args = new QueryCancelEventArgs();
            context.Log.GetPendingCancel(args);
            if (args.Cancel)
            {
                throw new PackageExecutionCancelledException();
            }
        }

        protected void ShowUserInfo(IExecutionContext context, UserInfoState state)
        {
            if ((UserInfo != null) && !String.IsNullOrEmpty(UserInfo.MessageText))
            {
                UserInfoData info = new UserInfoData
                {
                    Message = UserInfo.MessageText,
                    State = state,
                    Guid = UserInfo.MessageGuid
                };
                context.Log.AddUserInfo(info);
            }
        }

        protected void SetProgress(IExecutionContext context, Command command, CommandResult direction)
        {
            if (!(command is IProgressMyself))
            {
                int progressCost = command.ProgressCost;
                switch (direction)
                {
                    case CommandResult.Previous:
                        progressCost = -progressCost;
                        break;

                }
                context.Log.SendProgress(new ExecutionProgressInfo
                {
                    Message = String.Format("Выполнение команды <{0}> завершено", command.GetHumanReadableName()),
                    ModuleName = "",
                    ProgressCost = progressCost
                });
            }
        }

        public virtual void Visit(CommandVisitor visitor)
        {
            visitor.Visit(this);
        }

        public virtual string GetHumanReadableName()
        {
            GinNameAttribute attr = GetType().GetCustomAttributes(true).OfType<GinNameAttribute>().FirstOrDefault();
            if (attr == null)
            {
                return null;
            }
            if (String.IsNullOrEmpty(Description))
            {
                return attr.Name;
            }
            return attr.Name + "(" + Description + ")";
        }

        public Command Copy()
        {
            Type type = GetType();
            ConstructorInfo constructor = type.GetConstructor(new Type[0]);
            if (constructor == null)
            {
                return null;
            }
            object result = constructor.Invoke(new object[0]);
            if (result == null)
            {
                return null;
            }

            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                GinArgumentAttribute attr = property.GetCustomAttributes(true).OfType<GinArgumentAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    object propertyValue = property.GetValue(this, null);
                    property.SetValue(result, propertyValue, null);
                }
            }

            return (Command)result;
        }
    }
}
