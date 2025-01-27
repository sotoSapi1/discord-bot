using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Mscc.GenerativeAI;
using StrikerBot.Bot.Services.Handler;
using StrikerBot.Database;
using StrikerBot.Database.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikerBot.Bot.Services
{
    internal class RetardService : IHostedService
    {
        private static RetardService instance;

        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly CommandService _command;
        private readonly IMemoryCache _memoryCache;

        private static List<RetardRole> retardRoles = new();

        public RetardService(DiscordSocketClient client, CommandService commands, IMemoryCache memoryCache, IServiceProvider services)
        {
            _client = client;
            _command = commands;
            _memoryCache = memoryCache;
            _services = services;

            instance = this;

            retardRoles = BadRoleDatabase.GetRetardRolesOrderByTier();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _command.AddModuleAsync<PrefixCommands>(_services);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }

        private class PrefixCommands : ModuleBase<SocketCommandContext>
        {
            [Command("pingbadroles")]
            public async Task PingBadRoles([Remainder] string userMsg = "hahahahah")
            {
                string outMsg = "";

                foreach (var v in retardRoles)
                {
                    outMsg += v.ToSocketRole(Context.Guild).Mention + " ";
                }

                _ = ReplyAsync(outMsg + userMsg);
                _ = Context.Message.DeleteAsync();
            }

            [Command("retardleaderboard")]
            public async Task RetardLeadernoard()
            {
                string outMsg = "";

                int leaderboardPlace = 1;
                foreach (var v in BotUserDatabase.GetTop10Retards())
                {
                    outMsg += $"#{leaderboardPlace} <@{v.userId}>: {v.retardScore} votes" + "\n";
                    leaderboardPlace++;
                }

                _ = ReplyAsync(outMsg);
            }

            [RequireUserPermission(GuildPermission.Administrator)]
            [Command("deleteretardrole")]
            public async Task DeleteRetardRole(SocketRole role)
            {
                if (role == null)
                {
                    _ = ReplyAsync("Syntax: role-mention: role");
                    return;
                }

                BadRoleDatabase.TryDeleteRetardRole(role);
                retardRoles = BadRoleDatabase.GetRetardRolesOrderByTier();

                _ = ReplyAsync($"{role.Mention} removed");
            }

            [RequireUserPermission(GuildPermission.Administrator)]
            [Command("addretardrole")]
            public async Task AddRetardRole(SocketRole role, byte tier = 0)
            {
                if (role == null || tier == 0)
                {
                    _ = ReplyAsync("Syntax: role-mention: role, number: tier");
                    return;
                }

                BadRoleDatabase.TryUpdateRetardRoleTier(role, tier);
                retardRoles = BadRoleDatabase.GetRetardRolesOrderByTier();

                _ = ReplyAsync($"{role.Mention} added");
            }

            [Command("voteretard")]
            public async Task VoteRetard(SocketGuildUser target = null)
            {
                if (target == null)
                {
                    _ = ReplyAsync("You mentioned nobody. Type it correctly.");
                    return;
                }

                string userAlreadyVotedTargetKey = target.Id.ToString() + Context.User.Id.ToString();

                if (instance._memoryCache.TryGetValue(userAlreadyVotedTargetKey, out _))
                {
                    _ = ReplyAsync($"You've already given a vote to {target.Mention}.");
                    return;
                }
                else
                {
                    instance._memoryCache.Set(userAlreadyVotedTargetKey, true);
                }

                BotUser user = BotUserDatabase.UpdateRetardScore(target.Id);

                int retardModulo = user.retardScore % 5;
                int retardTier = user.retardScore / 5;

                if (retardTier < retardRoles.Count)
                {
                    if (retardModulo > 0 && retardModulo < 5)
                    {
                        SocketRole role = retardRoles.ElementAt(retardTier).ToSocketRole(Context.Guild);
                        _ = ReplyAsync($"{5 - retardModulo} more vote(s) for {target.Mention}, I'll give them {role.Mention} role.");
                    }
                    else if (retardModulo <= 0)
                    {
                        SocketRole role = retardRoles.ElementAt(retardTier - 1).ToSocketRole(Context.Guild);

                        await target.AddRoleAsync(role);
                        _ = ReplyAsync($"{target.Mention} I gave you {role.Mention} role buddy!");
                    }
                }
                else
                {
                    _ = ReplyAsync($"{target.Mention} now has {user.retardScore} retard votes.");
                }
            }
        }
    }
}
