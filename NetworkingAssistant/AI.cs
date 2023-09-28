using OpenAI_API;

namespace NetworkingAssistant
{
    internal static class AI
    {
        public static bool Initialized { get; private set; }

        private static OpenAIAPI _api;

        public static void Initialize()
        {
            _api = new OpenAIAPI(new APIAuthentication(Config.GPTKey));
            Initialized = true;
        }


    }
}
