
using MongoDB.Bson.Serialization.Attributes;

namespace Magneto.AzureFunctions.Stat
{
    [BsonIgnoreExtraElements]
    public class Human
    {
        public string pk { get; set; }
        public string[]? Dna { get; set; }
    }
}
