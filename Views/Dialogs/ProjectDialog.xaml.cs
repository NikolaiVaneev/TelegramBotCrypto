using MaterialDesignThemes.Wpf;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using TelegramBotCrypto.Data;
using TelegramBotCrypto.Models;

namespace TelegramBotCrypto.Views.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для ProjectDialog.xaml
    /// </summary>
    public partial class ProjectDialog : UserControl
    {
        public Project Project { get; set; }

        public ProjectDialog(Project project)
        {
            DataContext = this;
            Project = project;
            InitializeComponent();

            CryptoTypes.ItemsSource = DataBase.GetAllCryptoType();
            CryptoTypes.DisplayMemberPath = "Title";
            CryptoTypes.SelectedValuePath = "Title";
            
            if (project.Id != 0)
            {
                Title.Text = "Изменить проект";
                BtnDel.Visibility = System.Windows.Visibility.Visible;
                ProjectTitle.Text = project.Title;
                ProjectMessage.Text = project.Message;


                CryptoTypes.SelectedValue = project.CryptoType.Title;
                //CryptoTypes.SelectedItem = project.CryptoType;
            }
            else
            {
                Title.Text = "Добавить проект";
                BtnDel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void Save(object sender, System.Windows.RoutedEventArgs e)
        {
            Project.Title = ProjectTitle.Text;
            Project.Message = ProjectMessage.Text;
            CryptoType cryptoType = (CryptoType)CryptoTypes.SelectedItem;
            Project.CryptoTypeId = cryptoType.Id;
            DataBase.SaveProject(Project);
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
        private void Delete(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Project.Id != 0)
            {
                DataBase.DeleteProject(Project);
            }
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
