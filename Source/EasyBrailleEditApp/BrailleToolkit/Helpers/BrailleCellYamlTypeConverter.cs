using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace BrailleToolkit.Helpers
{
    public class BrailleCellYamlTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(BrailleCell);
        }

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

        public void WriteYaml(IEmitter emitter, object value, Type type, ObjectSerializer serializer)
        {
            var cell = (BrailleCell)value;
            
            emitter.Emit(new MappingStart(null, null, false, MappingStyle.Flow));
            emitter.Emit(new Scalar("Value"));
            emitter.Emit(new Scalar(cell.Value.ToString()));
            emitter.Emit(new MappingEnd());
        }
    }
}
