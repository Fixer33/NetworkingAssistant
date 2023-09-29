using TdLib;
using TdLib.Bindings;
using static TdLib.TdApi;

namespace NetworkingAssistant
{
    internal static class TelegramManager
    {
        public static bool Initialized { get; private set; }

        private static TdClient _client;

        public static async Task Initialize()
        {
            _client = new TdClient();
            _client.Bindings.SetLogVerbosityLevel(TdLogLevel.Error);
            _client.UpdateReceived += UpdateRecieved;

            await ReadyToAuthentificate.Wait();

            if (ReadyToAuthentificate.AuthRequired)
                await HandleAuthentication();

            //await ClientReady.Wait();

            Console.WriteLine("Client ready");
            Initialized = true;
        }

        private static async void UpdateRecieved(object? sender, TdApi.Update update)
        {
            switch (update)
            {
                case TdApi.Update.UpdateAuthorizationState { AuthorizationState: TdApi.AuthorizationState.AuthorizationStateWaitTdlibParameters }:
                    // TdLib creates database in the current directory.
                    // so create separate directory and switch to that dir.
                    var filesLocation = Path.Combine(AppContext.BaseDirectory, "db");
                    await _client.ExecuteAsync(new TdApi.SetTdlibParameters()
                    {
                        ApiId = Config.ApiId,
                        ApiHash = Config.ApiHash,
                        DeviceModel = "PC",
                        SystemLanguageCode = "en",
                        ApplicationVersion = Config.ApplicationVersion,
                        DatabaseDirectory = filesLocation,
                        FilesDirectory = filesLocation,
                    });
                    break;

                case TdApi.Update.UpdateAuthorizationState { AuthorizationState: TdApi.AuthorizationState.AuthorizationStateWaitPhoneNumber }:
                case TdApi.Update.UpdateAuthorizationState { AuthorizationState: TdApi.AuthorizationState.AuthorizationStateWaitCode }:
                    ReadyToAuthentificate.Set(true, false);
                    break;

                case TdApi.Update.UpdateAuthorizationState { AuthorizationState: TdApi.AuthorizationState.AuthorizationStateWaitPassword }:
                    ReadyToAuthentificate.Set(true, true);
                    break;

                case TdApi.Update.UpdateUser:
                    ReadyToAuthentificate.Set(false, false);
                    break;

                case TdApi.Update.UpdateConnectionState { State: TdApi.ConnectionState.ConnectionStateReady }:
                    // You may trigger additional event on connection state change
                    ClientReady.SetReady();
                    break;

                default:
                    // ReSharper disable once EmptyStatement
                    ;
                    // Add a breakpoint here to see other events
                    break;
            }
        }

        private static async Task HandleAuthentication()
        {
            // Setting phone number
            await _client.ExecuteAsync(new TdApi.SetAuthenticationPhoneNumber
            {
                PhoneNumber = Config.PhoneNumber
            });

            // Telegram servers will send code to us
            Console.Write("Insert the login code: ");
            var code = Console.ReadLine();

            await _client.ExecuteAsync(new TdApi.CheckAuthenticationCode
            {
                Code = code
            });

            if (!ReadyToAuthentificate.PasswordRequired) { return; }

            // 2FA may be enabled. Cloud password is required in that case.
            Console.Write("Insert the password: ");
            var password = Console.ReadLine();

            await _client.ExecuteAsync(new TdApi.CheckAuthenticationPassword
            {
                Password = password
            });
        }

        public static async Task<TdApi.User> GetCurrentUser()
        {
            return await _client.ExecuteAsync(new TdApi.GetMe());
        }

        public static async Task<List<TdApi.Chat>> GetChannels(int limit)
        {
            var chats = await _client.ExecuteAsync(new TdApi.GetChats
            {
                Limit = limit
            });

            List<TdApi.Chat> result = new List<TdApi.Chat>();

            foreach (var chatId in chats.ChatIds)
            {
                var chat = await _client.ExecuteAsync(new TdApi.GetChat
                {
                    ChatId = chatId
                });

                if (chat.Type is TdApi.ChatType.ChatTypeSupergroup or TdApi.ChatType.ChatTypeBasicGroup)
                {
                    result.Add(chat);
                }
            }

            return result;
        }

        public static async Task<TdApi.Chat> GetChannel(long id)
        {
            try
            {
                return await _client.ExecuteAsync(new TdApi.GetChat
                {
                    ChatId = id
                });
            }
            catch
            {
                return null;
            }
        }

        public static async Task<TdApi.Messages> GetMessages(long chatId, int limit, int offset)
        {
            try
            {
                return await _client.ExecuteAsync(new TdApi.GetChatHistory
                {
                    ChatId = chatId,
                    Limit = limit,
                    Offset = offset,
                });
            }
            catch
            {
                return null;
            }
        }

        public static void SendRegistrationPoll(string question)
        {
            var poll = new InputMessageContent.InputMessagePoll();
            poll.Options = new string[]
            {
                "Прыйду",
                "Магчыма прыйду",
                "Не прыйду",
                "Глядзець вынікі",
            };
            poll.Question = question;
            poll.IsAnonymous = false;

            _client.Send(new TdApi.SendMessage
            {
                ChatId = OperationBuffer.SelectedChat.Id,
                DataType = new TdApi.PollType.PollTypeRegular().DataType,
                InputMessageContent = poll,
            });
        }

        public static void Dispose()
        {
            _client.Dispose();
        }

        private static class ReadyToAuthentificate
        {
            public static bool AuthRequired { get; private set; }
            public static bool PasswordRequired { get; private set; }

            private static bool _isWaiting;

            public static async Task Wait()
            {
                _isWaiting = true;

                while (_isWaiting)
                {
                    await Task.Delay(100);
                }
            }

            public static void Set(bool authRequired, bool passwordRequired)
            {
                AuthRequired = authRequired;
                PasswordRequired = passwordRequired;

                _isWaiting = false;
            }
        }

        private static class ClientReady
        {
            private static bool _isWaiting;

            public static async Task Wait()
            {
                _isWaiting = true;

                while (_isWaiting)
                {
                    await Task.Delay(100);
                }
            }

            public static void SetReady()
            {
                _isWaiting = false;
            }
        }
    }
}
