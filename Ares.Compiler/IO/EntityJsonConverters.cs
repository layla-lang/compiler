using Ares.Compiler.Analysis.Entities;
using Newtonsoft.Json;

namespace Ares.Compiler.IO;

public class EntityJsonConverters
{
    /*
    public class TypeEntityConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var sc = (TypeEntity)value;
            
            var thisIndex = serializer.Converters.IndexOf(this);
            serializer.Converters.RemoveAt(thisIndex);
            serializer.Serialize(writer, value);
            serializer.Converters.Insert(thisIndex, this);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new InvalidOperationException("Unable to read JSON into TypeEntity.");
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(TypeEntity);

    }
    */
}