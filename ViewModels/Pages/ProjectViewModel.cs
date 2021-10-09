using System.Collections.Generic;
using System.Windows.Input;
using TelegramBotCrypto.Data;
using TelegramBotCrypto.Infrastructure.Commands;
using TelegramBotCrypto.Models;
using TelegramBotCrypto.Services;
using TelegramBotCrypto.Views.Dialogs;

namespace TelegramBotCrypto.ViewModels.Pages
{
    class ProjectViewModel : ViewModel
    {
        #region Список проектов
        private List<Project> _projects;
        public List<Project> Projects
        {
            get => _projects;
            set => SetProperty(ref _projects, value);
        }
        #endregion
        #region Выбранный проект
        private Project _selectedProject;
        public Project SelectedProject
        {
            get => _selectedProject;
            set => SetProperty(ref _selectedProject, value);
        }
        #endregion

        #region Dialog
        public ICommand OpenDialogCommand { get; }

        private bool _isDialogOpen;
        private object _dialog;

        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set
            {
                if (!value) LoadProjectList();
                SetProperty(ref _isDialogOpen, value);
            }

        }

        public object Dialog
        {
            get => _dialog;
            set => SetProperty(ref _dialog, value);
        }
        private void OpenDialog(object obj)
        {
            if (obj == null)
                Dialog = new ProjectDialog(new Project());
            else
                Dialog = new ProjectDialog(obj as Project);
            IsDialogOpen = true;
        }

        #endregion
        #region Изменить проект
        public ICommand OpenProjectCommand { get; }
        private bool CanOpenProjectCommandExcecut(object p) => true;
        private void OnOpenProjectCommandExecuted(object p)
        {
            OpenDialog(SelectedProject);
        }
        #endregion

        public ProjectViewModel()
        {
            LoadProjectList();
            OpenProjectCommand = new RelayCommand(OnOpenProjectCommandExecuted, CanOpenProjectCommandExcecut);
            OpenDialogCommand = new RelayCommand(OpenDialog);
        }

        private void LoadProjectList()
        {
            Projects = DataBase.GetProjects();
        }
    }
}
