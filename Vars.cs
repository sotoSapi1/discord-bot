using Discord.WebSocket;

namespace StrikerBot
{
    public static class Vars
    {
        public static string MongoDBUri { get; } = Environment.GetEnvironmentVariable("MONGO_DB_URI");
        public static string MongoDBDatabase { get; } = Environment.GetEnvironmentVariable("BOT_DB");

        public static string OpenRouterKey { get; } = Environment.GetEnvironmentVariable("OPEN_ROUTER_KEY");
        public static string GooleAIKey { get; } = Environment.GetEnvironmentVariable("GOOGLE_AI_KEY");

#if !DEBUG
        public static string BotToken  get; } = Environment.GetEnvironmentVariable("BOT_TOKEN_DEBUG");
#else
        public static string BotToken { get; } = Environment.GetEnvironmentVariable("BOT_TOKEN");
#endif

        public const string ModelAIHost = "https://openrouter.ai/api/v1/";
        public const string DefaultAIModel = "meta-llama/llama-3.1-8b-instruct:free";
        public const string DefaultNickname = "bot";
        public const string DefaultAIPersona = "the guy";
        public const string DefaultAvatarUrl = "https://i.ibb.co.com/DVrBNCS/bot-sewer.png";

        public const bool CanSendBanCopypasta = true;
    }
}