using System;
using System.Collections.Generic;
using System.Linq;
using Gin.Logging;
using System.IO;

namespace Gin.PackageContent
{
    public enum PackageContentType
    {
        Empty,
        Direct,
        Packed,
        Wrong
    }

    public abstract class PackageContent
    {
        public const string MAIN_PACKAGE_FILENAME = @"package.xml";
        public const string PACKAGE_INCLUDED_FILES_DIRECTORY = @"included";

        protected bool ContainFileName(string fileName)
        {
            return FileMap.ContainsValue(fileName);
        }

        public abstract string AddContent(string sourcePath);
        protected void AddContent(string sourcePath, string destFileName)
        {
            FileMap.Add(sourcePath, destFileName);
        }
        protected string PackageBodyFilePath;
        public void AddBody(string sourcePath)
        {
            PackageBodyFilePath = sourcePath;
            AddContent(sourcePath, MAIN_PACKAGE_FILENAME);
        }
        
        public string GetBody()
        {
            return PackageBodyFilePath;
        }

        public abstract void Save(string resultFilePath, string includedFilesPath);
        protected string GetFilePath(string fileName)
        {
            return FileMap.FirstOrDefault(f => f.Value == fileName).Key;
        }
        protected string PackageContentDirectory;
        public string GetContent(string fileName)
        {
            string includedPath = Path.Combine(PackageContentDirectory, PACKAGE_INCLUDED_FILES_DIRECTORY);
            return Path.Combine(includedPath, fileName);
        }

        public abstract void Load(string packageContentDirectory, string packageResultFilePath);
        public abstract void LoadBody(string packageContentDirectory, string packageResultFilePath);
        public void Clean()
        {
            try
            {
                Directory.Delete(PackageContentDirectory, true);
            }
// ReSharper disable EmptyGeneralCatchClause
            catch { }
// ReSharper restore EmptyGeneralCatchClause
        }

        protected Dictionary<string, string> FileMap = new Dictionary<string, string>();

        public event Action<ExecutionProgressInfo> OnProgress;
        protected void OnProgressHandler(ExecutionProgressInfo obj)
        {
            if (OnProgress != null)
            {
                OnProgress(obj);
            }
        }

        public event Action<QueryCancelEventArgs> OnQueryCancel;
        protected void OnQueryCancelHandler(QueryCancelEventArgs cancel)
        {
            if (OnQueryCancel != null)
            {
                OnQueryCancel(cancel);
            }
        }

        protected string GenerateContentFileName(string sourcePath)
        {
            if (sourcePath.EndsWith(@"\"))
            {
                sourcePath = sourcePath.Remove(sourcePath.Length - 1);
            }
            string result = Path.GetFileName(sourcePath);
            if (FileMap.ContainsValue(result))
            {
                int seq = 1;
                while (true)
                {
                    string ext = Path.GetExtension(result);
                    string name = Path.GetFileNameWithoutExtension(result);
                    result = name + "." + seq + ext;
                    if (!FileMap.ContainsValue(result))
                    {
                        break;
                    }
                    seq++;
                }
            }
            return result;
        }

        public static PackageContent Create(PackageContentType type)
        {
            switch (type)
            {
                case PackageContentType.Packed:
                    return new PackageContentPacked();
                case PackageContentType.Empty:
                    return new PackageContentEmpty();
                case PackageContentType.Direct:
                    return new PackageContentDirect();
                default:
                    return new PackageContentDirect();
            }
        }

        public static PackageContentType GetContentType(string filePath)
        {
            FileStream fs = null;
            int firstByte;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                firstByte = sr.Read();
            }
            catch 
            {
                return PackageContentType.Wrong;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
            if (firstByte == 60)
            {
                PackageBody body = GinSerializer.Deserialize<PackageBody>(filePath);
                return body.ContentType;
            }
            if (firstByte == 31)
            {
                return PackageContentType.Packed;
            }
            return PackageContentType.Wrong;
        }
    }
}
