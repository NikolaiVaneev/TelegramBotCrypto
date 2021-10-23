using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TelegramBotCrypto.Data;
using TelegramBotCrypto.Infrastructure.Commands;
using TelegramBotCrypto.Models;
using TelegramBotCrypto.Services;

namespace TelegramBotCrypto.ViewModels.Pages
{
    class UsersViewModel : ViewModel
    {
        private IEnumerable<User> _userList;
        public IEnumerable<User> UserList
        {
            get => _userList;
            set => SetProperty(ref _userList, value);
        }



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
                    //UserList = UserList.Where(u => u.User_Nickname.ToLower().Contains(SearchBar.ToLower()) || u.User_Id.ToString().Contains(SearchBar));
                    UserList = DataBase.GetUserList(SearchBar);
                }
                else
                {
                    InitUserListAcync();
                }
            }
        }
        #endregion

        #region Вывести пользователей в эксель
        public ICommand UserListToExcelCommand { get; }
        private bool CanUserListToExcelCommandExcecut(object p) => true;
        private void OnUserListToExcelCommandExecuted(object p)
        {
            ExcelWorker.ShowProjectUsers();
        }
        #endregion

        public UsersViewModel()
        {
            InitUserListAcync();
            UserListToExcelCommand = new RelayCommand(OnUserListToExcelCommandExecuted, CanUserListToExcelCommandExcecut);
    
        }
        private async void InitUserListAcync()
        {
            await Task.Run(() =>
            {
                List<User> users = DataBase.GetUserList();
                UserList = users;
            });
        }
    }


}
