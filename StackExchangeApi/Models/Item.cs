using System.Text.Json.Serialization;

namespace StackExchangeApi.Models
{
    public class Item
    {
        public int Id { get; set; }
        public int RootId { get; set; }

        [JsonPropertyName("has_synonyms")]
        public bool HasSynonyms { get; set; }

        [JsonPropertyName("is_moderator_only")]
        public bool IsModeratorOnly { get; set; }

        [JsonPropertyName("is_required")]
        public bool IsRequired { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public Root Root { get; set; } = null!;

        [JsonPropertyName("collectives")]
        public ICollection<Collective> Collectives { get; set; } = new List<Collective>();
    }
}
