using System.Text.Json.Serialization;

namespace StackExchangeApi.Models
{
    public class ExternalLink
    {
        public int Id { get; set; }
        public int CollectiveId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonIgnore]
        public Collective Collective { get; set; } = null!;
    }
}
