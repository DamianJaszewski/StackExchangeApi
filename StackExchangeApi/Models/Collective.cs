using System.Text.Json.Serialization;

namespace StackExchangeApi.Models
{
    public class Collective
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public List<string> Tags { get; set; }

        [JsonIgnore]
        public Item Item { get; set; } = null!;
        public ICollection<ExternalLink> ExternalLinks { get; set; } = new List<ExternalLink>();
    }
}
