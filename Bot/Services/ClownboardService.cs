using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;

namespace StrikerBot.Bot.Services
{
    internal class ClownboardService : IHostedService
    {
        private static ClownboardService instance;

        private const string wantedReactionEmoji = "🤡";
        private const int neededReactionCount = 5;
        private const string wantedClownboardChannelName = "clownboard";

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        private Dictionary<IGuild, ITextChannel> clownboardChannels;

        public ClownboardService(DiscordSocketClient client, CommandService commands, IServiceProvider services)
        {
            _client = client;
            _commands = commands;
            _services = services;

            instance = this;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.Ready += OnReady;
            _client.ReactionAdded += HandleDiscordReaction;

            await _commands.AddModuleAsync<PrefixCommands>(_services);
        }


        private async Task OnReady()
        {
            clownboardChannels = InitClownboardChannels();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }


        private Dictionary<IGuild, ITextChannel> InitClownboardChannels()
        {
            Dictionary<IGuild, ITextChannel> clownboardChannels = new();

            foreach (var guild in _client.Guilds)
            {
                clownboardChannels.Add(guild, guild.TextChannels.Where(x => x.Name == wantedClownboardChannelName).First());
            }

            return clownboardChannels;
        }

        private async Task<IUserMessage?> SendMessageToClownboardAsync(IUserMessage message)
        {
            Emoji emoji = new Emoji(wantedReactionEmoji);
            Embed clownEmbed = BuildClownEmbed(message);

            IGuild messageGuild = (message.Channel as SocketGuildChannel).Guild;

            ITextChannel clownboard = clownboardChannels[messageGuild];

            IUserMessage embedMessage = await clownboard.SendMessageAsync(text: message.GetJumpUrl(), embed: clownEmbed);
            _ = embedMessage.AddReactionAsync(emoji);

            return embedMessage;
        }

        private static Embed BuildClownEmbed(IUserMessage message)
        {            
            EmbedBuilder mainEmbedBuilder = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder()
                {
                    Name = message.Author.Username,
                    IconUrl = message.Author.GetAvatarUrl()
                },
                Description = message.Content,
                Color = Color.Red,
            };

            if(message.Attachments.Count > 0)
            {
                mainEmbedBuilder.ImageUrl = message.Attachments.First().Url;
            }

            return mainEmbedBuilder.Build();
        }

        private async Task HandleDiscordReaction(Cacheable<IUserMessage, ulong> messageCache, Cacheable<IMessageChannel, ulong> channelCache,
                                                SocketReaction reaction)
        {
            IUserMessage currentMessage = await messageCache.GetOrDownloadAsync();

            if (currentMessage.Author.IsBot) return;
            var wantedReactions = currentMessage.Reactions.Where(x => x.Key.Name == wantedReactionEmoji).ToList();

            if (wantedReactions.Count >= neededReactionCount)
            {
                IUserMessage? embedMessage = await SendMessageToClownboardAsync(currentMessage);
                _ = currentMessage.ReplyAsync(embedMessage.GetJumpUrl());
            }
        }

        private class PrefixCommands : ModuleBase<SocketCommandContext>
        {
            [RequireUserPermission(GuildPermission.BanMembers)]
            [Command("clownboard")]
            public async Task Clownboard()
            {
                IUserMessage refMessage = Context.Message.ReferencedMessage;

                if (refMessage == null) return;
                IUserMessage? embedMessage = await instance.SendMessageToClownboardAsync(refMessage);

                _ = refMessage.ReplyAsync(embedMessage.GetJumpUrl());
            }
        }
    }
}
