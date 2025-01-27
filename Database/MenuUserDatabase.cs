using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using StrikerBot.Database.Base;
using StrikerBot.Database.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikerBot.Database
{
    public class MenuUserDatabase : DatabaseDriver
    {
        private const string databaseName = "user_data";
        private static IMongoCollection<MenuUser> usersCollection;

        private static void Initialize()
        {
            usersCollection = TryGetCollection<MenuUser>(databaseName);

            if (usersCollection == null)
            {
                throw new Exception($"Could not find \"{databaseName}\" collection.");
            }
        }

        public static async Task<string> CreateNewUser()
        {
            string token = Guid.NewGuid().ToString();
            await usersCollection.InsertOneAsync(new MenuUser(token));

            return token;
        }

        public static async Task ResetUserIP(MenuUser user)
        {
            var userQuery = usersCollection.Find(x => x.token == user.token);
            var userUpdate = Builders<MenuUser>.Update.Set(x => x.ip, "");
            await usersCollection.UpdateOneAsync(userQuery.Filter, userUpdate);
        }

        public static async Task UpdateUserIP(MenuUser user, string newIp)
        {
            var userQuery = usersCollection.Find(x => x.token == user.token);
            var userUpdate = Builders<MenuUser>.Update.Set(x => x.ip, newIp);
            await usersCollection.UpdateOneAsync(userQuery.Filter, userUpdate);
        }

        public static async Task<DeleteResult> DeleteUser(string token)
        {
            var findFilter = Builders<MenuUser>.Filter.Eq(x => x.token, token);
            return await usersCollection.DeleteOneAsync(findFilter);
        }

        public static async Task<DeleteResult> DeleteUser(MenuUser user)
        {
            var findFilter = Builders<MenuUser>.Filter.Eq(x => x.token, user.token);
            return await usersCollection.DeleteOneAsync(findFilter);
        }

        public static async Task<MenuUser> FindUser(string token)
        {
            var findFilter = Builders<MenuUser>.Filter.Eq(x => x.token, token);
            return await usersCollection.Find(findFilter).FirstOrDefaultAsync();
        }
    }
}
