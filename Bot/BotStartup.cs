using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;
using Discord.Interactions;
using StrikerBot.Bot.Services;
using StrikerBot.Bot.Services.Handler;

namespace StrikerBot.Bot
{
    class BotStartup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("Bootstraping Bot.");

            var botClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All
            });

            services.AddMemoryCache();

            services.AddSingleton(x => botClient);
            services.AddSingleton<CommandService>();
            services.AddSingleton<InteractionService>();

            services.AddHostedService<PrefixHandler>();
            services.AddHostedService<AIService>();
            services.AddHostedService<RetardService>();
            services.AddHostedService<ClownboardService>();
            services.AddHostedService<PGUtilService>();
            services.AddHostedService<RGBRoleService>();
            services.AddHostedService<BotStartupHandler>();
        }
    }
}
