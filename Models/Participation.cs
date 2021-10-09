using SQLite;
using TelegramBotCrypto.Data;

namespace TelegramBotCrypto.Models
{
    public class Participation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// Адресс
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// ID проекта
        /// </summary>
        public int ProjectId { get; set; }
        public Project Project
        {
            get
            {
                if (ProjectId != 0)
                {
                    return DataBase.GetProject(ProjectId);
                }
                else
                    return new Project();
            }
        }
   
        public long UserId { get; set; }
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
