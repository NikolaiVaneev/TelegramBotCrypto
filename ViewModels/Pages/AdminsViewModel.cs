using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using TelegramBotCrypto.Data;
using TelegramBotCrypto.Infrastructure.Commands;
using TelegramBotCrypto.Models;
using TelegramBotCrypto.Services;

namespace TelegramBotCrypto.ViewModels.Pages
{
    class AdminsViewModel : ViewModel
    {

        public AdminsViewModel()
        {
            InitAdminListAcync();
            UserListToExcelCommand = new RelayCommand(OnUserListToExcelCommandExecuted, CanUserListToExcelCommandExcecut);
        }

        #region Вывести пользователей в эксель
        public ICommand UserListToExcelCommand { get; }
        private bool CanUserListToExcelCommandExcecut(object p) => true;
        private void OnUserListToExcelCommandExecuted(object p)
        {
            ExcelWorker.ShowProjectUsers();
        }
        #endregion

        private async void InitAdminListAcync()
        {
            await Task.Run(() =>
            {
                List<User> users = DataBase.GetAdminAll();
                UserList = users;
            });
        }

        private IEnumerable<User> _userList;
        public IEnumerable<User> UserList
        {
            get => _userList;
            set => SetProperty(ref _userList, value);
        }
    }
}
