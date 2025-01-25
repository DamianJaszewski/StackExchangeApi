using System.Text.Json.Serialization;

namespace StackExchangeApi.Models
{
    public class ExternalLink
    {
        public int Id { get; set; }
        public int CollectiveId { get; set; }
        public string Type { get; set; }
        public string Link { get; set; }

        [JsonIgnore]
        public Collective Collective { get; set; } = null!;
    }
}
