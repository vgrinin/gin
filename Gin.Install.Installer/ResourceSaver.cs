using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Gin.Install.Installer
{
    internal static class ResourceSaver
    {
        public static void SaveResource(byte[] resource, string filePath)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
                stream.Write(resource, 0, resource.Length);
                stream.Flush();
                files.Add(filePath);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        public static void SaveResource(string resource, string filePath)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(resource); 
                writer.Flush();
                files.Add(filePath);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        public static void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
            dirs.Add(path);
        }

        public static void TryDeleteDirectory(string path)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch
            { }
        }

        public static void TryDeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch
            { }
        }

        private static List<string> files = new List<string>();
        private static List<string> dirs = new List<string>();

        public static void Clean()
        {
            foreach (string file in files)
            {
                TryDeleteFile(file);
            }

            foreach (string dir in dirs)
            {
                TryDeleteDirectory(dir);
            }

        }
    }
}
