using SQLite;

namespace TelegramBotCrypto.Models
{
    public class Project
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// Название проекта
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Приветственное сообщение
        /// </summary>
        public string Message { get; set; }
    }
}
