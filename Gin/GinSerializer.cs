
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin;
using Gin.Commands;
using Gin.Transactions;
using System.Xml.Serialization;


namespace Gin
{

    public static class GinSerializer
    {

        private static Type[] _types = new Type[0];

        public static void IncludeTypes(Type[] types)
        {
            _types = types;
        }

        public static void Serialize<T>(T obj, string filePath)
        {
            XmlFileSerializer<T> serializer = new XmlFileSerializer<T>();
            serializer.Serialize(obj, _types, filePath);
        }

        public static T Deserialize<T>(string filePath)
        {
            XmlFileSerializer<T> serializer = new XmlFileSerializer<T>();
            return serializer.Deserialize(filePath, _types);
        }

    }
}
