using SQLite;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using TelegramBotCrypto.Data;
using TelegramBotCrypto.Infrastructure.Commands;

namespace TelegramBotCrypto.Models
{
    public class User
    {
        [Ignore]
        public ICommand ChangeUserStatusCommand { get; }
        private bool CanOpenUsersListCommandExcecut(object p) => true;
        private void OnChangeUserStatusCommandExecuted(object p)
        {
            DataBase.ChangeUserStatus(this);
        }

        private byte _userStatus;

        [PrimaryKey]
        public long User_Id { get; set; }
        public string User_FirstName { get; set; }
        public string User_LastName { get; set; }
        public string User_Nickname { get; set; }
        public string User_Phone { get; set; }
        public byte User_Status 
        { 
            get => _userStatus; 
            set 
            {
                _userStatus = value;
                DataBase.ChangeUserStatus(this);
            } 
        }
        public User() 
        {
            ChangeUserStatusCommand = new RelayCommand(OnChangeUserStatusCommandExecuted, CanOpenUsersListCommandExcecut);
        }
        public User(long user_Id, string user_FirstName, string user_LastName, string user_Nickname, string user_Phone, byte user_Status = (byte)Services.WC.UserStatus.User) 
        {
            ChangeUserStatusCommand = new RelayCommand(OnChangeUserStatusCommandExecuted, CanOpenUsersListCommandExcecut);
            User_Id = user_Id;
            User_FirstName = user_FirstName;
            User_LastName = user_LastName;
            User_Nickname = user_Nickname;
            User_Phone = user_Phone;
            User_Status = user_Status;
        }

    }
}
