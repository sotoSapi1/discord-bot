using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using StrikerBot.Database;
using StrikerBot.Database.DataModels;

namespace StrikerBot.Bot.Services
{
    internal class RGBRoleService : BotUserDatabase, IHostedService
    {
        private static RGBRoleService instance;

        private CancellationToken _cancellationToken;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        private Color _currentColor = Color.DarkGrey;

        private int colorIndex = 0;
        private Color[] colors = {
            Color.Red,
            Color.Green,
            Color.Blue
        };

        private List<RGBRole> rolesToUpdate = new();

        public RGBRoleService(DiscordSocketClient client, CommandService commands, IServiceProvider services)
        {
            _client = client;
            _commands = commands;
            _services = services;

            instance = this;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            rolesToUpdate = RGBRoleDatabase.GetRoles();
            await _commands.AddModuleAsync<PrefixCommands>(_services);
            _cancellationToken = cancellationToken;
            _client.Ready += OnReady;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }

        private async Task UpdateRGBRole()
        {
            while (true)
            {
                await Task.Delay(864000);
                if (rolesToUpdate.Count == 0) continue;

                _currentColor = colors[colorIndex % colors.Length];
                colorIndex++;

                foreach (var role in rolesToUpdate)
                {
                    SocketRole socketRole = role.ToSocketRole(_client);
                    _ = socketRole.ModifyAsync(x =>
                    {
                        x.Color = _currentColor;
                    });
                }
            }
        }

        private async Task OnReady()
        {
            _ = UpdateRGBRole();
        }

        private class PrefixCommands : ModuleBase<SocketCommandContext>
        {
            [RequireUserPermission(GuildPermission.Administrator)]
            [Command("setrgb")]
            public async Task SetRGB(SocketRole role)
            {
                RGBRoleDatabase.TryAddRole(role, Context.Guild);
                instance.rolesToUpdate = RGBRoleDatabase.GetRoles();

                _ = ReplyAsync($"{role.Mention} is now rgb! 👍");
            }

            [RequireUserPermission(GuildPermission.Administrator)]
            [Command("deletergb")]
            public async Task DeleteRGB(SocketRole role)
            {
                RGBRoleDatabase.TryDeleteRole(role);
                instance.rolesToUpdate = RGBRoleDatabase.GetRoles();

                _ = ReplyAsync($"{role.Mention} is no longer rgb.");
            }
        }
    }
}
