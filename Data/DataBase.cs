using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TelegramBotCrypto.Models;
using TelegramBotCrypto.Services;

namespace TelegramBotCrypto.Data
{
    internal static class DataBase
    {
        //public static string DataBasePath { get; set; } = Path.Combine(Environment.CurrentDirectory, "DataBase.db");
        public static string DataBasePath { get; set; } = "DataBase.db";
        public static bool CheckDataBaseExist()
        {
            if (!File.Exists(DataBasePath))
            {
                Create();
            }
            return CheckValidityDB();
        }
        /// <summary>
        /// Создать БД
        /// </summary>
        public static void Create()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                connection.CreateTable<User>();
                connection.CreateTable<Wallet>();
                connection.CreateTable<CryptoType>();
                connection.CreateTable<Project>();
                connection.CreateTable<Participation>();
            }
        }

        #region Пользователи

        /// <summary>
        /// Получить всех администраторов
        /// </summary>
        /// <returns></returns>
        internal static List<User> GetAdminAll()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                List<User> list = new List<User>();
                list = connection.Query<User>("SELECT * FROM User WHERE User_Status = 1");
                return list;
            };
        }

        internal static void SaveProject(Project project)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                if (project.Id == 0)
                    connection.Insert(project);
                else
                    connection.Update(project);
            };
        }

        internal static void DeleteProject(Project project)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                connection.Delete(project);
            };
        }
        internal static List<Participation> GetParticipationList(string finder = "")
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                List<Participation> list = new List<Participation>();
                if (string.IsNullOrEmpty(finder))
                    list = connection.Query<Participation>($"SELECT * FROM Participation JOIN User u ON u.User_id = Participation.UserId");
                else
                {
                    string query = "SELECT * FROM Participation JOIN User u ON u.User_id = Participation.UserId WHERE User_Nickname LIKE ? OR UserId LIKE ?;";
                    list = connection.Query<Participation>(query, new string[] { "%" + finder + "%", "%" + finder + "%" });
                }


                return list;
            };
        }
        /// <summary>
        /// Получить количество рефералов
        /// </summary>
        /// <returns></returns>
        internal static int GetReferCount(User user)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {

                return connection.Query<User>($"SELECT * FROM User WHERE ReferId = {user.User_Id}").Count;

            };
        }

        internal static List<Participation> GetParticipationProject(int projectId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                List<Participation> list = new List<Participation>();
                list = connection.Query<Participation>($"SELECT * FROM Participation WHERE ProjectId = {projectId}");
                return list;
            };
        }


        /// <summary>
        /// Изменить статус пользователя
        /// </summary>
        /// <param name="user"></param>
        public async static void ChangeUserStatusAcync(User user)
        {
            await Task.Run(() =>
            {
                using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
                {
                    connection.Query<User>($"UPDATE User SET User_Status = '{user.User_Status}' WHERE User_id = {user.User_Id}");
                    if (user.User_Status == 0)
                    {
                        connection.Query<User>($"UPDATE User SET AdminMessage = 0 WHERE User_id = {user.User_Id}");
                    }
                    else
                    {
                        connection.Query<User>($"UPDATE User SET AdminMessage = '{user.AdminMessage}' WHERE User_id = {user.User_Id}");
                    }
                };
            });
        }

        /// <summary>
        /// Получить все проекты
        /// </summary>
        /// <returns></returns>
        internal static List<Project> GetAllProjects()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                List<Project> list = new List<Project>();
                list = connection.Query<Project>("SELECT * FROM Project");
                return list;
            };
        }

        /// <summary>
        /// Получить список проектов
        /// </summary>
        /// <returns></returns>
        internal static List<Project> GetProjects()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                return connection.Query<Project>($"SELECT * FROM Project");
            };
        }

        /// <summary>
        /// Получить пользователя по ИД
        /// </summary>
        /// <param name="userId">ИД</param>
        /// <returns></returns>
        public static User GetUser(long userId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                return connection.Query<User>($"SELECT * FROM User WHERE User_Id = {userId}").FirstOrDefault();
            };
        }

        /// <summary>
        /// Получить свободные кошельки по ИД криптовалюты
        /// </summary>
        /// <param name="cryptoTypeId">ИД криптовалюты</param>
        /// <returns></returns>
        internal static List<Wallet> GetFreeWallets(int cryptoTypeId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                List<Wallet> list = new List<Wallet>();
                list = connection.Query<Wallet>($"SELECT * FROM Wallet WHERE UserId = 0 AND CryptoTypeId = {cryptoTypeId}");
                return list;
            };
        }

        /// <summary>
        /// Присоединить к пользователю кошелек
        /// </summary>
        /// <param name="id">ИД кошелька</param>
        /// <param name="user_Id">ИД пользователя</param>
        internal static void AttachWallet(int id, long user_Id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                connection.Query<Wallet>($"UPDATE Wallet SET UserId = {user_Id} WHERE Id = {id}");
            }
        }

        internal static void AttachParticipation(int projectId, long userId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                Participation participation = new Participation
                {
                    ProjectId = projectId,
                    UserId = userId
                };
                connection.Insert(participation);
            };
        }

        internal static string GetPaymentDetails(User user)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                return connection.Query<User>($"SELECT * FROM User WHERE User_Id = {user.User_Id}").FirstOrDefault().PaymentDetail;
            };
        }

        /// <summary>
        /// Учавствует-ли пользователь в проекте?
        /// </summary>
        /// <param name="user_Id">ИД пользователя</param>
        /// <param name="id">ИД проекта</param>
        /// <returns></returns>
        internal static bool GetParticipation(long userId, int id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                Participation participation = connection.Query<Participation>($"SELECT * FROM Participation WHERE ProjectId = {id} AND UserId = {userId}").FirstOrDefault();
                return participation == null;
            };
        }

        /// <summary>
        /// Получить кошелек пользователя по типу валюты
        /// </summary>
        /// <param name="user_Id">ИД пользователя</param>
        /// <param name="cryptoTypeId">ИД валюты</param>
        /// <returns></returns>
        internal static Wallet GetWallet(long userId, int cryptoTypeId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                return connection.Query<Wallet>($"SELECT * FROM Wallet WHERE UserId = {userId} AND CryptoTypeId = {cryptoTypeId}").FirstOrDefault();
            };
        }

        /// <summary>
        /// Получить список пользователей
        /// </summary>
        /// <returns></returns>
        //public static List<User> GetUserList()
        //{
        //    using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
        //    {
        //        List<User> list = new List<User>();
        //        list = connection.Query<User>("SELECT * FROM User");
        //        return list;
        //    };
        //}


        public static List<User> GetUserList(string filter = "")
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                List<User> list = new List<User>();

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    string query = "SELECT * FROM User WHERE User_Nickname LIKE ? OR User_Id LIKE ?;";
                    list = connection.Query<User>(query, new string[] { "%" + filter + "%", "%" + filter + "%" });
                }
                else
                {
                    list = connection.Query<User>("SELECT * FROM User");
                }

                return list;
            };
        }

        /// <summary>
        /// Сохранить пользователя в БД
        /// </summary>
        /// <param name="user"></param>
        public static void SaveUser(User user)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                var findedUser = connection.Query<User>($"SELECT * FROM User WHERE User_id = {user.User_Id}");
                if (findedUser.Count == 0)
                {
                    connection.Insert(user);
                    Logger.Add($"Пользователь {user.User_Nickname} добавлен в базу данных");
                }

                else if (findedUser[0].User_Nickname != user.User_Nickname)
                {
                    connection.Query<User>($"UPDATE User SET User_nickname = '{user.User_Nickname}' WHERE User_id = {user.User_Id}");
                    Logger.Add($"Ник пользователя {findedUser[0].User_Nickname} изменен на {user.User_Nickname}");
                }

            };
        }

        internal static void UpdatePaymentDetail(User user)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                {
                    string query = "UPDATE User SET PaymentDetail = ? WHERE User_id = ?;";
                    connection.Query<User>(query, new string[] { user.PaymentDetail, user.User_Id.ToString() });


                    Logger.Add($"Пользователь {user.User_Nickname} обновил платежные реквизиты");
                }

            };
        }


        #endregion

        #region Тип криптовалюты

        /// <summary>
        /// Сохранить тип криптовалюты
        /// </summary>
        /// <param name="cryptoType">Название</param>
        internal static void SaveCryptoType(CryptoType cryptoType)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                if (cryptoType.Id == 0)
                    connection.Insert(cryptoType);
                else
                    connection.Update(cryptoType);
            };
        }

        /// <summary>
        /// Удалить тип криптовалюты
        /// </summary>
        /// <param name="cryptoType"></param>
        internal static void DeleteCryptoType(CryptoType cryptoType)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                connection.Delete(cryptoType);
                connection.Execute($"DELETE FROM Wallet WHERE CryptoTypeId = {cryptoType.Id}");
            };
        }

        /// <summary>
        /// Получить тип криптовалюты по названию
        /// </summary>
        /// <param name="title">Название</param>
        /// <returns></returns>
        internal static CryptoType GetCryptoTypeData(string title)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                return connection.Query<CryptoType>($"SELECT * FROM CryptoType WHERE Title LIKE \"{title}\"").FirstOrDefault();
            };
        }
        /// <summary>
        /// Получить тип криптовалюты по ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        internal static CryptoType GetCryptoTypeData(int id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                return connection.Query<CryptoType>($"SELECT * FROM CryptoType WHERE ID = {id}").FirstOrDefault();
            };
        }
        public static List<CryptoType> GetAllCryptoType()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                return connection.Query<CryptoType>($"SELECT * FROM CryptoType");
            };
        }


        #endregion


        /// <summary>
        /// Получить проект по ИД
        /// </summary>
        /// <param name="projectId">ИД</param>
        /// <returns></returns>
        internal static Project GetProject(int projectId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                return connection.Query<Project>($"SELECT * FROM Project WHERE Id = {projectId}").FirstOrDefault();
            };
        }
        private static bool CheckValidityDB()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {

                int tableCount = connection.ExecuteScalar<int>("SELECT COUNT() FROM sqlite_master WHERE type = 'table' " +
                    "AND name = 'User' " +
                    "OR name = 'Crypto'");
                return tableCount == 2;
            };
        }
        /// <summary>
        /// Добавить новый тип криптовалюты
        /// </summary>
        /// <param name="cryptoType">Название</param>
        internal static List<Wallet> GetAllCryptoAddress(string cryptoTitle)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                List<Wallet> list = new List<Wallet>();
                if (string.IsNullOrEmpty(cryptoTitle))
                    list = connection.Query<Wallet>("SELECT * FROM Wallet");
                else
                {
                    var ct = GetAllCryptoType().Where(u => u.Title == cryptoTitle).FirstOrDefault();
                    list = connection.Query<Wallet>($"SELECT CryptoTypeId, Code, UserId FROM Wallet w JOIN CryptoType ct ON ct.Id = w.CryptoTypeId WHERE CryptoTypeId = {ct.Id}");
                }
                return list;
            };
        }
        internal static void AddCryptoAddressCollection(List<Wallet> list)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                connection.InsertAll(list);
            };
        }
        public static string AnchorUser(long id, string cryptoName)
        {
            StringBuilder message = new StringBuilder();
            CryptoType cryptoType = GetCryptoTypeData(cryptoName);
            if (cryptoType == null) return string.Empty;
            User currentUser = GetUser(id);

            // Если пользователю уже присвоен адрес
            if (CheckUserAnchor(id, cryptoType.Id))
            {
                Wallet crypto = GetCryptoData(id);
                message.AppendLine($"Ваш уникальный адрес {cryptoName}");

                message.AppendLine($"Идентификатор {crypto.Code}");
            }
            else
            {
                using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
                {
                    List<Wallet> cryptoList = connection.Query<Wallet>($"SELECT * FROM Crypto WHERE UserId = 0 AND CryptoId = {cryptoType.Id}");
                    // Если нет свободных
                    if (cryptoList.Count == 0)
                    {
                        message.AppendLine($"К сожалению, свободных адресов для {cryptoName} нет");
                        return message.ToString();
                    }
                    // Новая привязка
                    else
                    {
                        connection.Query<Wallet>($"UPDATE Crypto SET UserId = '{id}' WHERE Id = {cryptoList[0].Id}");
                        Wallet crypto = GetCryptoData(id);
                        message.AppendLine($"Поздравляем! Вам присвоен уникальный адрес {cryptoName}");

                        message.AppendLine($"Идентификатор {crypto.Code}");
                        //    Logger.Add($"За пользователем {currentUser.User_Nickname} закреплен адрес {cryptoName} {crypto.Link}");
                    }
                };
            }
            //message.AppendLine(cryptoType.Message);
            return message.ToString();
        }

        /// <summary>
        /// Проверить, привязан-ли пользователь к типу валюты
        /// </summary>
        /// <param name="id">ИД пользователя</param>
        /// <param name="cryptoName">Наименование криптовалюты</param>
        /// <returns></returns>
        private static bool CheckUserAnchor(long id, int cryptoId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                List<Wallet> crypto = connection.Query<Wallet>($"SELECT * FROM Crypto WHERE UserId = {id} AND CryptoId = {cryptoId}");
                return crypto.Count > 0;
            };
        }

        /// <summary>
        /// Получить привязанную к пользователю криптовалюту
        /// </summary>
        /// <param name="userId">ИД пользователя</param>
        /// <returns></returns>
        private static Wallet GetCryptoData(long userId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                return connection.Query<Wallet>($"SELECT * FROM Crypto WHERE UserId = {userId}").FirstOrDefault();
            };
        }

    }
}
