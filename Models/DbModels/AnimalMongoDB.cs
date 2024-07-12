using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ZooArcadia.API.Models.DbModels
{
    public class AnimalMongoDB : Animal
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int clickcount { get; set; }
    }
}
