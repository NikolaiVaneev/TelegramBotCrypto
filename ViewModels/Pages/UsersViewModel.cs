using System.Collections.Generic;
using System.Linq;
using TelegramBotCrypto.Data;
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
                    UserList = UserList.Where(u => u.User_Nickname.ToLower().Contains(SearchBar.ToLower()) || u.User_Id.ToString().Contains(SearchBar));
                }
                else
                {
                    InitUserList();
                }
            }
        }
        #endregion

        public UsersViewModel()
        {
            InitUserList();
        }
        private void InitUserList()
        {
            UserList = DataBase.GetUserList();
        }
    }

    
}
