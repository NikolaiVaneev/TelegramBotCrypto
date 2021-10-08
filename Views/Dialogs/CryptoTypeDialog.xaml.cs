using MaterialDesignThemes.Wpf;
using System.Windows.Controls;
using TelegramBotCrypto.Data;
using TelegramBotCrypto.Models;

namespace TelegramBotCrypto.Views.Dialogs
{

    public partial class CryptoTypeDialog : UserControl
    {
        public CryptoType CryptoType { get; set; }
        public CryptoTypeDialog(CryptoType cryptoType)
        {
            CryptoType = cryptoType;

            InitializeComponent();

            if (CryptoType.Id != 0)
            {
                Title.Text = "Изменить криптовалюту";
                BtnDel.Visibility = System.Windows.Visibility.Visible;
                CryptoTitle.Text = CryptoType.Title;
            }
            else
            {
                Title.Text = "Добавить криптовалюту";
                BtnDel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void SaveCryptoType(object sender, System.Windows.RoutedEventArgs e)
        {
            CryptoType.Title = CryptoTitle.Text;
            DataBase.SaveCryptoType(CryptoType);
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
        private void DeleteCryptoType(object sender, System.Windows.RoutedEventArgs e)
        {
            if (CryptoType.Id != 0)
            {
                DataBase.DeleteCryptoType(CryptoType);
            }
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
