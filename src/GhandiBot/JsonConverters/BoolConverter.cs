using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GhandiBot.JsonConverters
{
    public class BoolConverter : JsonConverter<bool>
    {
        private readonly ICollection<string> _trueVals = new[]{"true", "yes", "1"};
        private readonly ICollection<string> _falseVals = new[] {"false", "no", "0"};
        
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string val = reader.GetString().ToLower();

            if (_trueVals.Contains(val))
            {
                return true;
            }

            if (_falseVals.Contains(val))
            {
                return false;
            }
            
            throw new JsonException($"'{val}' could not be converted to bool");
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}