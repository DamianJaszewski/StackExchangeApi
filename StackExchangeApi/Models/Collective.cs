using System.Text.Json.Serialization;

namespace StackExchangeApi.Models
{
    public class Collective
    {
        public int Id { get; set; }
        public int ItemId { get; set; }

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

        [JsonIgnore]
        public Item Item { get; set; } = null!;

        [JsonPropertyName("external_links")]
        public ICollection<ExternalLink> ExternalLinks { get; set; } = new List<ExternalLink>();
    }
}
