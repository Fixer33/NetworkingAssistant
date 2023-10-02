using TdLib;

namespace NetworkingAssistant
{
    internal static class OperationBuffer
    {
        public static TdApi.Chat SelectedChat { get; private set; }
        public static List<TdApi.Message> SelectedMessages { get; private set; }
        public static List<string> SelectedStrings { get; private set; }

        public static bool SelectChat(TdApi.Chat chat)
        {
            if (chat == null)
            {
                Console.WriteLine("Trying to select not existing chat. Ignoring command");
                return false;
            }

            SelectedChat = chat;
            Console.WriteLine("Selected chat " + chat.Id);
            return true;
        }

        public static bool SelectMessages(TdApi.Message[] messages)
        {
            if (messages == null)
            {
                Console.WriteLine("Not valid message array to select");
                return false;
            }

            SelectedMessages = messages.ToList();
            return true;
        }

        public static bool SelectMessages(List<TdApi.Message> messages)
        {
            if (messages == null)
            {
                Console.WriteLine("Not valid message list to select");
                return false;
            }

            SelectedMessages = messages;
            return true;
        }

        public static bool SelectStrings(string[] strings)
        {
            if (strings == null)
            {
                Console.WriteLine("Not valid message array to select");
                return false;
            }

            SelectedStrings = strings.ToList();
            return true;
        }

        public static bool SelectStrings(List<string> strings)
        {
            if (strings == null)
            {
                Console.WriteLine("Not valid message list to select");
                return false;
            }

            SelectedStrings = strings;
            return true;
        }
    }
}
