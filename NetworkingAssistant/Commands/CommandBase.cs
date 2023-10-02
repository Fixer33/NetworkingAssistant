using System.Text.RegularExpressions;

namespace NetworkingAssistant.Commands
{
    public abstract class CommandBase
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
        ShowSelectedChatInfo = 3,
        SelectAllMessages = 4,
        LeaveOnlyQuestions = 5,
        GenerateQuestions = 6,

        GetChats = 15,
        SelectChat = 16,
        SelectMessageHundred = 17,

        ExportQuestions = 50,
        SendRegistrationPoll = 51,
    }
}
