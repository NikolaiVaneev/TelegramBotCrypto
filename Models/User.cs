using SQLite;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TelegramBotCrypto.Data;


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
        /// <summary>
        /// 1 - админ, 0 - пользователь
        /// </summary>
        public byte User_Status
        {
            get => _userStatus;
            set => SetProperty(ref _userStatus, value);
        }

        private byte _adminMessage;
        public byte AdminMessage
        {
            get => _adminMessage;
            set => SetProperty(ref _adminMessage, value);
        }

        public long ReferId { get; set; }
        public string PaymentDetail { get; set; }

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
