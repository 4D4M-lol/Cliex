using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cliex
{
    public class JsonTokenConverter : JsonConverter<Token>
    {
        public override Token Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Token value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            JsonSerializer.Serialize(writer, value.Type, options);
            writer.WritePropertyName("Value");
        
            if (value.Value is Dictionary<Token, Token> dictionary)
            {
                writer.WriteStartObject();
                
                foreach (KeyValuePair<Token,Token> kvp in dictionary)
                {
                    string keyString = kvp.Key.Value?.ToString() ?? "null";
                    
                    writer.WritePropertyName(keyString);
                    JsonSerializer.Serialize(writer, kvp.Value, options);
                }
                
                writer.WriteEndObject();
            }
            else JsonSerializer.Serialize(writer, value.Value, options);
        
            writer.WriteEndObject();
        }
    }
}