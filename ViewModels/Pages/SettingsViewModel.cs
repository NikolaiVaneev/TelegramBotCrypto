using System.Windows.Input;
using TelegramBotCrypto.Infrastructure.Commands;
using TelegramBotCrypto.Models;
using TelegramBotCrypto.Services;

namespace TelegramBotCrypto.ViewModels.Pages
{
    internal class SettingsViewModel : ViewModel
    {
        private Settings _settings;
        public Settings Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        #region Сохранить настройки
        public ICommand SaveSettingsCommand { get; }
        private bool CanSaveSettingsCommandExcecut(object p) => true;


        private void OnSaveSettingsCommandExecuted(object p)
        {
            Settings.Save();
            Bot.Init();
        }
        #endregion

        public SettingsViewModel()
        {
            Settings = new Settings().Load();
            SaveSettingsCommand = new RelayCommand(OnSaveSettingsCommandExecuted, CanSaveSettingsCommandExcecut);
        }
    }
}
