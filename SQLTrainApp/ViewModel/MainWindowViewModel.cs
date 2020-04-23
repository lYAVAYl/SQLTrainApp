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
                    LoadTableOfCompliantsPage("");
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
                      LoadUserMainPage("");
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
                    LoadTaskDecisionPage("");
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
            ChangeViewModel(new SignOnPageViewModel());
        }
        /// <summary>
        /// Загрузка страницы с информацией о пользователе 
        /// </summary>
        /// <param name="obj"></param>
        private void LoadUserMainPage(object obj)
        {
            ChangeViewModel(new UserMainPageViewModel());
            UserPhoto = CurrentUser.Photo;
            UserLogin = CurrentUser.Login.Length>10? CurrentUser.Login.Substring(0,10)+"...": CurrentUser.Login;
            
        }
        /// <summary>
        /// Загрузка страницы решения заданий
        /// </summary>
        /// <param name="obj"></param>
        private void LoadTaskDecisionPage(object obj)
        {
            ChangeViewModel(PageViewModels[3]);        
        }
        /// <summary>
        /// Загрузка страницы отправки жалобы
        /// </summary>
        /// <param name="obj"></param>
        private void LoadSendComplaintPage(object obj)
        {
            ChangeViewModel(PageViewModels[4]);           
        }
        /// <summary>
        /// Загрузка страницы оглавления
        /// </summary>
        /// <param name="obj"></param>
        private void LoadTableOfContentsPage(object obj)
        {
            ChangeViewModel(PageViewModels[5]);
        }
        /// <summary>
        /// Загрузка страницы редактирования заданий
        /// </summary>
        /// <param name="obj"></param>
        private void LoadEditTaskPage(object obj)
        {
            ChangeViewModel(PageViewModels[6]);
        }
        /// <summary>
        /// Загрузка страницы редактирования темы
        /// </summary>
        /// <param name="obj"></param>
        private void LoadEditThemePage(object obj)
        {
            ChangeViewModel(PageViewModels[7]);
        }
        /// <summary>
        /// Загрузка страницы теории главы
        /// </summary>
        /// <param name="obj"></param>
        private void LoadTheoryPage(object obj)
        {
            ChangeViewModel(PageViewModels[8]);
        }
        /// <summary>
        /// Загрузка страницы списка жалоб
        /// </summary>
        /// <param name="obj"></param>
        private void LoadTableOfCompliantsPage(object obj)
        {
            ChangeViewModel(PageViewModels[9]);
        }
        /// <summary>
        /// Загрузка страницы заданий
        /// </summary>
        /// <param name="obj"></param>
        private void LoadTableOfTasksPage(object obj)
        {
            ChangeViewModel(PageViewModels[10]);
        }
        /// <summary>
        /// Загрузка страницы обновления ппароля
        /// </summary>
        /// <param name="obj"></param>
        private void LoadRefreshPasswordPage(object obj)
        {
            ChangeViewModel(new RefreshPasswordViewModel());
        }
        /// <summary>
        /// Загрузка страницы подтверждения емаила
        /// </summary>
        /// <param name="obj"></param>
        private void LoadConfirmEmailPage(object obj)
        {            
            ChangeViewModel(new ConfirmEmailViewModel(obj));
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
            PageViewModels.Add(new EditThemePageViewModel(17));           // 7 Изменение теории
            PageViewModels.Add(new TheoryPageViewModel());              // 8 Теория главы
            PageViewModels.Add(new TableOfCompliantsViewModel());       // 9 Список жалоб
            PageViewModels.Add(new TableOfTasksViewModel());            // 10 Список заданий
            PageViewModels.Add(new RefreshPasswordViewModel());         // 11 Обновление пароля
            //PageViewModels.Add(new ConfirmEmailViewModel());            // 12 Подтверждение емаила

            // Загрузка первой страницы
            CurrentPageViewModel = PageViewModels[0];

            // Установка команд
            Mediator.Subscribe("LoadSignOnPage", LoadSignOnPage);
            Mediator.Subscribe("LoadSignInPage", LoadSignInPage);
            Mediator.Subscribe("LoadUserMainPage", LoadUserMainPage);
            Mediator.Subscribe("LoadTaskDecisionPage", LoadTaskDecisionPage);
            Mediator.Subscribe("LoadSendComplaintPage", LoadSendComplaintPage);
            Mediator.Subscribe("LoadTableOfContentsPage", LoadTableOfContentsPage);
            Mediator.Subscribe("LoadEditTaskPage", LoadEditTaskPage);
            Mediator.Subscribe("LoadEditThemePage", LoadEditThemePage);
            Mediator.Subscribe("LoadTheoryPage", LoadTheoryPage);
            Mediator.Subscribe("LoadTableOfCompliantsPage", LoadTableOfCompliantsPage);
            Mediator.Subscribe("LoadTableOfTasksPage", LoadTableOfTasksPage);
            Mediator.Subscribe("LoadRefreshPasswordPage", LoadRefreshPasswordPage);
            Mediator.Subscribe("LoadConfirmEmailPage", LoadConfirmEmailPage);
        }

    }
}
