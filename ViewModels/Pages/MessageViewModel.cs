using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows.Input;
using TelegramBotCrypto.Data;
using TelegramBotCrypto.Infrastructure.Commands;
using TelegramBotCrypto.Models;
using TelegramBotCrypto.Services;

namespace TelegramBotCrypto.ViewModels.Pages
{
    internal class MessageViewModel : ViewModel
    {
        #region Текст сообщения
        private string _message;
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }
        #endregion

        #region Получатели сообщения
        public enum Recipients
        {
            Все,
            Администраторы,
            Пользователи,
        }
        /// <summary>
        /// Получатели сообщения
        /// </summary>
        public IEnumerable<Recipients> RecipientsType
        {
            get
            {
                return Enum.GetValues(typeof(Recipients)).Cast<Recipients>();
            }
        }
        #endregion
        #region Выбраный тип пользователей
        private int _selectedRecipient;
        public int SelectedRecipient
        {
            get => _selectedRecipient;
            set => SetProperty(ref _selectedRecipient, value);
        }
        #endregion
        #region Список валют (не исп)
        public IEnumerable<CryptoType> CryptoTypes
        {
            get
            {
                return DataBase.GetAllCryptoType();
            }
        }
        #endregion
        #region Выбраный тип валют (не исп)
        private CryptoType _selectedCryptoType;
        public CryptoType SelectedCryptoType
        {
            get => _selectedCryptoType;
            set => SetProperty(ref _selectedCryptoType, value);
        }
        #endregion
       
        /// Команды
        #region Отправить сообщения
        public ICommand SendMessagesCommand { get; }
        private bool CanSendMessagesCommandExcecut(object p) => true;
        private void OnSendMessagesCommandExecuted(object p)
        {
            Bot.SendMessagesAllAsync(Message, SelectedRecipient);
           // Bot.SendMessagesAllAsync(Message, SelectedCryptoType);
        }
        #endregion

        public MessageViewModel()
        {
            SendMessagesCommand = new RelayCommand(OnSendMessagesCommandExecuted, CanSendMessagesCommandExcecut);
        }
    }
}
