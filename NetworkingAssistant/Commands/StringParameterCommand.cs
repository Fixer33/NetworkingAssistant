using Newtonsoft.Json;
using System.Text.RegularExpressions;
using static TdLib.TdApi;

namespace NetworkingAssistant.Commands
{
    public class StringParameterCommand : CommandBase
    {
        public StringParameterCommand(CommandId id) : base(id)
        {
        }

        public override string GetCommandHint()
        {
            return Id switch
            {
                CommandId.ExportQuestions => "export_questions *file_name* - Exports all questions from selected chat into file. No extension needed",
                CommandId.SendRegistrationPoll => "send_reg_poll *headline* - Send poll for registration. *headline* will be displayed on poll header. Shows created poll id",
                CommandId.SendTextToRegisteredUsersInPoll => "send_text_to_poll_users *text* - Send *text* to all users who voted first two options in selected registration poll",

                _ => "",
            };
        }

        public override string GetRegexPattern()
        {
            return Id switch
            {
                CommandId.ExportQuestions => @"^export_questions ([\d\w]+)$",
                CommandId.SendRegistrationPoll => @"^send_reg_poll (.+)$",
                CommandId.SendTextToRegisteredUsersInPoll => @"^send_text_to_poll_users (.+)$",

                _ => "",
            };
        }

        public override void HandleCommand(Match match)
        {
            Console.WriteLine();
            Console.WriteLine(new string('-', 100));
            Console.WriteLine();
            if (match.Groups.Count != 2)
            {
                Console.WriteLine("Wrong parameter count for command " + Id.ToString());
                Console.WriteLine();
                Console.WriteLine(new string('-', 100));
                Console.WriteLine();
                return;
            }

            string parameter = match.Groups[1].Value;

            switch (Id)
            {
                case CommandId.ExportQuestions:
                    ExportQuestions(parameter);
                    break;
                case CommandId.SendRegistrationPoll:
                    SendRegistrationPoll(parameter);
                    break;
                case CommandId.SendTextToRegisteredUsersInPoll:
                    SendTextToUsersFromRegPoll(parameter);
                    break;
            }
            Console.WriteLine();
            Console.WriteLine(new string('-', 100));
            Console.WriteLine();
        }

        private static async void ExportQuestions(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                Console.WriteLine($"File {fileName}.json already exists!");
                return;
            }

            if (OperationBuffer.SelectedMessages == null || OperationBuffer.SelectedMessages.Count < 1)
            {
                Console.WriteLine("No messages selected");
                return;
            }

            System.IO.File.Create(fileName + ".json").Close();

            List<QuestionList.QuestionData> questions = new();
            var messages = OperationBuffer.SelectedMessages;
            Match match;
            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i].Content is not MessageContent.MessageText)
                    continue;

                match = Regex.Match(((MessageContent.MessageText)messages[i].Content).Text.Text, @"[\s\S]+1\.([\s\S]+)[\s\S]+2\.([\s\S]+)3\.([\s\S]+\?)[\s\S]+");
                if (match.Success == false || match.Groups.Count < 4)
                {
                    continue;
                }

                questions.Add(new QuestionList.QuestionData
                {
                    Data = DateTimeOffset.FromUnixTimeSeconds(messages[i].Date).ToString("dd.MM.yyyy_HH:mm:ss"),
                    Question1 = match.Groups[1].Value.Replace("\n", ""),
                    Question2 = match.Groups[2].Value.Replace("\n", ""), 
                    Question3 = match.Groups[3].Value.Replace("\n", ""),
                });
            }

            var list = new QuestionList()
            {
                Questions = questions.ToArray(),
            };

            System.IO.File.WriteAllText(fileName + ".json", JsonConvert.SerializeObject(list));
            Console.WriteLine("Questions exported to " + fileName + ".json");
        }

        private static void SendRegistrationPoll(string question)
        {
            if (OperationBuffer.SelectedChat == null)
            {
                Console.WriteLine("No chat selected");
                return;
            }

            var msg = TelegramManager.SendRegistrationPoll('`' + question);

            Console.WriteLine("Poll created with id " + msg.Result.Id);
        }

        private static async void SendTextToUsersFromRegPoll(string text)
        {
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
                    await TelegramManager.SendTextToUser(users.UserIds[i], text);
                    voterSentCount++;
                }

                users = await TelegramManager.GetPollVoters(OperationBuffer.SelectedChat.Id, OperationBuffer.SelectedPollMessage.Id, 1);
                for (int i = 0; i < users.UserIds.Length; i++)
                {
                    await TelegramManager.SendTextToUser(users.UserIds[i], text);
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

        [Serializable]
        public struct QuestionList
        {
            public QuestionData[] Questions;

            [Serializable]
            public struct QuestionData
            {
                public string Data;
                public string Question1;
                public string Question2;
                public string Question3;
            }
        }
    }
}
