﻿using NetworkingAssistant.Commands;
using System.Text.RegularExpressions;

namespace NetworkingAssistant
{
    internal static class CommandHandler
    {
        private static List<CommandBase> _commands = new List<CommandBase>()
        {
            new ParameterlessCommand(CommandId.Stop),
            new ParameterlessCommand(CommandId.Help),

            new IntegerParameterCommand(CommandId.GetChats),
            new IntegerParameterCommand(CommandId.SelectChat),

            new StringParameterCommand(CommandId.ExportQuestions),
        };


        public static void HandleCommand(string command)
        {
            Match rMatch;

            for (int i = 0; i < _commands.Count; i++)
            {
                rMatch = Regex.Match(command, _commands[i].GetRegexPattern());
                if (rMatch.Success)
                {
                    _commands[i].HandleCommand(rMatch);
                    return;
                }
            }

            Console.WriteLine("No such command presented");
        }  

        public static void PrintHelp()
        {
            for (int i = 0; i < _commands.Count; i++)
            {
                Console.WriteLine(_commands[i].GetCommandHint());
            }
        }
    }
}