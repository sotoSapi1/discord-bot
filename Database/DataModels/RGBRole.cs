using Discord.WebSocket;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikerBot.Database.DataModels
{
    public class RGBRole(SocketRole role, SocketGuild guild)
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; init; }

        [BsonElement("role-id")]
        public ulong roleId { get; init; } = role.Id;

        [BsonElement("guild-id")]
        public ulong guildId { get; init; } = guild.Id;

        public SocketRole ToSocketRole(DiscordSocketClient client)
        {
            return client.GetGuild(guildId).GetRole(roleId);
        }
    }
}
