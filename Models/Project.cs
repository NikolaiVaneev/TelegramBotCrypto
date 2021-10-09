using SQLite;
using TelegramBotCrypto.Data;

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
        public int CryptoTypeId { get; set; }
        public CryptoType CryptoType
        {
            get
            {
                if (CryptoTypeId != 0)
                {
                    return DataBase.GetCryptoTypeData(CryptoTypeId);
                }
                else
                {
                    return new CryptoType();
                }
            }
        }
    }
}
