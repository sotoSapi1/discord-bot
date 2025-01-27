using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace StrikerBot.Bot.Services
{
    internal class PGUtilService : IHostedService
    {


        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        private const int PROJECT_ID = 196500;
        private const int MERCHANT_ID = 319917;

        private readonly HttpClient client = new();

        public PGUtilService(DiscordSocketClient client, CommandService commands, IServiceProvider services)
        {
            _client = client;
            _commands = commands;
            _services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _commands.AddModuleAsync<PrefixCommands>(_services);
        }

        private async Task<string> GetToken(string id)
        {
            var payload = JsonConvert.SerializeObject(new
            {
                settings = new
                {
                    projectId = PROJECT_ID,
                    merchantId = MERCHANT_ID
                },
                loginId = "ee2f78e4-2f53-4a29-b26f-c4911cacb2ab",
                webhookUrl = "https://offerwall-currency.lightmap.com/xsolla/login",
                user = new
                {
                    id,
                    country = "US"
                },
                isUserIdFromWebhook = false
            });

            var response = await client.PostAsync(
                "https://sb-user-id-service.xsolla.com/api/v1/user-id",
                new StringContent(payload, Encoding.UTF8, "application/json")
            );

            var responseString = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);

            return jsonResponse.token;
        }

        private async Task<string> GetUsernameById(string id)
        {
            string encodedToken = await GetToken(id);
            JwtSecurityToken token = new(encodedToken);
            return token.Claims.FirstOrDefault(x => x.Type == "nickname").Value;
        }

        private async Task<bool> IsBanned(string id)
        {
            FormUrlEncodedContent payload = new(new Dictionary<string, string>
            {
                {"type_device", "1"},
                {"id", "{'" + id + "'}"}
            });

            var response = await client.PostAsync(
                "https://secure.pixelgunserver.com/pixelgun3d-config/getBanList.php",
                payload
            );

            return await response.Content.ReadAsStringAsync() == "1";
        }

        private async Task<bool> SendDailyChest(string id)
        {
            string token = await GetToken(id);
            
            var response = await client.PostAsync(
                $"https://store.xsolla.com/api/v2/project/{PROJECT_ID}/free/item/121027",
                null
            );

            dynamic json = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
            return json?.order_id != null;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }

        private class PrefixCommands : ModuleBase<SocketCommandContext>
        {
            [Command("checkban")]
            public async Task Checkban(string id)
            {
                try
                {
                    string username = await GetUsernameById(id);
                    bool banned = await IsBanned(id);

                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    {
                        Title = "Player info",
                        Description =
                        $"- **ID:** {id}\n" +
                        $"- **Username:** {username}\n" +
                        $"- **Is Banned:** {Convert.ToString(banned)}",

                        Color = Color.Purple,
                    };

                    _ = Context.Message.ReplyAsync(embed: embedBuilder.Build());
                }
                catch (Exception err)
                {
                    _ = Context.Message.ReplyAsync($"kill yourself ({err.GetType().Name})");
                }
            }

            [Command("claimdailychest")]
            public async Task ClaimDailyChest(string id)
            {
                try
                {
                    bool success = await SendDailyChest(id);

                    if (success)
                    {
                        _ = Context.Message.ReplyAsync("Sent! 👍");
                    }
                    else
                    {
                        _ = Context.Message.ReplyAsync("User already claimed daily store chest or an error occured.");
                    }
                }
                catch (Exception err) 
                {
                    _ = Context.Message.ReplyAsync($"kill yourself ({err.GetType().Name})");
                }
            }
        }
    }
}
