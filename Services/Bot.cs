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
            // Если запрос бонусов 
            if (e.CallbackQuery.Data == "get_bonus_stat")
            {
                int balance = DataBase.GetReferCount(user);
                SendMessageAsync(user, $"Ваш баланс бонусов - {balance} единиц");
                //await TelegramBot.SendTextMessageAsync(msg.Chat.Id, $"Ваш баланс - {balance}");
                string message = $"Чтобы пригласить друга, отправьте ему ссылку https://t.me/{Info.Username}?start={user.User_Id}";
                SendMessageAsync(user, message);
                //await TelegramBot.SendTextMessageAsync(msg.Chat.Id, message);
                return;
            }

            // Если кликнули на проект
            Project Project = DataBase.GetAllProjects().FirstOrDefault(u => u.Id.ToString() == e.CallbackQuery.Data);
            if (Project != null)
            {
                if (Project.IsCompletion)
                {
                    await TelegramBot.SendTextMessageAsync(msg.Chat.Id, $"К сожалению, данный проект уже завершен");
                    return;
                }
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

                if (project.IsCompletion)
                {
                    await TelegramBot.SendTextMessageAsync(msg.Chat.Id, $"К сожалению, данный проект уже завершен");
                    return;
                }

                // Если в проекте используется кошелек
                if (project.CryptoTypeId != 0)
                {
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
                }

                // Добавить пользователя в проект
                // Если пользователь не участвует
                bool isNewMember = DataBase.GetParticipation(user.User_Id, project.Id);
                if (isNewMember)
                {
                    DataBase.AttachParticipation(project.Id, user.User_Id);
                    SendMessageAsync(user, "Благодарим Вас за интерес к нашему проекту");
                }
                else
                {
                    SendMessageAsync(user, "Вы уже учавствуете в этом проекте :)");
                }
            }

            // Если кликнули на "Мои реквизиты"
            if (e.CallbackQuery.Data == "get_payment_delails")
            {
                string paymentDetails = DataBase.GetPaymentDetails(user);
                if (string.IsNullOrWhiteSpace(paymentDetails))
                {
                    await TelegramBot.SendTextMessageAsync(msg.Chat.Id, $"У Вас не заданы реквизиты");
                }
                else
                {
                    await TelegramBot.SendTextMessageAsync(msg.Chat.Id, $"Ваши реквизиты - {paymentDetails}");
                }
                
                await TelegramBot.SendTextMessageAsync(msg.Chat.Id, $"Для привязки Ваших платежных данных напишите мне сообщение в формате  \"/pd Номер_карты(счета) Платежная_система(банк)\" " +
                    $"{Environment.NewLine}Например: {Environment.NewLine}/pd 4201234567890000 Сбербанк" +
                    $"{Environment.NewLine}❗️Мы работаем со следующими платежными системами: Сбербанк, Тинькофф, Qiwi, ЮMoney, Webmoney");



                return;
            }


            // Возврат к базовому списку
            if (e.CallbackQuery.Data == "comeback")
            {
                SendProjectList(user, msg);
            }
        }
        private async static void SendProjectList(User user, Telegram.Bot.Types.Message msg)
        {
            var projects = DataBase.GetAllProjects().Where(u => !u.IsCompletion);

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
            // Реферальная программа
            var paymentBtn = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData($"💵 Мои реквизиты 💵", "get_payment_delails")
            };
            buttons.Add(paymentBtn);
            // Реферальная программа
            var referBtn = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData($"🏆 Мои бонусы 🏆", "get_bonus_stat")
            };
            // UNDONE : Пока рефералы отключены
        //    buttons.Add(referBtn);
            // Кнопка помощи
            var helpBtn = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData($"❓ Мне нужна помощь ❓", "need_help")
            };
            buttons.Add(helpBtn);



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
            string messageText = msg.Text;

            User user = new User
            {
                User_Id = msg.Chat.Id,
                User_Nickname = msg.Chat.Username,
                User_FirstName = msg.Chat.FirstName,
                User_LastName = msg.Chat.LastName
            };
            if (msg.Photo != null || msg.Document != null)
            {
                // TODO: Отправить фото админам (и сохранить может быть?)
                SendPhotoMessageAllAdmin(msg);
                return;
            }
            // Добавляем реферала
            if (messageText == null) return;
            if (messageText.Contains("start") && messageText.Length > 10)
            {
                string referId = messageText.Substring(messageText.IndexOf(" ") + 1);
                user.ReferId = int.Parse(referId);
            }
            DataBase.SaveUser(user);

            if (messageText.Contains("/pd"))
            {
                if (messageText.Length < 10)
                {
                    SendMessageAsync(user, "Введены некорректные платежные реквизиты");
 
                }
                else
                {
                    // TODO: Алгоритм привязки платежных реквизитов
                    string pd = messageText.Substring(3);
                    user.PaymentDetail = pd.Trim();
                    DataBase.UpdatePaymentDetail(user);
                    SendMessageAsync(user, $"Вы установили следующие реквизиты - {pd}");
                }
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
                        try { 
                        await TelegramBot.SendPhotoAsync(admin.User_Id, msg.Photo[0].FileId, message);
                        Logger.Add($"Пользователь {msg.Chat.Username} отправил изображение");
                        }
                        catch
                        {

                        }
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
                    SendMessageAsync(user, message);

                }
            });

        }

        /// <summary>
        /// Отправить сообщение всем пользователям
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="recipientsType">Кому</param>
        /// <param name="project">Проект</param>
        public async static void SendMessagesAllAsync(string message, int recipientsType, Project project)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            List<DiliveryStatus> diliveries = new List<DiliveryStatus>();
            List<User> SendedMessage = new List<User>();
            List<User> NotSendedMessage = new List<User>();
            List<User> FromUser = new List<User>();

            await Task.Run(() =>
            {

                List<Participation> participations = new List<Participation>();


                // Если выбран проект
                if (project != null)
                {
                    switch (recipientsType)
                    {
                        case 0:
                            participations = DataBase.GetParticipationList().Where(u => u.ProjectId == project.Id).ToList();
                            break;
                        case 1:
                            participations = DataBase.GetParticipationList().Where(u => u.ProjectId == project.Id).Where(u => u.User.User_Status == 1).ToList();
                            break;
                        case 2:
                            participations = DataBase.GetParticipationList().Where(u => u.ProjectId == project.Id).Where(u => u.User.User_Status == 0).ToList();
                            break;
                        default:
                            break;
                    }

                    foreach (var item in participations)
                    {
                        FromUser.Add(item.User);
                    }
                }
                else
                {
                    switch (recipientsType)
                    {
                        case 0:
                            FromUser = DataBase.GetUserList();
                            break;
                        case 1:
                            FromUser = DataBase.GetUserList().Where(u => u.User_Status == 1).ToList();
                            break;
                        case 2:
                            FromUser = DataBase.GetUserList().Where(u => u.User_Status == 0).ToList();
                            break;
                        default:
                            break;
                    }
                }

                foreach (var user in FromUser)
                {
                    Task<Telegram.Bot.Types.Message> sentMessage = default;
               

                        sentMessage = TelegramBot.SendTextMessageAsync(user.User_Id, message);
                  

                    try
                    {
                        var r = sentMessage.Result;
                        Logger.Add($"Сообщение пользователю {user.User_Nickname} ({user.User_Id}) отправлено");
                        SendedMessage.Add(user);
                    }
                    catch
                    {
                        Logger.Add($"Сообщение для {user.User_Nickname} ({user.User_Id}) НЕ отправлено");
                        NotSendedMessage.Add(user);
                    }
                }


                //SendMessagesAllAsync($"{cryptoType.Title}{Environment.NewLine}{message}", 1);
            });
            Logger.Add($"Всего сообщений - {FromUser.Count}, доставлено - {SendedMessage.Count}, не доставлено - {NotSendedMessage.Count}");
            ExcelWorker.ShowSendingReport(SendedMessage, NotSendedMessage);
  
    

        }
        public async static void SendMessageAsync(User user, string message)
        {
            await Task.Run(() =>
            {
                try
                {
                    Task<Telegram.Bot.Types.Message> sentMessage = TelegramBot.SendTextMessageAsync(user.User_Id, message);
                    Logger.Add($"Сообщение пользователю {user.User_Nickname} ({user.User_Id}) отправлено");
                }
                catch
                {
                    Logger.Add($"Сообщение для {user.User_Nickname} ({user.User_Id}) НЕ отправлено");
                }
            });
        }
    }
}