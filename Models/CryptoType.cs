using SQLite;

namespace TelegramBotCrypto.Models
{
    public class CryptoType
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// Название валюты
        /// </summary>
        public string Title { get; set; }
    }
}
