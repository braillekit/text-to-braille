using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace BrailleToolkit.Helpers
{
    /// <summary>
    /// 為 YamlDotNet 提供 BrailleCell 型別的自訂序列化和反序列化邏輯。
    /// </summary>
    public class BrailleCellYamlTypeConverter : IYamlTypeConverter
    {
        /// <summary>
        /// 判斷此轉換器是否可以處理指定的型別。
        /// </summary>
        public bool Accepts(Type type)
        {
            return type == typeof(BrailleCell);
        }

        /// <summary>
        /// 從 YAML 讀取物件。
        /// </summary>
        public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            // Expected YAML: { Value: 17 } or Value: 17 (inline mapping)
            
            parser.Consume<MappingStart>();
            
            byte value = 0;
            
            while (parser.TryConsume<Scalar>(out var key))
            {
                if (key.Value == "Value")
                {
                    var valueScalar = parser.Consume<Scalar>();
                    value = byte.Parse(valueScalar.Value);
                }
                else
                {
                    // Ignore other keys if any, but consume the value
                    parser.SkipThisAndNestedEvents();
                }
            }

            parser.Consume<MappingEnd>();

            return BrailleCell.GetInstance(value);
        }

        /// <summary>
        /// 將物件寫入 YAML。
        /// </summary>
        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            var cell = (BrailleCell)value;
            
            emitter.Emit(new MappingStart(null, null, false, MappingStyle.Flow));
            emitter.Emit(new Scalar("Value"));
            emitter.Emit(new Scalar(cell.Value.ToString()));
            emitter.Emit(new MappingEnd());
        }
    }
}
