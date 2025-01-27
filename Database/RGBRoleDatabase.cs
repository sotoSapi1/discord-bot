using Discord.WebSocket;
using MongoDB.Driver;
using Mscc.GenerativeAI;
using StrikerBot.Database.Base;
using StrikerBot.Database.DataModels;

namespace StrikerBot.Database
{
    public class RGBRoleDatabase : DatabaseDriver
    {
        private const string databaseName = "rgb_roles";
        private static IMongoCollection<RGBRole> rolesCollection;

        private static void Initialize()
        {
            rolesCollection = TryGetCollection<RGBRole>(databaseName);

            if (rolesCollection == null)
            {
                throw new Exception($"Could not find \"{databaseName}\" collection.");
            }
        }

        public static void TryDeleteRole(SocketRole role)
        {
            var filter = Builders<RGBRole>.Filter.Eq(x => x.roleId, role.Id);
            var search = rolesCollection.Find(filter);

            var roleData = search.FirstOrDefault();

            if (roleData != null)
            {
                rolesCollection.DeleteOne(filter);
                return;
            }
        }

        public static void TryAddRole(SocketRole role, SocketGuild guild)
        {
            var filter = Builders<RGBRole>.Filter.Eq(x => x.roleId, role.Id);
            var search = rolesCollection.Find(filter);

            var roleData = search.FirstOrDefault();

            if (roleData == null)
            {
                rolesCollection.InsertOne(new RGBRole(role, guild));
                return;
            }
        }

        public static List<RGBRole> GetRoles()
        {
            var list = rolesCollection.AsQueryable().ToList();
            return list != null ? list : new();
        }
    }
}
