using System.Text.Json.Serialization;

namespace StackExchangeApi.Models
{
    public class ExternalLinkDto
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }
    }
}
