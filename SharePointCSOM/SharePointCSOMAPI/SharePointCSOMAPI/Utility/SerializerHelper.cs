using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SharePointCSOMAPI
{
    public static class SerializerHelper
    {
        public static T DeserializeObjectFromString<T>(string input) where T : class
        {
            using (var stream = new StringReader(input))
            using (var reader = new XmlTextReader(stream))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(reader) as T;
            }
        }

        public static T DeserializeObjectFromStream<T>(Stream input) where T : class
        {
            using (var reader = new XmlTextReader(input))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(reader) as T;
            }
        }

        public static byte[] SerializeObjectToBytes<T>(T targetOject) where T : class
        {
            using (var stream = new MemoryStream())
            using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, targetOject);
                var bytes = stream.ToArray();
                return bytes;
            }
        }

        public static string SerializeObjectToString<T>(T targetOject) where T : class
        {
            var bytes = SerializeObjectToBytes(targetOject);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

    }
}
