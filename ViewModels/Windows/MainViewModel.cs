
using System;
using System.Windows.Input;
using TelegramBotCrypto.Data;
using TelegramBotCrypto.Infrastructure.Commands;
using TelegramBotCrypto.Services;
using TelegramBotCrypto.Views.Pages;

namespace TelegramBotCrypto.ViewModels
{
    internal class MainViewModel : ViewModel
    {
        #region Заголовок окна
        private string _title;
        /// <summary>Заголовок окна</summary>
        public string Title
        {
            get => _title;
            private set => SetProperty(ref _title, value);
        }
        #endregion

        #region Статус программы
        private string _status = "Статус";
        /// <summary>Статус программы</summary>
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        #endregion

        #region Главное содержимое 
        private object _page;
        /// <summary>Главное содержимое</summary>
        public object Content
        {
            get => _page;
            set => SetProperty(ref _page, value);
        }
        #endregion

        #region Команды

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }
        private bool CanCloseApplicationCommandExcecut(object p) => true;
        private void OnCloseApplicationCommandExecuted(object p) => Environment.Exit(0);
        #endregion

        #region Открыть список пользователей
        public ICommand OpenUsersListCommand { get; }
        private bool CanOpenUsersListCommandExcecut(object p) => true;
        private void OnOpenUsersListCommandExecuted(object p)
        {
            Content = new UsersView();
        }
        #endregion
        #region Открыть настройки
        public ICommand OpenSettingsCommand { get; }
        private bool CanOpenSettingsCommandExcecut(object p) => true;
        private void OnOpenSettingsCommandExecuted(object p)
        {
            Content = new SettingsView();
        }
        #endregion
        #region Открыть список типов криптовалют
        public ICommand OpenCryptoTypeListCommand { get; }
        private bool CanOpenCryptoTypeListCommandExcecut(object p) => true;
        private void OnOpenCryptoTypeListCommandExecuted(object p)
        {
            Content = new CryptoTypeView();
        }
        #endregion
        #region Открыть список криптовалют
        public ICommand OpenCryptoListCommand { get; }
        private bool CanOpenCryptoListCommandExcecut(object p) => true;
        private void OnOpenCryptoListCommandExecuted(object p)
        {
            Content = new CryptoView();
        }
        #endregion
        #region Открыть сообщения
        public ICommand OpenMessagesCommand { get; }
        private bool CanOpenMessagesCommandExcecut(object p) => true;
        private void OnOpenMessagesCommandExecuted(object p)
        {
            Content = new MessagesView();
        }
        #endregion
        #region Открыть проекты
        public ICommand OpenProjectListCommand { get; }
        private bool CanOpenProjectListCommandExcecut(object p) => true;
        private void OnOpenProjectListCommandExecuted(object p)
        {
            Content = new ProjectView();
        }
        #endregion
        #region Открыть участников
        public ICommand OpenParticipationPageCommand { get; }
        private bool CanOpenParticipationPageCommandExcecut(object p) => true;
        private void OnOpenParticipationPageCommandExecuted(object p)
        {
            Content = new ParticipationView();
        }
        #endregion
        #region Открыть логи
        public ICommand OpenLogPageCommand { get; }
        private bool CanOpenLogPageCommandExcecut(object p) => true;
        private void OnOpenLogPageCommandExecuted(object p)
        {
            Content = new LogView();
        }
        #endregion
        #endregion

        [Obsolete]
        public MainViewModel()
        {
            Logger.Add("Приложение запущено");
            Bot.InitializationError += BotInitializationError;
            Bot.InitializationOk += BotInitializationOk;
            Bot.Init();

            DataBase.CheckDataBaseExist();
            
            #region Команды

            CloseApplicationCommand = new RelayCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExcecut);
            OpenUsersListCommand = new RelayCommand(OnOpenUsersListCommandExecuted, CanOpenUsersListCommandExcecut);
            OpenSettingsCommand = new RelayCommand(OnOpenSettingsCommandExecuted, CanOpenSettingsCommandExcecut);
            OpenCryptoTypeListCommand = new RelayCommand(OnOpenCryptoTypeListCommandExecuted, CanOpenCryptoTypeListCommandExcecut);
            OpenCryptoListCommand = new RelayCommand(OnOpenCryptoListCommandExecuted, CanOpenCryptoListCommandExcecut);
            OpenProjectListCommand = new RelayCommand(OnOpenProjectListCommandExecuted, CanOpenProjectListCommandExcecut);
            OpenMessagesCommand = new RelayCommand(OnOpenMessagesCommandExecuted, CanOpenMessagesCommandExcecut);
            OpenLogPageCommand = new RelayCommand(OnOpenLogPageCommandExecuted, CanOpenLogPageCommandExcecut);
            OpenParticipationPageCommand = new RelayCommand(OnOpenParticipationPageCommandExecuted, CanOpenParticipationPageCommandExcecut);

            #endregion
        }

        private void BotInitializationOk()
        {
            Status = $"Подключено к @{Bot.Info.FirstName}";
            Title = $"TelegramBot Crypto @{Bot.Info.FirstName}";
            Logger.Add("Подключение к боту успешно");
        }

        private void BotInitializationError()
        {
            Content = new SettingsView();
            Title = "TelegramBot Crypto";
            Status = "Ошибка подключения";
            Logger.Add("Ошибка подключения к боту");
        }
    }
}
