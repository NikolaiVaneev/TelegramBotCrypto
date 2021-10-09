using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotCrypto.Data;
using TelegramBotCrypto.Models;

namespace TelegramBotCrypto.Services
{

    internal static class Bot
    {
        public static TelegramBotClient TelegramBot { get; set; }
        private static Settings Settings { get; set; }
        public static Telegram.Bot.Types.User Info { get; set; }

        public static event Action InitializationError;
        public static event Action InitializationOk;

        [Obsolete]
        public static void Init()
        {
            SettingUpdated();
            try
            {
                TelegramBot = new TelegramBotClient(Settings.APIKey);
                Info = TelegramBot.GetMeAsync().Result;
                TelegramBot.StartReceiving();
                InitializationOk?.Invoke();

                TelegramBot.OnMessage += BotOnMessage;
                TelegramBot.OnCallbackQuery += BotOnCallbackQuery;
                Settings.OnSettingUpdated += SettingUpdated;
            }
            catch
            {
                InitializationError?.Invoke();
            }
        }
        private static void SettingUpdated()
        {
            Settings = new Settings().Load();
        }

        [Obsolete]
        private static async void BotOnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            var msg = e.CallbackQuery.Message;
            User user = new User
            {
                User_Id = msg.Chat.Id,
                User_Nickname = msg.Chat.Username,
                User_FirstName = msg.Chat.FirstName,
                User_LastName = msg.Chat.LastName
            };

            // Если нужна помощь
            if (e.CallbackQuery.Data == "need_help")
            {
                List<User> admins = DataBase.GetAdminAll();
                Random random = new Random();
                if (admins.Count > 0)
                {
                    int selectedAdmin = random.Next(0, admins.Count - 1);
                    await TelegramBot.SendTextMessageAsync(msg.Chat.Id, $"Для получения технической поддержки свяжитесь с @{admins[selectedAdmin].User_Nickname}");
                }
                else
                {
                    await TelegramBot.SendTextMessageAsync(msg.Chat.Id, $"К сожалению, свободные администраторы отсутствуют");
                }
                return;
            }

            // Если кликнули на проект
            Project Project = DataBase.GetAllProjects().FirstOrDefault(u => u.Id.ToString() == e.CallbackQuery.Data);
            if (Project != null)
            {
                await TelegramBot.SendTextMessageAsync(msg.Chat.Id, Project.Message);
                List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();
                var list = new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData("Да, принять участие!", $"{Project.Id}_register"),
                    InlineKeyboardButton.WithCallbackData("Нет, к проектам", "comeback")
                };
                buttons.Add(list);
                var ikm = new InlineKeyboardMarkup(buttons);
                await TelegramBot.SendTextMessageAsync(msg.Chat.Id, $"Примите участие?", replyMarkup: ikm);
                //TelegramBot.SendTextMessageAsync(msg.Chat.Id, DataBase.AnchorUser(msg.Chat.Id, e.CallbackQuery.Data));
            }

            // Если кликнули на "Принять участите"
            if (e.CallbackQuery.Data.Contains("_register"))
            {
                string projectID = e.CallbackQuery.Data.Substring(0, e.CallbackQuery.Data.IndexOf("_register"));
                Project project = DataBase.GetProject(int.Parse(projectID));

                // Присвоить криптокошелек типа данного проекта, если его не было
                Wallet userWallet = DataBase.GetWallet(user.User_Id, project.CryptoTypeId);
                if (userWallet == null)
                {
                    //Проверить, есть-ли вообще свободные кошельки
                    List<Wallet> freeWallets = DataBase.GetFreeWallets(project.CryptoTypeId);
                    if (freeWallets.Count > 0)
                    {
                        DataBase.AttachWallet(freeWallets[0].Id, user.User_Id);
                        await TelegramBot.SendTextMessageAsync(msg.Chat.Id, $"Вам присвоен {project.CryptoType.Title} кошелек - {freeWallets[0].Code}");
                    }
                    else
                    {
                        await TelegramBot.SendTextMessageAsync(msg.Chat.Id, $"К сожалению, свободных {project.CryptoType.Title} кошельков пока нет. Попробуйте позже");
                        return;
                    }
                }
                else
                {
                    await TelegramBot.SendTextMessageAsync(msg.Chat.Id, $"Ваш {project.CryptoType.Title} кошелек - {userWallet.Code}");
                }

                // Добавить пользователя в проект
                // Если пользователь не участвует
                if (!DataBase.GetParticipation(user.User_Id, project.Id))
                {
                    DataBase.AttachParticipation(project.Id, user.User_Id);
                    await TelegramBot.SendTextMessageAsync(msg.Chat.Id, $"Благодарим Вас за интерес к нашему проекту");
                }
                else
                {
                    await TelegramBot.SendTextMessageAsync(msg.Chat.Id, $"Вы уже учавствуете в этом проекте :)");
                }
            }

            // Возврат к списку проектов
            if (e.CallbackQuery.Data == "comeback")
            {
                SendProjectList(user, msg);
            }
        }

        private async static void SendProjectList(User user, Telegram.Bot.Types.Message msg)
        {
            var projects = DataBase.GetAllProjects();


            //List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();

            foreach (var project in projects)
            {
                // По одной линии
                var btn = InlineKeyboardButton.WithCallbackData(project.Title, project.Id.ToString());
                var list = new List<InlineKeyboardButton>
                {
                    btn
                };
                buttons.Add(list);

                // В одну линию
                //var btn = InlineKeyboardButton.WithCallbackData(type.Title, type.Title);
                //buttons.Add(btn);
            }

            // Кнопка помощи
            var helpBtn = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData($"Мне нужна помощь", "need_help")
            };
            buttons.Add(helpBtn);

            //var rkm = new ReplyKeyboardMarkup
            //{
            //    Keyboard =
            //        new KeyboardButton[][]
            //        {
            //            new KeyboardButton[]
            //            {
            //                new KeyboardButton("item1"),
            //                new KeyboardButton("item2")
            //            }
            //        }
            //};

            //await TelegramBot.SendTextMessageAsync(msg.Chat.Id, "Text", replyMarkup: rkm);

            var ikm = new InlineKeyboardMarkup(buttons);

            Settings settings = new Settings().Load();
            try
            {
                await TelegramBot.SendTextMessageAsync(msg.Chat.Id, settings.HelloMessage, replyMarkup: ikm);
                Logger.Add($"Сообщение пользователю {user.User_Nickname} отправлено");
            }
            catch
            {
                Logger.Add($"Сообщение для {user.User_Nickname} НЕ отправлено");
            }
        }

        [Obsolete]
        private static void BotOnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            Telegram.Bot.Types.Message msg = e.Message;
            User user = new User
            {
                User_Id = msg.Chat.Id,
                User_Nickname = msg.Chat.Username,
                User_FirstName = msg.Chat.FirstName,
                User_LastName = msg.Chat.LastName
            };
            DataBase.SaveUser(user);


            if (msg.Photo != null || msg.Document != null)
            {
                // TODO: Отправить фото админам (и сохранить может быть?)
                SendPhotoMessageAllAdmin(msg);
                return;
            }

            if (msg.Text == null) return;
            SendProjectList(user, msg);

        }
        public async static void SendPhotoMessageAllAdmin(Telegram.Bot.Types.Message msg)
        {
            List<User> admins = DataBase.GetAdminAll();
            if (admins.Count > 0)
            {

                string message = $"Пользователь @{msg.Chat.Username} (id {msg.Chat.Id}) отправил это сообщение";
                foreach (User admin in admins)
                {
                    if (msg.Photo != null)
                    {
                        await TelegramBot.SendPhotoAsync(admin.User_Id, msg.Photo[0].FileId, message);
                        Logger.Add($"Пользователь {msg.Chat.Username} отправил изображение");
                    }

                    if (msg.Document != null)
                    {
                        var file = await TelegramBot.GetFileAsync(msg.Document.FileId);
                        await TelegramBot.SendDocumentAsync(admin.User_Id, file.FileId);
                        await TelegramBot.SendTextMessageAsync(admin.User_Id, message);

                        Logger.Add($"Пользователь {msg.Chat.Username} отправил документ");

                    }
                }
            }

        }

        /// <summary>
        /// Отправить сообщения по типу пользователей
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="recipientsType">0 - всем, 1 - админам, 2 - пользователям</param>
        public async static void SendMessagesAllAsync(string message, int recipientsType)
        {
            await Task.Run(() =>
            {
                IEnumerable<User> users = new List<User>();

                switch (recipientsType)
                {
                    //Все
                    case 0:
                        users = DataBase.GetUserList();
                        break;
                    //Админы
                    case 1:
                        users = DataBase.GetUserList().Where(u => u.User_Status == 1);
                        break;
                    //Пользователи
                    case 2:
                        users = DataBase.GetUserList().Where(u => u.User_Status == 0);
                        break;
                    default:
                        break;
                }

                foreach (var user in users)
                {
                    try
                    {
                        TelegramBot.SendTextMessageAsync(user.User_Id, message);
                        Logger.Add($"Сообщение пользователю {user.User_Nickname} ({user.User_Id}) отправлено");
                    }
                    catch
                    {
                        Logger.Add($"Сообщение для {user.User_Nickname} ({user.User_Id}) НЕ отправлено");
                    }
                }
            });

        }

        /// <summary>
        /// Отправить сообщения по типу криптовалют
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="cryptoType">Тип криптовалюты</param>
        public async static void SendMessagesAllAsync(string message, CryptoType cryptoType)
        {
            if (cryptoType == null || string.IsNullOrEmpty(message))
            {
                return;
            }

            await Task.Run(() =>
            {
                IEnumerable<User> users = DataBase.GetUserList();
                IEnumerable<Wallet> cryptos = DataBase.GetAllCryptoAddress(cryptoType.Title);

                if (cryptos.Count() == 0) return;

                foreach (var crypto in cryptos)
                {
                    try
                    {
                        if (crypto.UserId != 0 && crypto.User.User_Status == 0)
                        {
                            TelegramBot.SendTextMessageAsync(crypto.UserId, message);
                            Logger.Add($"Сообщение пользователю {crypto.User.User_Nickname} ({crypto.User.User_Id}) отправлено");
                        }
                    }
                    catch
                    {
                        Logger.Add($"Сообщение для {crypto.User.User_Nickname} ({crypto.User.User_Id}) НЕ отправлено");
                    }
                }

                SendMessagesAllAsync($"{cryptoType.Title}{Environment.NewLine}{message}", 1);
            });

        }
    }
}
