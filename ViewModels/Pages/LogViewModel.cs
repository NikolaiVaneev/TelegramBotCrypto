using System;
using TelegramBotCrypto.Services;

namespace TelegramBotCrypto.ViewModels.Pages
{
    internal class LogViewModel : ViewModel
    {
        #region Лог
        private string _log;
        public string Log
        {
            set => SetProperty(ref _log, value);
            get => _log;
        }

        public LogViewModel()
        {
            Log = Logger.Logs.ToString();
            Logger.OnUpdateLog += Logger_OnUpdateLog;
        }

        private void Logger_OnUpdateLog()
        {
            Log = Logger.Logs.ToString();
        }
        #endregion
    }
}
