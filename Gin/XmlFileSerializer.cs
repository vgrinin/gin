using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Reflection;

namespace Gin
{

    public class XmlFileSerializer<T>
    {

        public void Serialize(T obj, Type[] types, string filePath)
        {
            FileStream stream = null;
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(T), types);
                stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                ser.Serialize(stream, obj);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();
                }
            }
        }

        public T Deserialize(string filePath, Type[] types)
        {
            T result = default(T);
            FileStream stream = null;
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(T), types);
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                result = (T)ser.Deserialize(stream);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return result;
        }

    }
}
