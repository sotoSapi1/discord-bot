using MongoDB.Driver;
using System.Reflection;

namespace StrikerBot.Database.Base
{
    public class DatabaseDriver
    {
        private static MongoClient client;
        private static IMongoDatabase database;

        public static void Initialize(string connectionString, string mainDatabase)
        {
            Console.WriteLine("Bootstrapping database.");

            client = new MongoClient(connectionString);
            database = client.GetDatabase(mainDatabase);

            if (database == null)
            {
                throw new Exception($"Could not find {mainDatabase} database.");
            }

            InitializeChildren();

            Console.WriteLine("Bootstrapping database done.");
        }
            
        private static void InitializeChildren()
        {
            const string entrypoint = nameof(Initialize);

            Type[] databaseClasses = Assembly.GetExecutingAssembly().GetExportedTypes().Where(x =>
            {
                return x.IsClass && x.BaseType == typeof(DatabaseDriver);
            }).ToArray();


            foreach (Type type in databaseClasses)
            {
                MethodInfo methodInfo = type.GetMethod(entrypoint, BindingFlags.Static | BindingFlags.NonPublic);

                if (methodInfo != null && methodInfo.GetParameters().Count() == 0)
                {
                    methodInfo.Invoke(null, null);
                }
            }
        }

        protected static MongoClient GetClient()
        {
            return client;
        }

        protected static IMongoDatabase GetMainDatabase()
        {
            return database;
        }

        protected static IMongoCollection<T>? GetCollection<T>(string collectionName)
        {
            var collection = database.GetCollection<T>(collectionName);
            return collection;
        }

        protected static IMongoCollection<T> TryGetCollection<T>(string collectionName)
        {
            var collection = GetCollection<T>(collectionName);

            if (collection == null)
            {
                database.CreateCollection(collectionName);
                return GetCollection<T>(collectionName);
            }

            return collection;
        }
    }
}
