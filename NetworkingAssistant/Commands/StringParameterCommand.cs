using System.Text.RegularExpressions;

namespace NetworkingAssistant.Commands
{
    internal class StringParameterCommand : CommandBase
    {
        public StringParameterCommand(CommandId id) : base(id)
        {
        }

        public override string GetCommandHint()
        {
            return Id switch
            {
                CommandId.ExportQuestions => "export_questions *file_name* - Exports all questions from selected chat into file. No extension needed",

                _ => "",
            };
        }

        public override string GetRegexPattern()
        {
            return Id switch
            {
                CommandId.ExportQuestions => @"^export_questions ([\d\w]+)$",

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
            }
        }

        private static async void ExportQuestions(string fileName)
        {

        }
    }
}
