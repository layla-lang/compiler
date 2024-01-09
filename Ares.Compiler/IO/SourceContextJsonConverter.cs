using Ares.Compiler.Analysis;
using Newtonsoft.Json;

namespace Ares.Compiler.IO;

public class SourceContextJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var sc = (SourceContext)value;
        var j = SourceContextJsonUtils.ToJson(sc);
        
        var thisIndex = serializer.Converters.IndexOf(this);
        serializer.Converters.RemoveAt(thisIndex);

        serializer.Serialize(writer, j);
                
        serializer.Converters.Insert(thisIndex, this);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new InvalidOperationException("Unable to read JSON into SourceContext.");
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(SourceContext);
    }

}