using SQLite;
using TelegramBotCrypto.Data;

namespace TelegramBotCrypto.Models
{
    public class Address
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
    }
}
