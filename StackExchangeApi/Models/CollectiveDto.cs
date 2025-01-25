using System.Text.Json.Serialization;

namespace StackExchangeApi.Models
{
    public class CollectiveDto
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }

        [JsonPropertyName("external_links")]
        public ICollection<ExternalLinkDto> ExternalLinks { get; set; }
    }
}
