using System.Text.Json.Serialization;

namespace StackExchangeApi.Models
{
    public class ExternalLink
    {
        public int Id { get; set; }
        public int CollectiveId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;

        [JsonIgnore]
        public Collective Collective { get; set; } = null!;
    }
}
