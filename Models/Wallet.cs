using SQLite;
using TelegramBotCrypto.Data;

namespace TelegramBotCrypto.Models
{
    public class Wallet
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int CryptoTypeId { get; set; }
        /// <summary>
        /// Адрес кошелька
        /// </summary>
        public string Code { get; set; }
        public long UserId { get; set; }
        /// <summary>
        /// Владелец кошелька
        /// </summary>
        public User User
        {
            get
            {
                if (UserId != 0)
                {
                    return DataBase.GetUser(UserId);
                }
                else
                {
                    return new User();
                }
            } 
        }
    }
}
