using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Gin.Util.GZip
{
    public class GZipWriter: IDisposable
    {
        private Stream outStream;
        private BinaryWriter outStreamWriter;
        private byte[] buffer = new byte[1024];

        public GZipWriter(Stream writeStream)
        {
            outStream = new GZipStream(writeStream, CompressionMode.Compress);
            outStreamWriter = new BinaryWriter(outStream);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion

        public void Close()
        {
            outStreamWriter.Flush();
            outStreamWriter.Close();
        }

        public void Write(FileStream data, string name)
        {
            bool isFile = data != null;
            long count = isFile ? data.Length : 0;
            outStreamWriter.Write(name);
            outStreamWriter.Write(isFile);
            outStreamWriter.Write(count);
            while (count > 0)
            {
                int bytesRead = data.Read(buffer, 0, buffer.Length);
                if (bytesRead <= 0)
                {
                    break;
                }
                outStream.Write(buffer, 0, bytesRead);
                count -= bytesRead;
            }
        }
    }
}
