using Discord.WebSocket;
using MongoDB.Driver;
using StrikerBot.Database.Base;
using StrikerBot.Database.DataModels;

namespace StrikerBot.Database
{
    public class BadRoleDatabase : DatabaseDriver
    {
        private const string databaseName = "retard_roles";
        private static IMongoCollection<RetardRole> retardCollection;

        private static void Initialize()
        {
            retardCollection = TryGetCollection<RetardRole>(databaseName);

            if (retardCollection == null)
            {
                throw new Exception($"Could not find \"{databaseName}\" collection.");
            }
        }

        public static void TryDeleteRetardRole(SocketRole role)
        {
            var filter = Builders<RetardRole>.Filter.Eq(x => x.roleId, role.Id);
            var search = retardCollection.Find(filter);

            var retardRole = search.FirstOrDefault();

            if (retardRole != null)
            {
                retardCollection.DeleteOne(filter);
                return;
            }
        }

        public static void TryUpdateRetardRoleTier(SocketRole role, byte tier)
        {
            var filter = Builders<RetardRole>.Filter.Eq(x => x.tier, role.Id);
            var search = retardCollection.Find(filter);

            var retardRole = search.FirstOrDefault();

            if (retardRole == null)
            {
                retardCollection.InsertOne(new RetardRole(role, tier));
                return;
            }

            retardCollection.UpdateOne(filter, Builders<RetardRole>.Update.Set(x => x.tier, tier));
        }

        public static List<RetardRole> GetRetardRolesOrderByTier()
        {
            return retardCollection.AsQueryable().OrderBy(x => x.tier).ToList();
        }
    }
}
