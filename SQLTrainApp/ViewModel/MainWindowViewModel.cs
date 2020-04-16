using SQLTrainApp.Model.Logic;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SQLTrainApp.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        public Visibility IsHeaderVisible { get; set; } = Visibility.Hidden;

        public BitmapImage UserPhoto { get; set; }
        public string UserLogin { get; set; }


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
                || viewModel.GetType() == typeof(SignOnPageViewModel))
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
            ChangeViewModel(PageViewModels[0]);
        }
        /// <summary>
        /// Загрузка страницы Регистранции
        /// </summary>
        /// <param name="obj"></param>
        private void LoadSignOnPage(object obj)
        {
            ChangeViewModel(PageViewModels[1]);
        }
        /// <summary>
        /// Загрузка страницы с информацией о пользователе 
        /// </summary>
        /// <param name="obj"></param>
        private void LoadUserMainPage(object obj)
        {
            ChangeViewModel(PageViewModels[2]);
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
            PageViewModels.Add(new TableOfCompliantsViewModel());       // 9 Список жалоб
            PageViewModels.Add(new TableOfTasksViewModel());            // 10 Список заданий

            // Загрузка первой страницы
            CurrentPageViewModel = PageViewModels[7];

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
        }




    }
}
