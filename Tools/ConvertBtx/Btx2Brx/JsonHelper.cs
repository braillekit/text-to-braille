using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Huanlin.Helpers
{
    public class JsonHelper
    {
        public static string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.UTF8.GetString(ms.ToArray());
            ms.Dispose();
            return retVal;
        }

        public static T Deserialize<T>(string jsonStr)
        {
            T obj = Activator.CreateInstance<T>();  // 注意: 欲反序列化的類別必須有預設建構元.
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);
                return obj;
            }            
        }
    }
}
