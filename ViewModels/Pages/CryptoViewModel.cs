
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using TelegramBotCrypto.Data;
using TelegramBotCrypto.Infrastructure.Commands;
using TelegramBotCrypto.Models;
using TelegramBotCrypto.Services;
using TelegramBotCrypto.Views.Pages;

namespace TelegramBotCrypto.ViewModels.Pages
{
    internal class CryptoViewModel : ViewModel
    {

        #region Включение кнопки "Добавить"
        private bool _addBtnEnabled = false;
        public bool AddBtnEnabled
        {
            get => _addBtnEnabled;
            set => SetProperty(ref _addBtnEnabled, value);
        }
        #endregion
        #region Список криптовалют
        private IEnumerable<Wallet> _cryptoList;
        public IEnumerable<Wallet> CryptoList
        {
            get => _cryptoList;
            set => SetProperty(ref _cryptoList, value);
        }
        #endregion
        #region Список типов валют (Комбобокс)
        private IEnumerable<CryptoType> _cryptoTypes;
        public IEnumerable<CryptoType> CryptoTypes
        {
            get => _cryptoTypes;
            set => SetProperty(ref _cryptoTypes, value);
        }
        #endregion
        #region Выбранная валюта (текст)
        private string _selectedCryptoType;
        public string SelectedCryptoType
        {
            get => _selectedCryptoType;
            set
            {
                SetProperty(ref _selectedCryptoType, value);
                LoadCryptoList(_selectedCryptoType);

                if (value != null)
                    AddBtnEnabled = true;
                else
                    AddBtnEnabled = false;
            }
        }
        #endregion
        #region Поисковая строка
        private string _searchBar;
        public string SearchBar
        {
            get => _searchBar;
            set
            {
                SetProperty(ref _searchBar, value);
                if (SearchBar.Length > 0)
                {
                    CryptoList = CryptoList.Where(u =>  
                                                       u.Code.ToLower().Contains(SearchBar.ToLower()) ||
                                                       u.UserId.ToString().Contains(SearchBar.ToLower()));
                }
                else
                {
                    LoadCryptoList();
                }
            }
        }
        #endregion


        #region Добавить список адресов
        public ICommand AddCryptoAddressCommand { get; }
        private bool CanAddCryptoAddressCommandExcecut(object p) => true;
        private async void OnAddCryptoAddressCommandExecuted(object p)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "MS Excel files| *.xls; *.xlsx"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                await Task.Run(() =>
                {
                    CryptoType cryptoType = DataBase.GetCryptoTypeData(SelectedCryptoType);
                    List<Wallet> list = ExcelWorker.GetData(ofd.FileName, cryptoType.Id.ToString());
                    DataBase.AddCryptoAddressCollection(list);
                    LoadCryptoTypeList();
           
                });
            }


            //DataBase.AddCryptoType(CryptoType);


        }
        #endregion
        public CryptoViewModel()
        {
            LoadCryptoTypeList();
            LoadCryptoList();
            AddCryptoAddressCommand = new RelayCommand(OnAddCryptoAddressCommandExecuted, CanAddCryptoAddressCommandExcecut);

        }
        private void LoadCryptoTypeList()
        {
            CryptoTypes = DataBase.GetAllCryptoType();
        
        }
        private void LoadCryptoList(string criptoTitle = "")
        {
            CryptoList = DataBase.GetAllCryptoAddress(criptoTitle);
         
        }
    }
}
