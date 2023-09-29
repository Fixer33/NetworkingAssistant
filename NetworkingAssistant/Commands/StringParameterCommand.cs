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

                _ => "",
            };
        }

        public override string GetRegexPattern()
        {
            return Id switch
            {
                CommandId.ExportQuestions => @"^export_questions ([\d\w]+)$",
                CommandId.SendRegistrationPoll => @"^send_reg_poll (.+)$",

                _ => "",
            };
        }

        public override void HandleCommand(Match match)
        {
            if (match.Groups.Count != 2)
            {
                Console.WriteLine("Wrong parameter count for command " + Id.ToString());
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
            }
        }

        private static async void ExportQuestions(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                Console.WriteLine($"File {fileName}.list already exists!");
                return;
            }

            if (OperationBuffer.SelectedMessages.Count < 1)
            {
                Console.WriteLine("No messages selected");
                return;
            }

            System.IO.File.Create(fileName + ".list").Close();

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

            System.IO.File.WriteAllText(fileName + ".list", JsonConvert.SerializeObject(list));
            Console.WriteLine("Questions exported to " + fileName + ".list");
        }

        private static void SendRegistrationPoll(string question)
        {
            if (OperationBuffer.SelectedChat == null)
            {
                Console.WriteLine("No chat selected");
                return;
            }

            TelegramManager.SendRegistrationPoll(question);

            Console.WriteLine("Poll created");
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
