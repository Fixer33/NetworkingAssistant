using NetworkingAssistant.Properties;
using OpenAI_API;
using OpenAI_API.Chat;

namespace NetworkingAssistant
{
    internal static class AI
    {
        public static bool Initialized { get; private set; }

        private static OpenAIAPI _api;
        private static Conversation _conversation;

        public static void Initialize()
        {
            _api = new OpenAIAPI(new APIAuthentication(Config.GPTKey));
            Initialized = true;
        }

        public static async Task<string> GenerateQuestions(string prePrompt = null)
        {
            if (_conversation == null)
            {
                _conversation = _api.Chat.CreateConversation();
            }

            _conversation.AppendUserInput(Resources.openai_initial);

            if (prePrompt == null)
            {
                _conversation.AppendUserInput("Additionally: " + prePrompt);
            }

            return await _conversation.GetResponseFromChatbotAsync();
        }
    }
}
