using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace StrikerBot.Bot.Services.Handler
{
    public class PrefixHandler : IHostedService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        public PrefixHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services)
        {
            _commands = commands;
            _client = client;
            _services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.MessageReceived += HandleCommandAsync;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            SocketGuildUser socketGuildUser = message.Author as SocketGuildUser;

            if (!(message.HasCharPrefix('!', ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            var context = new SocketCommandContext(_client, message);

            await _commands.ExecuteAsync(
                context: context, 
                argPos: argPos, 
                services: null
            );
        }
    }
}
