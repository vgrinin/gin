using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Gin.Util
{
    public static class IOUtil
    {
        public static void CopyDirectory(string Src, string Dst, bool Move)
        {
            CopyDirectory(Src, Dst, Move, null);
        }

        public static void CopyDirectory(string Src, string Dst, bool Move, Action<string> progress)
        {

            if (Dst[Dst.Length - 1] != Path.DirectorySeparatorChar)
            {
                Dst += Path.DirectorySeparatorChar;
            }

            if (!Directory.Exists(Dst))
            {
                Directory.CreateDirectory(Dst);
            }

            String[] Files;
            Files = Directory.GetFileSystemEntries(Src);
            foreach (string Element in Files)
            {
                if (Directory.Exists(Element))
                {
                    CopyDirectory(Element, Dst + Path.GetFileName(Element), Move, progress);
                }
                else
                {
                    string DestPath = Dst + Path.GetFileName(Element);
                    if (DestPath != Element)
                    {

                        if (Move)
                        {
                            if (File.Exists(DestPath))
                            {
                                File.Delete(DestPath);
                            }
                            File.Move(Element, DestPath);
                        }
                        else
                        {
                            File.Copy(Element, DestPath, true);
                        }
                    }
                    if (progress != null)
                    {
                        progress(Path.GetFileName(Element));
                    }
                }
            }
        }

        public static void IterateDirectory(string Src, Action<string> action)
        {
            String[] Files;

            Files = Directory.GetFileSystemEntries(Src);
            foreach (string Element in Files)
            {
                action(Element);
                if (Directory.Exists(Element))
                {
                    IterateDirectory(Element, action);
                }
            }
        }

        public static string GetParentDirectory(string sourceFilePath)
        {
            if (sourceFilePath.EndsWith(@"\"))
            {
                sourceFilePath = sourceFilePath.Remove(sourceFilePath.Length - 1);
            }
            DirectoryInfo sourceParentDirectory = Directory.GetParent(sourceFilePath);
            return sourceParentDirectory.FullName;
        }

        public static string ReadFile(string filePath)
        {
            StreamReader streamReader = null;
            string stringData;
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Open);
                streamReader = new StreamReader(fs);
                stringData = streamReader.ReadToEnd();
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Close();
                }
            }
            return stringData;
        }

        public static void WriteFile(string filePath, string stringData)
        {
            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                streamWriter = new StreamWriter(fileStream);
                streamWriter.Write(stringData);
                streamWriter.Close();
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
                if (streamWriter != null)
                {
                    streamWriter.Close();
                }
            }
        }

        public static void TryDeleteFile(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
            catch
            { }
        }
    }
}
