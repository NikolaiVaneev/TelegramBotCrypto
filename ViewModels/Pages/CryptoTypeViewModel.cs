using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Windows.Input;
using TelegramBotCrypto.Data;
using TelegramBotCrypto.Infrastructure.Commands;
using TelegramBotCrypto.Models;
using TelegramBotCrypto.Services;

namespace TelegramBotCrypto.ViewModels.Pages
{
    internal class CryptoTypeViewModel : ViewModel
    {
        private List<CryptoType> _cryptoTypes;
        public List<CryptoType> CryptoTypes
        {
            get => _cryptoTypes;
            set => SetProperty(ref _cryptoTypes, value);
        }
        #region Имя нового типа
        private string _cryptoType;
        /// <summary>Имя нового типа</summary>
        public string CryptoType
        {
            get => _cryptoType;
            set => SetProperty(ref _cryptoType, value);
        }
        #endregion
        #region Приветственное сообщение
        private string _message;
        /// <summary>Сообщение</summary>
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }
        #endregion
        #region Выбранный Item
        private CryptoType _selectedItem;
        /// <summary>Сообщение</summary>
        public CryptoType SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }
        #endregion
        #region Добавить криптовалюту
        public ICommand AddCryptoTypeCommand { get; }
        private bool CanAddCryptoTypeCommandExcecut(object p) => true;
        private void OnAddCryptoTypeCommandExecuted(object p)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
            DataBase.AddCryptoType(CryptoType, Message);
            LoadCryptoTypeList();
            CryptoType = string.Empty;
            Message = string.Empty;
        }
        #endregion

        #region Удалить криптовалюту
        public ICommand OpenCryptoType { get; }
        private bool CanOpenCryptoTypeExcecut(object p) => true;
        private void OnOpenCryptoTypeExecuted(object p)
        {
            DataBase.DeleteCryptoType(SelectedItem);
            LoadCryptoTypeList();
        }
        #endregion
        public CryptoTypeViewModel()
        {
            LoadCryptoTypeList();
            AddCryptoTypeCommand = new RelayCommand(OnAddCryptoTypeCommandExecuted, CanAddCryptoTypeCommandExcecut);
            OpenCryptoType = new RelayCommand(OnOpenCryptoTypeExecuted, CanOpenCryptoTypeExcecut);
        }
        private void LoadCryptoTypeList()
        {
            CryptoTypes = DataBase.GetAllCryptoType();
        }
    }
}
