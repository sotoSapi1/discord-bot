using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Net;

namespace StrikerBot.Database.DataModels
{
    public class MenuUser(string token)
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; init; }

        [BsonElement("token")]
        public string token { get; init; } = token;

        [BsonElement("ip")]
        public string ip { get; init; } = "";
    }
}
