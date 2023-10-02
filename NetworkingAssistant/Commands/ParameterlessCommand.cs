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
                CommandId.SelectAllMessages => @"select_all_messages - Select all messages from selected chat. Will print out the amount. You should repeat this command a few times until number of selected messages stops changing",
                CommandId.LeaveOnlyQuestions => @"leave_only_questions - Leave only questions in message buffer",
                CommandId.GenerateQuestions => @"generate_questions - Generate messages in chatgpt. Places them into string buffer",

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
                CommandId.GenerateQuestions => @"^generate_questions$",

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
                case CommandId.GenerateQuestions:
                    GenerateQuestions();
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

            List<Message> messages = new();
            bool cycle = true;
            int currentCycles = 0;
            const int maxCycles = 100;
            Messages messageCollection = await TelegramManager.GetMessages(chat.Id);
            if (messageCollection == null || messageCollection.Messages_ == null)
            {
                Console.WriteLine("Error, try again.");
                return;
            }
            messages.AddRange(messageCollection.Messages_);
            while (cycle)
            {
                messageCollection = await TelegramManager.GetMessages(chat.Id, messages[^1].Id);

                if (messageCollection != null && messageCollection.Messages_ != null)
                {
                    messages.AddRange(messageCollection.Messages_);

                    if (messageCollection.TotalCount < 100)
                    {
                        cycle = false;
                    }
                }
                else
                {
                    await Task.Delay(300);
                }

                if (++currentCycles > maxCycles)
                {
                    Console.WriteLine("Cycle limit exceeded. Breaking cycle");
                    cycle = false;
                }
            }

            if (OperationBuffer.SelectMessages(messages))
            {
                Console.WriteLine($"Selected {messages.Count} messages");

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

        private static async void GenerateQuestions()
        {
            Console.WriteLine("Sending generation request...");

            string response = null;
            try
            {
                response = await AI.GenerateQuestions();
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch
            {
                Console.WriteLine("Failed to get response from gpt");
                return;
            }

            Console.WriteLine(response);
        }
    }
}
