using System;
using System.Collections.Generic;
using Gin.Commands;
using System.IO;
using Gin.Context;
using Gin.PackageContent;
using Gin.Visitors;

namespace Gin
{
    public class PackageBuilder
    {

        public const string PACKAGE_DEFAULT_FILENAME = @"package.gin";


        readonly PackageBody _body;
        readonly PackageContent.PackageContent _content;

        public event Action OnSaved;

        public List<PackageErrorInfo> Errors { get; private set; }

        private void OnSavedHandler()
        {
            if (OnSaved != null)
            {
                OnSaved();
            }
        }

        public PackageBuilder(PackageBody body, PackageContentType contentType)
        {
            body.ContentType = contentType;
            _content = PackageContent.PackageContent.Create(contentType);
            Errors = new List<PackageErrorInfo>();
            _body = body;
        }

        public bool Save(string path, bool isFileName)
        {
            CheckPackageVisitor visitor = new CheckPackageVisitor(_body);
            _body.Command.Visit(visitor);
            visitor.CollectInfo();
            if (!visitor.IsCorrect)
            {
                Errors = visitor.Errors;
                return false;
            }
            ProcessIncludedFiles(_body.Command);
            string xmlFilePath = Path.GetTempFileName();
            GinSerializer.Serialize(_body, xmlFilePath);
            _content.AddBody(xmlFilePath);
            string packageFilePath = isFileName ? path : Path.Combine(path, PACKAGE_DEFAULT_FILENAME);
            _content.Save(packageFilePath, _body.IncludedPath);
            OnSavedHandler();
            return true;
        }

        public IAsyncResult SaveAsync(string filePath, bool isFileName)
        {
            Func<string, bool, bool> del = Save;
            return del.BeginInvoke(filePath, isFileName, SavedCallback, del);
        }

        private void SavedCallback(IAsyncResult asyncResult)
        {
            Action<string, bool> del = (Action<string, bool>)(asyncResult.AsyncState);
            del.EndInvoke(asyncResult);
        }

        private void ProcessIncludedFiles(Command command)
        {
            if (command is IContainerCommand)
            {
                IContainerCommand iContainer = (IContainerCommand)command;
                if (iContainer.InnerCommands == null)
                {
                    return;
                }
                foreach (Command cmd in iContainer.InnerCommands)
                {
                    ProcessIncludedFiles(cmd);
                }
            }

            if (command is IContentCommand)
            {
                IContentCommand cntCommand = (IContentCommand)command;
                string sourceFilePath = cntCommand.ContentPath;
                if (sourceFilePath == null)
                {
                    return;
                }
                if (sourceFilePath.Contains(ExecutionContext.PACKAGE_CONTENT_TAG) ||
                    sourceFilePath.Contains(ExecutionContext.GIN_EXE_TAG))
                {
                    return;
                }
                cntCommand.ContentPath = _content.AddContent(sourceFilePath);
            }
            if (command is IMultipleContentCommand)
            {
                IMultipleContentCommand cntCommand = (IMultipleContentCommand)command;
                if (cntCommand.ContentPaths != null)
                {
                    foreach (ContentPath p in cntCommand.ContentPaths)
                    {
                        string sourceFilePath = p.Value;
                        if (sourceFilePath == null)
                        {
                            return;
                        }
                        if (sourceFilePath.Contains(ExecutionContext.PACKAGE_CONTENT_TAG) ||
                            sourceFilePath.Contains(ExecutionContext.GIN_EXE_TAG))
                        {
                            return;
                        }
                        p.Value = _content.AddContent(sourceFilePath);
                    }
                }

            }

        }
    }
}
