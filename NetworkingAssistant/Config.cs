using System.Xml.Serialization;

namespace NetworkingAssistant
{
    public static class Config
    {
        private const string FILE_NAME = "config.xml";

        public static int ApiId { get; private set; }
        public static string ApplicationVersion { get; private set; }
        public static string ApiHash { get; private set; }
        public static string PhoneNumber { get; private set; }

        public static void Load()
        {
            if (File.Exists(FILE_NAME))
            {
                ConfigSerializable saved;
                
                using (FileStream fs = File.Open(FILE_NAME, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ConfigSerializable));
                    try
                    {
                        saved = serializer.Deserialize(fs) as ConfigSerializable;
                    }
                    catch
                    {
                        saved = null;
                    }
                    fs.Close();
                }

                if (saved == null)
                {
                    LoadDefault();
                    return;
                }

                ApiId = saved.ApiId;
                ApplicationVersion = saved.ApplicationVersion;
                ApiHash = saved.ApiHash;
                PhoneNumber = saved.PhoneNumber;
                Console.WriteLine("Config loaded");
            }
            else
            {
                LoadDefault();
            }
        }

        private static void LoadDefault()
        {
            if (File.Exists(FILE_NAME))
                File.Delete(FILE_NAME);

            ConfigSerializable defaultConfig = new ConfigSerializable()
            {
                ApiId = 0,
                ApiHash = "FILL ME by https://my.telegram.org/apps",
                PhoneNumber = "Phone number",
                ApplicationVersion = "0.1",
            };

            using (FileStream fs = File.Create(FILE_NAME))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigSerializable));
                serializer.Serialize(fs, defaultConfig);
                fs.Close();
            }

            ApiId = defaultConfig.ApiId;
            ApplicationVersion = defaultConfig.ApplicationVersion;
            ApiHash = defaultConfig.ApiHash;
            PhoneNumber = defaultConfig.PhoneNumber;
            Console.WriteLine("Loaded default config. App will not work, visit config.xml file");

            Console.WriteLine("Config file created");
        }

        [Serializable]
        public class ConfigSerializable
        {
            public int ApiId;
            public string ApiHash;
            public string ApplicationVersion;
            public string PhoneNumber;
        }
    }
}
