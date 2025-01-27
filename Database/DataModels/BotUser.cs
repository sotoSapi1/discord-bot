using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikerBot.Database.DataModels
{
    public class BotUser(ulong userId)
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; init; }

        [BsonElement("user-id")]
        public ulong userId { get; set; } = userId;

        [BsonElement("retard-score")]
        public int retardScore { get; init; }

        [BsonElement("rng-roll")]
        public int rngRoll { get; init; }
    }
}
