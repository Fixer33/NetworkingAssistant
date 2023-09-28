using System.Text.RegularExpressions;

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

                _ => "",
            };
        }

        public override string GetRegexPattern()
        {
            return Id switch
            {
                CommandId.Stop => @"^stop$",
                CommandId.Help => @"^help$",

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
            }

        }

        private static void StopCommand()
        {
            Program.Stop();
        }
    }
}
