using MongoDB.Driver;
using StrikerBot.Database.Base;
using StrikerBot.Database.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikerBot.Database
{
    public class BotUserDatabase : DatabaseDriver
    {
        private const string databaseName = "bot_user_data";
        private static IMongoCollection<BotUser> usersCollection;

        static void Initialize()
        {
            usersCollection = TryGetCollection<BotUser>(databaseName);

            if (usersCollection == null)
            {
                throw new Exception($"Could not find \"{databaseName}\" collection.");
            }
        }

        public static BotUser GetUserDocument(ulong userId)
        {
            var userFilter = Builders<BotUser>.Filter.Eq(x => x.userId, userId);
            var userSearch = usersCollection.Find(userFilter);

            var user = userSearch.FirstOrDefault();

            if (user == null)
            {
                usersCollection.InsertOne(new BotUser(userId));
                user = userSearch.FirstOrDefault();
            }

            return user;
        }

        public static void UpdateUserDocument(ulong userId, UpdateDefinition<BotUser> userUpdate)
        {
            var userFilter = Builders<BotUser>.Filter.Eq(x => x.userId, userId);
            var userSearch = usersCollection.Find(userFilter);

            var user = userSearch.FirstOrDefault();

            if (user == null)
            {
                usersCollection.InsertOne(new BotUser(userId));
            }

            usersCollection.UpdateOne(userFilter, userUpdate);
        }

        public static BotUser UpdateRngRoll(ulong userId)
        {
            var oldUser = GetUserDocument(userId);
            UpdateUserDocument(userId, Builders<BotUser>.Update.Set(x => x.rngRoll, oldUser.rngRoll + 1));

            return GetUserDocument(userId);
        }

        public static BotUser UpdateRetardScore(ulong userId)
        {
            var oldUser = GetUserDocument(userId);
            UpdateUserDocument(userId, Builders<BotUser>.Update.Set(x => x.retardScore, oldUser.retardScore + 1));

            return GetUserDocument(userId);
        }

        //public static int GetRngRoll(ulong userId)
        //{
        //    var user = GetUserDocument(userId);

        //    return user.rngRoll;
        //}

        public static List<BotUser> GetTop10Retards()
        {
            return usersCollection.AsQueryable().OrderByDescending(x => x.retardScore).Take(10).ToList();
        }
    }
}
