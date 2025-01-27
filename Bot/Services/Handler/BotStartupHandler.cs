using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using StrikerBot.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StrikerBot.Bot.Services.Handler
{
    public class BotStartupHandler : IHostedService
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;

        public BotStartupHandler(DiscordSocketClient client, IServiceProvider services)
        {
            _client = client;
            _services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.Log += msg => LogHelper.OnLogAsync(msg);

            await _client.LoginAsync(Discord.TokenType.Bot, Vars.BotToken);
            await _client.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.LogoutAsync();
            await _client.StopAsync();
        }
    }
}
