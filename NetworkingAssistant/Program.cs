namespace NetworkingAssistant
{
    internal class Program
    {
        private static bool _isWorking;

        static void Main(string[] args)
        {
            Start();

            while (TelegramManager.Initialized == false)
            {
                
            }

            while (_isWorking)
            {
                CommandHandler.HandleCommand(Console.ReadLine());
                Console.WriteLine("");
            }
        }

        private static async void Start()
        {
            _isWorking = true;
            Config.Load();
            AI.Initialize();

            await TelegramManager.Initialize();


        }

        public static void Stop()
        {
            Console.WriteLine("Stopping client");
            TelegramManager.Dispose();
            _isWorking = false;
        }
    }
}