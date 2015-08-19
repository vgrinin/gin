using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using Gin.Logging;

namespace Gin.Util.GZip
{
    public class GZipReader
    {

        public const string MODULE_NAME = "GZip reader";

        public const int TOTAL_PROGRESS_COST = 100;
        private int previousProgress = 0;

        private Stream inStream;
        private BinaryReader inStreamReader;
        private byte[] buffer = new byte[1024];

        private Stream readStream;

        public GZipReader(Stream readStream)
        {
            inStream = new GZipStream(readStream, CompressionMode.Decompress);
            inStreamReader = new BinaryReader(inStream);
            this.readStream = readStream;
        }

        public event Action<ExecutionProgressInfo> OnProgress;

        private void OnProgressHandler(ExecutionProgressInfo obj)
        {
            if (OnProgress != null)
            {
                OnProgress(obj);
            }
        }

        public void ReadToEnd(string destDirectory)
        {
            ReadToEnd(destDirectory, null);
        }

        public void ReadToEnd(string destDirectory, string searchFileName)
        {
            while (true)
            {
                try
                {
                    string name = inStreamReader.ReadString();
                    bool isFile = inStreamReader.ReadBoolean();
                    long count = inStreamReader.ReadInt64();

                    bool needLoad = searchFileName == null || searchFileName == name;

                    long fileSizeProgress = count;
                    string fileName = Path.Combine(destDirectory, name);
                    string dirName = Path.GetDirectoryName(fileName);
                    if (!Directory.Exists(dirName))
                    {
                        Directory.CreateDirectory(dirName);
                    }
                    int currentProgress = 0;
                    if (isFile)
                    {
                        // создаем файл
                        Stream outStream = null;
                        if (needLoad)
                        {
                            outStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                        }
                        while (count > 0)
                        {
                            int bytesToRead = count < buffer.Length ? (int)count : buffer.Length;
                            int bytesRead = inStreamReader.Read(buffer, 0, bytesToRead);
                            if (bytesRead <= 0)
                            {
                                break;
                            }
                            if (needLoad)
                            {
                                outStream.Write(buffer, 0, bytesRead);
                            }
                            count -= bytesRead;
                        }
                        if (needLoad)
                        {
                            outStream.Flush();
                            outStream.Close();
                            if (searchFileName != null)
                            {
                                break;
                            }
                        }
                        currentProgress = (int)Math.Round((double)(((double)readStream.Position / readStream.Length) * TOTAL_PROGRESS_COST));
                    }
                    else
                    {
                        // создаем папку
                        Directory.CreateDirectory(fileName);
                        currentProgress = 0;
                    }
                    OnProgressHandler(new ExecutionProgressInfo()
                    {
                        Message = "Прочитан файл " + name + "(" + fileSizeProgress + ")",
                        ProgressCost = currentProgress - previousProgress,
                        ModuleName = MODULE_NAME
                    });
                    previousProgress = currentProgress;
                }
                catch (EndOfStreamException)
                {
                    break;
                }
            }
            OnProgressHandler(new ExecutionProgressInfo()
            {
                Message = "Чтение архива завершено",
                ProgressCost = TOTAL_PROGRESS_COST - previousProgress,
                ModuleName = MODULE_NAME
            });
        }

    }
}
