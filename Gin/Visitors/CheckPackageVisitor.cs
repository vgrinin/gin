using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using Gin.Attributes;
using Gin.Commands;
using Gin.Context;

namespace Gin.Visitors
{
    public class CheckPackageVisitor : CommandVisitor
    {

        private readonly PackageBody _body;
        public CheckPackageVisitor(PackageBody body)
        {
            _body = body;
        }

        public void CollectInfo()
        {
            if (_containsUserInfos && !_containsUserInfoForm)
            {
                CMShowUserInfo cmd = new CMShowUserInfo();
                _errors.Add(new PackageErrorInfo
                {
                    Body = _body,
                    Command = null,
                    Description = "Пакет выводит пользовательские сообщения, однако не содержит в себе команду отображения списка пользовательских сообщений. Необходимо добавить в пакет команду <" + cmd.GetHumanReadableName() + ">" 
                });
            }
        }

        public bool IsCorrect 
        {
            get
            {
                return _errors.Count == 0;
            }
        }
        private readonly List<PackageErrorInfo> _errors = new List<PackageErrorInfo>();


        public List<PackageErrorInfo> Errors
        {
            get
            {
                return _errors;
            }
        }

        public override void Visit(Command command)
        {
            CheckPackageSourceNeeds(command);
            CheckResultNamePresent(command);
            CheckUserInfos(command);
        }

        private bool _containsUserInfos;
        private bool _containsUserInfoForm;
        private void CheckUserInfos(Command command)
        {
            if (command is UserInfo)
            {
                _containsUserInfos = true;
            }
            if (command.UserInfo != null && !String.IsNullOrEmpty(command.UserInfo.MessageText))
            {
                _containsUserInfos = true;
            }
            if (command is CMShowUserInfo)
            {
                _containsUserInfoForm = true;
            }
        }

        private void CheckResultNamePresent(Command command)
        {
            PropertyInfo[] properties = command.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                GinResultAttribute attr = (GinResultAttribute)property.GetCustomAttributes(typeof(GinResultAttribute), true).FirstOrDefault();
                if (attr != null)
                {
                    string name = (string)property.GetValue(command, null);
                    if (String.IsNullOrEmpty(name))
                    {
                        _errors.Add(new PackageErrorInfo
                        {
                            Body = _body,
                            Command = command,
                            Description = "У команды " + command + "(" + command.Description + ") отсутствует параметр 'Имя результата'"
                        });
                    }
                }
            }
        }

        private bool _containsPackageSource;
        private void CheckPackageSourceNeeds(Command command)
        {
            if (command is IContentCommand)
            {
                IContentCommand iCont = (IContentCommand) command;
                if (iCont.ContentPath != null && iCont.ContentPath.StartsWith(ExecutionContext.PACKAGE_SOURCE_TAG))
                {
                    if (!_containsPackageSource && String.IsNullOrEmpty(_body.IncludedPath))
                    {
                        _errors.Add(new PackageErrorInfo
                                        {
                                            Body = _body,
                                            Command = null,
                                            Description =
                                                "В пакете используется доступ по относительным путям %PACKAGE_SOURCE%. Необходимо указать для пакета путь к исходным файлам, включаемым в пакет."
                                        });
                        _containsPackageSource = true;
                    }
                    else
                    {
                        CheckFilePresent(command);
                    }
                }
            }
            if (command is IMultipleContentCommand)
            {
                IMultipleContentCommand iCont = (IMultipleContentCommand) command;
                foreach (ContentPath p in iCont.ContentPaths)
                {
                    if (p.Value != null && p.Value.StartsWith(ExecutionContext.PACKAGE_SOURCE_TAG))
                    {
                        if (!_containsPackageSource && String.IsNullOrEmpty(_body.IncludedPath))
                        {
                            _errors.Add(new PackageErrorInfo
                                            {
                                                Body = _body,
                                                Command = null,
                                                Description =
                                                    "В пакете используется доступ по относительным путям %PACKAGE_SOURCE%. Необходимо указать для пакета путь к исходным файлам, включаемым в пакет."
                                            });
                            _containsPackageSource = true;
                        }
                        else
                        {
                            CheckFilePresent(command);
                        }
                    }

                }
            }
        }

        private void CheckFilePresent(Command command)
        {
//#warning нет проверки для IMultipleContentCommand
            if (!(command is IContentCommand))
            {
                return;
            }
            IContentCommand iCont = (IContentCommand)command;
            string contentPath = iCont.ContentPath;
            if (contentPath.Contains(ExecutionContext.PACKAGE_CONTENT_TAG))
            {
                return;
            }
            string currentPath = null;
            if (contentPath.Contains(ExecutionContext.PACKAGE_SOURCE_TAG))
            {
                currentPath = contentPath.Replace(ExecutionContext.PACKAGE_SOURCE_TAG, _body.IncludedPath);
            }
            if (String.IsNullOrEmpty(currentPath))
            {
                _errors.Add(new PackageErrorInfo
                {
                    Body = _body,
                    Command = command,
                    Description = "У команды <" + command.GetHumanReadableName() + "> отсутствует указание на исходный файловый объект."
                });
                return;
            }

            if (!File.Exists(currentPath) && !Directory.Exists(currentPath))
            {
                _errors.Add(new PackageErrorInfo
                {
                    Body = _body,
                    Command = command,
                    Description = "У команды <" + command.GetHumanReadableName() + "> указан исходный файловый объект '" + currentPath + "' отсутствующий физически в указанном расположении."
                });
            }
        }
    }

    public class PackageErrorInfo
    {
        public string Description;
        public Command Command;
        public PackageBody Body;
    }
}
