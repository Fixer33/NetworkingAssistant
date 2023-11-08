using Newtonsoft.Json;
using System.Text.RegularExpressions;
using static NetworkingAssistant.TableFormer;
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
                CommandId.SelectLastRegPoll => @"select_last_reg_poll - Selects last registration poll in selected messages",
                CommandId.ImportPersonFile => @"import_person_file - Selects last registration poll in selected messages",
                CommandId.MovePeople => @"move_people - Selects last registration poll in selected messages",
                CommandId.PrintTableOrder => @"print_table_order - Selects last registration poll in selected messages",
                CommandId.SendTextToPollUsers => @"send_text_to_poll_users - Sends selected formatted text to poll users",
                CommandId.SelectLastMessageText => @"select_last_message_text - Selects formatted text from last selected message",

                _ => "",
            };
        }

        public override string GetRegexPattern()
        {
            return "^" + Id switch
            {
                CommandId.Stop => @"stop",
                CommandId.Help => @"help",
                CommandId.ShowSelectedChatInfo => @"show_selected_chat_info",
                CommandId.SelectAllMessages => @"select_all_messages",
                CommandId.LeaveOnlyQuestions => @"leave_only_questions",
                CommandId.GenerateQuestions => @"generate_questions",
                CommandId.SelectLastRegPoll => @"select_last_reg_poll",
                CommandId.ImportPersonFile => @"import_person_file",
                CommandId.MovePeople => @"move_people",
                CommandId.PrintTableOrder => @"print_table_order",
                CommandId.SendTextToPollUsers => @"send_text_to_poll_users",
                CommandId.SelectLastMessageText => @"select_last_message_text",

                _ => "",
            } + "$";
        }

        public override void HandleCommand(Match match)
        {
            Console.WriteLine();
            Console.WriteLine(new string('-', 100));
            Console.WriteLine();
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
                case CommandId.SelectLastRegPoll:
                    SelectLastRegPoll();
                    break;
                case CommandId.ImportPersonFile:
                    ImportPersonFile();
                    break;
                case CommandId.MovePeople:
                    MovePeople();
                    break;
                case CommandId.PrintTableOrder:
                    PrintTableOrder();
                    break;
                case CommandId.SelectLastMessageText:
                    SelectLastMessageText();
                    break;
                case CommandId.SendTextToPollUsers:
                    SendSelectedTextToPollUsers();
                    break;
            }
            Console.WriteLine();
            Console.WriteLine(new string('-', 100));
            Console.WriteLine();
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

        private static void SelectLastRegPoll()
        {
            var messages = OperationBuffer.SelectedMessages;
            if (messages == null || messages.Count <= 0)
            {
                Console.WriteLine("No messages selected");
                return;
            }

            MessageContent.MessagePoll poll = null;
            Message pollMessage = null;
            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i].Content is MessageContent.MessagePoll)
                {
                    poll = messages[i].Content as MessageContent.MessagePoll;
                    if (poll == null || poll.Poll == null || poll.Poll.Options.Length != 4 || poll.Poll.Question.StartsWith('Р') == false)
                    {
                        poll = null;
                        continue;
                    }

                    pollMessage = messages[i];
                    break;
                }
            }

            if (poll == null || pollMessage == null)
            {
                Console.WriteLine("No poll with such id found");
                return;
            }

            if (OperationBuffer.SelectPoll(poll, pollMessage))
            {
                Console.WriteLine($"Selected poll with {poll.Poll.TotalVoterCount} voters and question {poll.Poll.Question}");
            }
            else
            {
                Console.WriteLine("No poll existing");
            }


        }

        private static void ImportPersonFile()
        {
            const string filename = "people.json";

            if (System.IO.File.Exists(filename))
            {
                PersonArray array;
                try
                {
                    array = JsonConvert.DeserializeObject<PersonArray>(System.IO.File.ReadAllText(filename));
                    List<Person> people = new();
                    for (int i = 0; i < array.People.Count; i++)
                    {
                        people.Add(new Person(array.People[i].Name, array.People[i].Coefficient));
                    }

                    OperationBuffer.TableForming.SetPeople(people);
                    Console.WriteLine($"Imported {people.Count} people");
                    return;
                }
                catch
                {
                    Console.WriteLine("Corrupted file!");
                }
            }

            System.IO.File.Create(filename).Close();
            System.IO.File.WriteAllText(filename, JsonConvert.SerializeObject(new PersonArray()
            {
                People = new List<SerializablePerson>()
                    {
                        new SerializablePerson()
                        {
                            Name = "P1",
                            Coefficient = 0,
                        },
                        new SerializablePerson()
                        {
                            Name = "P2",
                            Coefficient = 0,
                        }
                    }
            }));
            Console.WriteLine("Created new " + filename);
        }

        private static void MovePeople()
        {
            if (OperationBuffer.TableForming.TablePositions == null)
            {
                if (OperationBuffer.TableForming.TableCount < 2)
                {
                    Console.WriteLine("Table count is too low to create new order. Set table amount first!");
                    return;
                }

                if (OperationBuffer.TableForming.People.Count < 2)
                {
                    Console.WriteLine("People count is too low to create new order. Import people file first!");
                    return;
                }

                Console.WriteLine("Creating new order...");
                OperationBuffer.TableForming.SetTablePositions(
                    GetPositions(OperationBuffer.TableForming.TableCount, OperationBuffer.TableForming.People).result);
                Console.WriteLine("New order created");
            }
            else
            {
                OperationBuffer.TableForming.TablePositions.MoveLayers();
                Console.WriteLine("Moved people across tables");
            }
        }

        private static void PrintTableOrder()
        {
            var order = OperationBuffer.TableForming.TablePositions;
            if (order == null)
            {
                Console.WriteLine("No table order to print!");
                return;
            }

            for (int i = 0; i < order.TableCount; i++)
            {
                Console.WriteLine();
                Console.Write($"{i + 1}. ");

                var people = order.GetTablePeople(i);
                for (int j = 0; j < people.Count; j++)
                {
                    if (people[j] == null)
                    {
                        continue;
                    }

                    if (j >= people.Count - 1)
                    {
                        Console.Write($"{people[j].Name}");
                    }
                    else
                    {
                        Console.Write($"{people[j].Name}, ");
                    }
                }
            }
        }

        private static void SelectLastMessageText()
        {
            if (OperationBuffer.SelectedMessages == null || OperationBuffer.SelectedMessages.Count <= 0)
            {
                Console.WriteLine("No messages selected");
                return;
            }

            var messages = OperationBuffer.SelectedMessages;
            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i].Content is MessageContent.MessageText text)
                {
                    if (text == null)
                    {
                        continue;
                    }

                    OperationBuffer.SelectText(text.Text);
                    Console.WriteLine("Formatted text selected");
                    break;
                }
            }
        }

        private static async void SendSelectedTextToPollUsers()
        {
            if (OperationBuffer.SelectedText == null)
            {
                Console.WriteLine("No text selected");
                return;
            }

            if (OperationBuffer.SelectedPoll == null || OperationBuffer.SelectedPollMessage == null)
            {
                Console.WriteLine("No poll selected");
                return;
            }

            if (OperationBuffer.SelectedChat == null)
            {
                Console.WriteLine("No chat selected");
                return;
            }

            int voterSentCount = 0;

            try
            {
                var users = await TelegramManager.GetPollVoters(OperationBuffer.SelectedChat.Id, OperationBuffer.SelectedPollMessage.Id, 0);
                for (int i = 0; i < users.UserIds.Length; i++)
                {
                    await TelegramManager.SendTextToUser(users.UserIds[i], OperationBuffer.SelectedText);
                    voterSentCount++;
                }

                users = await TelegramManager.GetPollVoters(OperationBuffer.SelectedChat.Id, OperationBuffer.SelectedPollMessage.Id, 1);
                for (int i = 0; i < users.UserIds.Length; i++)
                {
                    await TelegramManager.SendTextToUser(users.UserIds[i], OperationBuffer.SelectedText);
                    voterSentCount++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error appeared in sending messages! Amount of successfull sends: {voterSentCount}. After that got error: {e.Message}");
                return;
            }
            Console.WriteLine($"Messages were sent to {voterSentCount} users");
        }
    }
}
