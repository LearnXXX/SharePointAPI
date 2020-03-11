using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointGraph
{
    class CustomSerializer : ISerializer
    {
        private JsonSerializerSettings jsonSerializerSettings;


        public CustomSerializer() 
        {
            JsonSerializerSettings settings1 = new JsonSerializerSettings();
            settings1.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            settings1.TypeNameHandling = TypeNameHandling.None;
            settings1.DateParseHandling = DateParseHandling.None;
        }

        public CustomSerializer(JsonSerializerSettings jsonSerializerSettings)
        {
            this.jsonSerializerSettings = jsonSerializerSettings;
        }

        public T DeserializeObject<T>(Stream stream)
        {
            T local;
            if (stream == null) return default(T);
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, true, 0x1000, true))
            {
                using (JsonTextReader reader2 = new JsonTextReader(reader))
                {
                    local = JsonSerializer.Create(this.jsonSerializerSettings).Deserialize<T>(reader2);
                }
            }
            return local;

        }

        public T DeserializeObject<T>(string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return default(T);
            return JsonConvert.DeserializeObject<T>(System.IO.File.ReadAllText(@"C:\Users\xluo\Desktop\a.txt"));
            return JsonConvert.DeserializeObject<T>(inputString, this.jsonSerializerSettings);

        }

        public string SerializeObject(object serializeableObject)
        {
            if (serializeableObject == null) return null;
            Stream stream = serializeableObject as Stream;
            if (stream != null)
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            string str = serializeableObject as string;
            if (str != null) return str;
            return JsonConvert.SerializeObject(serializeableObject, this.jsonSerializerSettings);

        }
    }
}
