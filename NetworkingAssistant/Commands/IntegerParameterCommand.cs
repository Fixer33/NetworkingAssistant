using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetworkingAssistant.Commands
{
    internal class IntegerParameterCommand : CommandBase
    {
        public IntegerParameterCommand(CommandId id) : base(id)
        {
        }

        public override string GetCommandHint()
        {
            return Id switch
            {
                CommandId.GetChats => "get_chats *chat_limit* - Lists all available to current user chats. *chat_limit* set maximum amount of chats listed",
                CommandId.SelectChat => @"select_chat *id* - Selects chat with *id*. Saves in buffer for further operations",

                _ => "",
            };
        }

        public override string GetRegexPattern()
        {
            return Id switch
            {
                CommandId.GetChats => @"^get_chats (\d+)$",
                CommandId.SelectChat => @"^select_chat (-?\d+)$",

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

            int parameter;
            if (int.TryParse(match.Groups[1].Value, out parameter) == false)
            {
                Console.WriteLine("Can't parse integer for " + Id);
                return;
            }

            switch (Id)
            {
                case CommandId.GetChats:
                    GetChatList(parameter);
                    break;

                case CommandId.SelectChat:
                    SelectChat(parameter);
                    break;
            }
        }

        private static async void GetChatList(int limit)
        {
            var list = await TelegramManager.GetChannels(limit);

            Console.WriteLine("Chat ID : Chat name");
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine(list[i].Id + " : " + list[i].Title);
            }
            Console.WriteLine();
        }

        private static async void SelectChat(int id)
        {
            OperationBuffer.SelectChat(await TelegramManager.GetChannel(id));
        }
    }
}
