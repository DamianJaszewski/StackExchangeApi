using System.Text.Json.Serialization;

namespace StackExchangeApi.Models
{
    public class Item
    {
        public int Id { get; set; }
        public bool HasSynonyms { get; set; }
        public bool IsModeratorOnly { get; set; }
        public bool IsRequired { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
        public List<Collective> Collectives { get; set; } = new List<Collective>();
    }
}
