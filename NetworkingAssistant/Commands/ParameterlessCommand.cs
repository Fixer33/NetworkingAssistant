using System.Text.RegularExpressions;
using static TdLib.TdApi;

namespace NetworkingAssistant.Commands
{
    internal class ParameterlessCommand : CommandBase
    {
        public ParameterlessCommand(CommandId id) : base(id)
        {
        }

        public override string GetCommandHint()
        {
            return Id switch
            {
                CommandId.Stop => "stop - Stops client and exits application",
                CommandId.Help => @"help - Get list and description of all available commands",
                CommandId.ShowSelectedChatInfo => @"show_selected_chat_info - Get info about selected chat",
                CommandId.SelectAllMessages => @"select_all_messages$ - Select all messages from selected chat. Will print out the amount",
                CommandId.LeaveOnlyQuestions => @"leave_only_questions$ - Leave only questions in message buffer",

                _ => "",
            };
        }

        public override string GetRegexPattern()
        {
            return Id switch
            {
                CommandId.Stop => @"^stop$",
                CommandId.Help => @"^help$",
                CommandId.ShowSelectedChatInfo => @"^show_selected_chat_info$",
                CommandId.SelectAllMessages => @"^select_all_messages$",
                CommandId.LeaveOnlyQuestions => @"^leave_only_questions$",

                _ => "",
            };
        }

        public override void HandleCommand(Match match)
        {
            switch (Id)
            {
                case CommandId.Stop:
                    StopCommand();
                    break;
                case CommandId.Help:
                    Console.WriteLine("\nAll commands:");
                    CommandHandler.PrintHelp();
                    Console.WriteLine();
                    break;
                case CommandId.ShowSelectedChatInfo:
                    ShowSelectedChatInfo();
                    break;
                case CommandId.SelectAllMessages:
                    SelectAllMessages();
                    break;
                case CommandId.LeaveOnlyQuestions:
                    LeaveOnlyQuestions();
                    break;
            }

        }

        private static void StopCommand()
        {
            Program.Stop();
        }

        private static void ShowSelectedChatInfo()
        {
            var chat = OperationBuffer.SelectedChat;

            if (chat == null)
            {
                Console.WriteLine("No chat selected");
                return;
            }

            Console.WriteLine("Selected chat info:");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(chat.Id);
            Console.ResetColor();
            Console.WriteLine(" : " + chat.Title);
        }

        private static async void SelectAllMessages()
        {
            var chat = OperationBuffer.SelectedChat;

            if (chat == null)
            {
                Console.WriteLine("No chat selected");
                return;
            }

            var messageCollection = await TelegramManager.GetMessages(chat.Id, 100, 0);

            if (messageCollection == null)
            {
                Console.WriteLine("Could not load messages for selected chat. Try selecting it again");
                return;
            }

            if (OperationBuffer.SelectMessages(messageCollection.Messages_))
            {
                Console.WriteLine($"Selected {messageCollection.TotalCount} messages");

                //for (int i = 0; i < messageCollection.Messages_.Length; i++)
                //{
                //    var msg = messageCollection.Messages_[i].Content as MessageContent.MessageText;
                //    if (msg == null)
                //        continue;
                //    Console.WriteLine($"{messageCollection.Messages_[i].SenderId} : {msg.Text.Text}");
                //}
            }
        }

        private static void LeaveOnlyQuestions()
        {
            var messages = OperationBuffer.SelectedMessages;

            if (messages == null)
            {
                Console.WriteLine("No messages selected");
                return;
            }

            messages = messages.Where(i => 
                i.Content is MessageContent.MessageText && 
                Regex.Match(((MessageContent.MessageText)i.Content).Text.Text, @"[\s\S]+1\.([\s\S]+)[\s\S]+2\.([\s\S]+)3\.([\s\S]+\?)[\s\S]+").Success)
                .ToList();

            OperationBuffer.SelectMessages(messages);
            Console.WriteLine($"Selected {messages.Count} messages");
        }
    }
}
