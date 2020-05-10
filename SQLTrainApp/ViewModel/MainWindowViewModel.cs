using SQLTrainApp.Model.Commands;
using SQLTrainApp.Model.Logic;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using TrainSQL_DAL;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using System.Drawing;

namespace SQLTrainApp.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        public Visibility IsHeaderVisible { get; set; } = Visibility.Hidden;

        public BitmapImage UserPhoto { get; set; }
        public string UserLogin { get; set; }

        private ICommand _loadContentsPage;
        public ICommand LoadContentsPage
        {
            get
            {
                return _loadContentsPage ?? (_loadContentsPage = new RelayCommand(x =>
                    {
                        LoadTableOfContentsPage("");
                    }));
            }
        }

        private ICommand _loadTasksPage;
        public ICommand LoadTasksPage
        {
            get
            {
                return _loadTasksPage ?? (_loadTasksPage = new RelayCommand(x =>
                {
                    LoadTableOfTasksPage("");
                }));
            }
        }

        private ICommand _loadCompliantssPage;
        public ICommand LoadCompliantsPage
        {
            get
            {
                return _loadCompliantssPage ?? (_loadCompliantssPage = new RelayCommand(x =>
                {
                    LoadTableOfComplaintsPage("");
                }));
            }
        }

        private ICommand _loadUserPage;
        public ICommand LoadUserPage
        {
            get
            {
                return _loadUserPage ?? (_loadUserPage = new RelayCommand(x =>
                  {
                      LoadUserMainPage(CurrentUser.Login);
                  }));
            }
        }

        private ICommand _exit;
        public ICommand Exit
        {
            get
            {
                return _exit ?? (_exit = new RelayCommand(x =>
                {
                    CurrentUser.RemoveData();
                    LoadSignInPage("");
                }));
            }
        }

        private ICommand _loadTest;
        public ICommand LoadTest
        {
            get
            {
                return _loadTest ?? (_loadTest = new RelayCommand(x =>
                {
                    LoadTaskDecisionPage(5);
                }));
            }
        }


        // Список страниц, доступных для отрисовки
        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
            }
        }

        // Своство, отвечающее за отображение нужно страницы
        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                _currentPageViewModel = value;
                OnPropertyChanged("CurrentPageViewModel");
            }
        }

        // Изменить текущую страницу
        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            if(viewModel.GetType() == typeof(SignInPageViewModel) 
                || viewModel.GetType() == typeof(SignOnPageViewModel)
                || viewModel.GetType() == typeof(ConfirmEmailViewModel)
                || viewModel.GetType() == typeof(RefreshPasswordViewModel)
                )
            {
                IsHeaderVisible = Visibility.Hidden;
            }
            else
            {
                IsHeaderVisible = Visibility.Visible;
            }

            CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm == viewModel);
        }
        

        /// <summary>
        /// Загрузка страницы Входа
        /// </summary>
        /// <param name="obj"></param>
        private void LoadSignInPage(object obj)
        {
            PageViewModels[0] = new SignInPageViewModel();
            ChangeViewModel(PageViewModels[0]);
        }
        /// <summary>
        /// Загрузка страницы Регистранции
        /// </summary>
        /// <param name="obj"></param>
        private void LoadSignOnPage(object obj)
        {
            PageViewModels[1] = new SignOnPageViewModel();
            ChangeViewModel(PageViewModels[1]);
        }
        /// <summary>
        /// Загрузка страницы с информацией о пользователе 
        /// </summary>
        /// <param name="obj"></param>
        private void LoadUserMainPage(object obj)
        {
            if(obj is string)
            {
                PageViewModels[2] = new UserMainPageViewModel((string)obj);
                ChangeViewModel(PageViewModels[2]);
            }

            UserPhoto = CurrentUser.Photo;
            UserLogin = CurrentUser.Login.Length > 10 ? CurrentUser.Login.Substring(0, 10) + "..." 
                                                      : CurrentUser.Login;       
        }
        /// <summary>
        /// Загрузка страницы решения заданий
        /// </summary>
        /// <param name="obj"></param>
        private void LoadTaskDecisionPage(object obj)
        {
            if (obj is Task) PageViewModels[3] = new TaskDecisionPageViewModel((Task)obj);
            else if(obj is int) PageViewModels[3] = new TaskDecisionPageViewModel((int)obj);

            ChangeViewModel(PageViewModels[3]);        
        }
        /// <summary>
        /// Загрузка страницы отправки жалобы
        /// </summary>
        /// <param name="obj"></param>
        private void LoadSendComplaintPage(object obj)
        {
            if(obj is int)
            {
                PageViewModels[4] = new SendComplaintPageViewModel((int)obj);
            }

            ChangeViewModel(PageViewModels[4]);           
        }
        /// <summary>
        /// Загрузка страницы оглавления
        /// </summary>
        /// <param name="obj"></param>
        private void LoadTableOfContentsPage(object obj)
        {
            PageViewModels[5] = new TableOfContentsPageViewModel();
            ChangeViewModel(PageViewModels[5]);
        }
        /// <summary>
        /// Загрузка страницы редактирования заданий
        /// </summary>
        /// <param name="obj"></param>
        private void LoadEditTaskPage(object obj)
        {
            if (obj is Task) PageViewModels[6] = new EditTaskPageViewModel((Task)obj);
            ChangeViewModel(PageViewModels[6]);
        }
        /// <summary>
        /// Загрузка страницы редактирования темы
        /// </summary>
        /// <param name="obj"></param>
        private void LoadEditThemePage(object obj)
        {
            if (obj is int) PageViewModels[7] = new EditThemePageViewModel((int)obj);
            ChangeViewModel(PageViewModels[7]);
        }
        /// <summary>
        /// Загрузка страницы теории главы
        /// </summary>
        /// <param name="obj"></param>
        private void LoadTheoryPage(object obj)
        {
            if(obj is int)
            {
                PageViewModels[8] = new TheoryPageViewModel((int)obj);
            }
            ChangeViewModel(PageViewModels[8]);
        }
        /// <summary>
        /// Загрузка страницы списка жалоб
        /// </summary>
        /// <param name="obj"></param>
        private void LoadTableOfComplaintsPage(object obj)
        {
            PageViewModels[9] = new TableOfComplaintsPageViewModel();
            ChangeViewModel(PageViewModels[9]);
        }
        /// <summary>
        /// Загрузка страницы заданий
        /// </summary>
        /// <param name="obj"></param>
        private void LoadTableOfTasksPage(object obj)
        {
            PageViewModels[10] = new TableOfTasksPageViewModel();
            ChangeViewModel(PageViewModels[10]);
        }
        /// <summary>
        /// Загрузка страницы обновления ппароля
        /// </summary>
        /// <param name="obj"></param>
        private void LoadRefreshPasswordPage(object obj)
        {
            PageViewModels[11] = new RefreshPasswordViewModel();
            ChangeViewModel(PageViewModels[11]);
        }
        /// <summary>
        /// Загрузка страницы подтверждения емаила
        /// </summary>
        /// <param name="obj"></param>
        private void LoadConfirmEmailPage(object obj)
        {
            PageViewModels[12] = new ConfirmEmailViewModel(obj);
            ChangeViewModel(PageViewModels[12]);
        }


        public MainWindowViewModel()
        {
            // Добавить доступные страницы и установить команды
            PageViewModels.Add(new SignInPageViewModel());              // 0 Вход
            PageViewModels.Add(new SignOnPageViewModel());              // 1 Регистрация
            PageViewModels.Add(new UserMainPageViewModel());            // 2 Пользователь
            PageViewModels.Add(new TaskDecisionPageViewModel());        // 3 Решение заданий
            PageViewModels.Add(new SendComplaintPageViewModel());       // 4 Отправка жалобы
            PageViewModels.Add(new TableOfContentsPageViewModel());     // 5 Оглавление
            PageViewModels.Add(new EditTaskPageViewModel());            // 6 Изменение задания
            PageViewModels.Add(new EditThemePageViewModel());           // 7 Изменение теории
            PageViewModels.Add(new TheoryPageViewModel());              // 8 Теория главы
            PageViewModels.Add(new TableOfComplaintsPageViewModel());   // 9 Список жалоб
            PageViewModels.Add(new TableOfTasksPageViewModel());        // 10 Список заданий
            PageViewModels.Add(new RefreshPasswordViewModel());         // 11 Обновление пароля
            PageViewModels.Add(new ConfirmEmailViewModel());          // 12 Подтверждение емаила

            // Загрузка первой страницы
            CurrentPageViewModel = PageViewModels[0];

            // Установка команд
            Mediator.Append("LoadSignOnPage", LoadSignOnPage);
            Mediator.Append("LoadSignInPage", LoadSignInPage);
            Mediator.Append("LoadUserMainPage", LoadUserMainPage);
            Mediator.Append("LoadTaskDecisionPage", LoadTaskDecisionPage);
            Mediator.Append("LoadSendComplaintPage", LoadSendComplaintPage);
            Mediator.Append("LoadTableOfContentsPage", LoadTableOfContentsPage);
            Mediator.Append("LoadEditTaskPage", LoadEditTaskPage);
            Mediator.Append("LoadEditThemePage", LoadEditThemePage);
            Mediator.Append("LoadTheoryPage", LoadTheoryPage);
            Mediator.Append("LoadTableOfCompliantsPage", LoadTableOfComplaintsPage);
            Mediator.Append("LoadTableOfTasksPage", LoadTableOfTasksPage);
            Mediator.Append("LoadRefreshPasswordPage", LoadRefreshPasswordPage);
            Mediator.Append("LoadConfirmEmailPage", LoadConfirmEmailPage);
        }

    }
}
