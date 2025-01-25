using System.Text.Json.Serialization;

namespace StackExchangeApi.Models
{
    public class Collective
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public List<string> Tags { get; set; }

        [JsonIgnore]
        public Item Item { get; set; } = null!;
        public ICollection<ExternalLink> ExternalLinks { get; set; } = new List<ExternalLink>();
    }
}
