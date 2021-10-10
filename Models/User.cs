using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TelegramBotCrypto.Data;
using TelegramBotCrypto.Infrastructure.Commands;


namespace TelegramBotCrypto.Models
{
    public class User : INotifyPropertyChanged
    {


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
            set => SetProperty(ref _userStatus, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }
                
            storage = value;
            OnPropertyChanged(propertyName);
            DataBase.ChangeUserStatusAcync(this);
            return true;
        }
    }
}
