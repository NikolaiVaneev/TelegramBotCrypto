using SQLite;
using TelegramBotCrypto.Data;

namespace TelegramBotCrypto.Models
{
    public class Crypto
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public string CryptoId { get; set; }
        //public string Title { get; set; }
        public string Code { get; set; }
        public string Link { get; set; }
        public int UserId { get; set; }
        public User User 
        {
            get 
            {
                if (UserId != 0)
                {
                    return DataBase.GetUser(UserId);
                }
                else
                    return new User();
            } 
        }
    }
}
