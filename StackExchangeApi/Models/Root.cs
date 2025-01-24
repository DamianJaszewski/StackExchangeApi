using System.Text.Json.Serialization;

namespace StackExchangeApi.Models
{
    public class Root
    {
        public int Id { get; set; } 

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        [JsonPropertyName("quota_max")]
        public int QuotaMax { get; set; }

        [JsonPropertyName("quota_remaining")]
        public int QuotaRemaining { get; set; }

        [JsonPropertyName("items")]
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
