using SQLite;

namespace TelegramBotCrypto.Models
{
    public class CryptoType
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
