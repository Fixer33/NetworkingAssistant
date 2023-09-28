using TdLib;

namespace NetworkingAssistant
{
    internal static class OperationBuffer
    {
        public static TdApi.Chat SelectedChat { get; private set; }

        public static void SelectChat(TdApi.Chat chat)
        {
            if (chat == null)
            {
                Console.WriteLine("Trying to select not existing chat. Ignoring command");
                return;
            }

            SelectedChat = chat;
            Console.WriteLine("Selected chat " + chat.Id);
        }
    }
}
