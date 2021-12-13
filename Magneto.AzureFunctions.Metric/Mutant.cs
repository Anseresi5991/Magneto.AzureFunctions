using MongoDB.Bson.Serialization.Attributes;
namespace Magneto.AzureFunctions.Metrics
{
    [BsonIgnoreExtraElements]
    public class Mutant : Human
    {
    }
}
