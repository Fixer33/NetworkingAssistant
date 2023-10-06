using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static TdLib.TdApi;

namespace NetworkingAssistant.Commands
{
    internal class LongParameterCommand : CommandBase
    {
        public LongParameterCommand(CommandId id) : base(id)
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

            long parameter;
            if (long.TryParse(match.Groups[1].Value, out parameter) == false)
            {
                Console.WriteLine("Can't parse long for " + Id);
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

        private static async void GetChatList(long limit)
        {
            var list = await TelegramManager.GetChannels(Convert.ToInt32(limit));

            Console.WriteLine("Chat ID : Chat name");
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine(list[i].Id + " : " + list[i].Title);
            }
            Console.WriteLine();
        }

        private static async void SelectChat(long id)
        {
            OperationBuffer.SelectChat(await TelegramManager.GetChannel(id));
        }

      
    }
}
