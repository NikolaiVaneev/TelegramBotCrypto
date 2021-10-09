using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using TelegramBotCrypto.Data;
using TelegramBotCrypto.Infrastructure.Commands;
using TelegramBotCrypto.Models;
using TelegramBotCrypto.Services;

namespace TelegramBotCrypto.ViewModels.Pages
{
    class ParticipationViewModel : ViewModel
    {
        #region Список участников
        private IEnumerable<Participation> _participationList;
        public IEnumerable<Participation> ParticipationList
        {
            get => _participationList;
            set => SetProperty(ref _participationList, value);
        }
        #endregion
        #region Список проектов (Комбобокс)
        private IEnumerable<Project> _projects;
        public IEnumerable<Project> Projects
        {
            get => _projects;
            set => SetProperty(ref _projects, value);
        }
        #endregion
        #region Выбранный проект (текст)
        private string _selectedProject;
        public string SelectedProject
        {
            get => _selectedProject;
            set
            {
                SetProperty(ref _selectedProject, value);
                LoadParticipationList();
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
                LoadParticipationList();
            }
        }
        #endregion

        #region Вывести в эксель
        public ICommand BringToExcelCommand { get; }
        private bool CanBringToExcelCommandExcecut(object p) => true;
        private async void OnBringToExcelCommandExecuted(object p)
        {
            ExcelWorker.ShowAllProjectUsers(SelectedProject);
        }
        #endregion

        public ParticipationViewModel()
        {
            BringToExcelCommand = new RelayCommand(OnBringToExcelCommandExecuted, CanBringToExcelCommandExcecut);

            Projects = DataBase.GetAllProjects();
            LoadParticipationList();
        }

        private void LoadParticipationList()
        {
            ParticipationList = DataBase.GetParticipationList(SearchBar).Where(u => u.Project.Title == SelectedProject);
        }
    }
}
