using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        internal static void AddCryptoType(string cryptoType, string message)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                var item = new CryptoType
                {
                    Title = cryptoType,
                    Message = message
                };
                connection.Insert(item);
            };
        }
        internal static List<Crypto> GetAllCryptoAddress(string cryptoTitle)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                List<Crypto> list = new List<Crypto>();
                if (string.IsNullOrEmpty(cryptoTitle))
                    list = connection.Query<Crypto>("SELECT * FROM Crypto");
                else
                    list = connection.Query<Crypto>($"SELECT  CryptoId, Code, Link, UserId FROM Crypto c JOIN CryptoType ct ON ct.Id = c.CryptoId WHERE ct.Title LIKE \"{cryptoTitle}\"");
                return list;
            };
        }

        internal static void DeleteCryptoType(CryptoType cryptoType)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                connection.Delete(cryptoType);
                connection.Execute($"DELETE FROM Crypto WHERE CryptoID = {cryptoType.Id}");
            };
        }

        internal static void AddCryptoAddressCollection(List<Crypto> list)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                connection.InsertAll(list);
            };
        }

        /// <summary>
        /// Создать БД
        /// </summary>
        public static void Create()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                connection.CreateTable<User>();
                connection.CreateTable<Crypto>();
                connection.CreateTable<CryptoType>();
            }
        }

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

        /// <summary>
        /// Изменить статус пользователя
        /// </summary>
        /// <param name="user"></param>
        public static void ChangeUserStatus(User user)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                connection.Query<User>($"UPDATE User SET User_Status = '{user.User_Status}' WHERE User_id = {user.User_Id}");
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
        public static string AnchorUser(long id, string cryptoName)
        {
            StringBuilder message = new StringBuilder();
            CryptoType cryptoType = GetCryptoTypeData(cryptoName);
            if (cryptoType == null) return string.Empty;
            User currentUser = GetUser(id);

            // Если пользователю уже присвоен адрес
            if (CheckUserAnchor(id, cryptoType.Id))
            {
                Crypto crypto = GetCryptoData(id);
                message.AppendLine($"Ваш уникальный адрес {cryptoName}");
                message.AppendLine($"Ссылка {crypto.Link}");
                message.AppendLine($"Идентификатор {crypto.Code}");
            }
            else
            {
                using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
                {
                    List<Crypto> cryptoList = connection.Query<Crypto>($"SELECT * FROM Crypto WHERE UserId = 0 AND CryptoId = {cryptoType.Id}");
                    // Если нет свободных
                    if (cryptoList.Count == 0)
                    {
                        message.AppendLine($"К сожалению, свободных адресов для {cryptoName} нет");
                        return message.ToString();
                    }
                    // Новая привязка
                    else
                    {
                        connection.Query<Crypto>($"UPDATE Crypto SET UserId = '{id}' WHERE Id = {cryptoList[0].Id}");
                        Crypto crypto = GetCryptoData(id);
                        message.AppendLine($"Поздравляем! Вам присвоен уникальный адрес {cryptoName}");
                        message.AppendLine($"Ссылка {crypto.Link}");
                        message.AppendLine($"Идентификатор {crypto.Code}");
                        Logger.Add($"За пользователем {currentUser.User_Nickname} закреплен адрес {cryptoName} {crypto.Link}");
                    }
                };
            }
            message.AppendLine(cryptoType.Message);
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
                List<Crypto> crypto = connection.Query<Crypto>($"SELECT * FROM Crypto WHERE UserId = {id} AND CryptoId = {cryptoId}");
                return crypto.Count > 0;
            };
        }

        /// <summary>
        /// Получить привязанную к пользователю криптовалюту
        /// </summary>
        /// <param name="userId">ИД пользователя</param>
        /// <returns></returns>
        private static Crypto GetCryptoData(long userId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                return connection.Query<Crypto>($"SELECT * FROM Crypto WHERE UserId = {userId}").FirstOrDefault();
            };
        }

        internal static CryptoType GetCryptoTypeData(string title)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                return connection.Query<CryptoType>($"SELECT * FROM CryptoType WHERE Title LIKE \"{title}\"").FirstOrDefault();
            };
        }
        public static List<CryptoType> GetAllCryptoType()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                return connection.Query<CryptoType>($"SELECT * FROM CryptoType");
            };
        }

        /// <summary>
        /// Получить список пользователей
        /// </summary>
        /// <returns></returns>
        public static List<User> GetUserList()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DataBasePath))
            {
                List<User> list = new List<User>();
                list = connection.Query<User>("SELECT * FROM User");
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
    }
}
