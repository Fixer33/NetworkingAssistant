﻿using System.Text.RegularExpressions;

namespace NetworkingAssistant.Commands
{
    internal abstract class CommandBase
    {
        public CommandId Id { get; private set; }

        public CommandBase(CommandId id)
        {
            Id = id;
        }

        public abstract string GetCommandHint();
        public abstract string GetRegexPattern();
        public abstract void HandleCommand(Match match);
    }

    public enum CommandId
    {
        Stop = 1,
        Help = 2,

        GetChats = 15,
        SelectChat = 16,

        ExportQuestions = 50,
    }
}