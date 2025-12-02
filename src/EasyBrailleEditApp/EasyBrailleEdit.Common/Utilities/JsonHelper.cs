using System.Runtime.Serialization.Json;
using System.Text;

namespace EasyBrailleEdit.Common.Utilities
{
    /// <summary>
    /// JSON 序列化/反序列化輔助類別。
    /// 注意：不可將 DataContractJsonSerializer 改為 System.Text.Json，否則會引發一連串的序列化／反序列化的問題。
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 將物件序列化為 JSON 字串。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj!.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.UTF8.GetString(ms.ToArray());
            ms.Dispose();
            return retVal;
        }

        /// <summary>
        /// 將 JSON 字串反序列化為物件。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string jsonStr)
        {
            T obj = Activator.CreateInstance<T>()!;  // 注意: 欲反序列化的類別必須有預設建構元.
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms)!;
                return obj;
            }
        }
    }
}
