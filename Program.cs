using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using StrikerBot.API;
using StrikerBot.Bot;
using StrikerBot.Database.Base;

namespace StrikerBot
{
    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        public static async Task MainAsync()
        {
            DatabaseDriver.Initialize(Vars.MongoDBUri, Vars.MongoDBDatabase);

            var botBuilder = Host.CreateDefaultBuilder();
            botBuilder.ConfigureServices(BotStartup.ConfigureServices);

            _ = botBuilder.Build().RunAsync();

            var apiBuilder = WebHost.CreateDefaultBuilder();
            apiBuilder.UseStartup<APIStartup>();
#if !DEBUG
            apiBuilder.UseUrls("http://0.0.0.0:8000");
#endif
            _ = apiBuilder.Build().RunAsync();

            await Task.Delay(-1);
        }
    }
}