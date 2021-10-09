using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Windows.Input;
using TelegramBotCrypto.Data;
using TelegramBotCrypto.Infrastructure.Commands;
using TelegramBotCrypto.Models;
using TelegramBotCrypto.Services;
using TelegramBotCrypto.Views.Dialogs;

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
            LoadCryptoTypeList();
        }
        #endregion

        #region Изменить криптовалюту
        public ICommand OpenCryptoType { get; }
        private bool CanOpenCryptoTypeExcecut(object p) => true;
        private void OnOpenCryptoTypeExecuted(object p)
        {
            OpenDialog(SelectedItem);
        }
        #endregion


        #region Dialog

        public ICommand OpenDialogCommand { get; }

        private bool _isDialogOpen;
        private object _dialog;

        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set
            {
                if (!value)
                {
                    LoadCryptoTypeList();
                }

                SetProperty(ref _isDialogOpen, value);
            }

        }
        public object Dialog
        {
            get => _dialog;
            set => SetProperty(ref _dialog, value);
        }

        private void OpenDialog(object obj)
        {
            if (obj == null)
                Dialog = new CryptoTypeDialog(new CryptoType());
            else
                Dialog = new CryptoTypeDialog(obj as CryptoType);
            IsDialogOpen = true;
        }

        #endregion

        public CryptoTypeViewModel()
        {
            LoadCryptoTypeList();
            AddCryptoTypeCommand = new RelayCommand(OnAddCryptoTypeCommandExecuted, CanAddCryptoTypeCommandExcecut);
            OpenCryptoType = new RelayCommand(OnOpenCryptoTypeExecuted, CanOpenCryptoTypeExcecut);

            OpenDialogCommand = new RelayCommand(OpenDialog);


        }
        private void LoadCryptoTypeList()
        {
            CryptoTypes = DataBase.GetAllCryptoType();
        }
    }
}
