using System;
using System.IO;
using System.Xml.Serialization;

namespace TelegramBotCrypto.Models
{
    public class Settings
    {
        public event Action OnSettingUpdated;

        [NonSerialized]
        private readonly string fileName = "Settings.xml";
        public string APIKey { get; set; } 
        public string HelloMessage { get; set; } 
        public void Save()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(Settings));
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, this);
            }
            OnSettingUpdated?.Invoke();
        }
        public Settings Load()
        {
            if (!File.Exists(fileName))
            {
                Save();
            }

            XmlSerializer formatter = new XmlSerializer(typeof(Settings));
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                return (Settings)formatter.Deserialize(fs);
            }
        }
    }
}
