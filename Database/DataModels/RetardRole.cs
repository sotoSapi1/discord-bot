using Discord.WebSocket;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace StrikerBot.Database.DataModels
{
    public class RetardRole(SocketRole role, byte tier)
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; init; }

        [BsonElement("role-id")]
        public ulong roleId { get; init; } = role.Id;

        [BsonElement("retard-tier")]
        public byte tier { get; init; } = tier;

        public SocketRole ToSocketRole(SocketGuild guild)
        {
            return guild.GetRole(roleId);
        }
    }
}