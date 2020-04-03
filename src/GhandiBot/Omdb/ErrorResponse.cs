using System.Text.Json.Serialization;
using GhandiBot.JsonConverters;

namespace GhandiBot.Omdb
{
    public class ErrorResponse
    {
        [JsonConverter(typeof(BoolConverter))]
        public bool Response { get; set; }
        public string Error { get; set; }
    }
}