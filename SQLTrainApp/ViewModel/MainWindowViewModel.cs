using SQLTrainApp.Model.Logic;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace SQLTrainApp.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;


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

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
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
            ChangeViewModel(PageViewModels[3]);
            UserPhoto = CurrentUser.Photo;
            UserLogin = CurrentUser.Login;
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


        public MainWindowViewModel()
        {
            // Добавить доступные страницы и установить команды
            PageViewModels.Add(new SignInPageViewModel());          // 0 Вход
            PageViewModels.Add(new SignOnPageViewModel());          // 1 Регистрация
            PageViewModels.Add(new UserMainPageViewModel());        // 2 Пользователь
            PageViewModels.Add(new TaskDecisionPageViewModel());    // 3 Решение заданий
            PageViewModels.Add(new SendComplaintPageViewModel());   // 4 Отправка жалобы

            // Загрузка первой страницы
            CurrentPageViewModel = PageViewModels[0];

            // Установка команд
            Mediator.Subscribe("LoadSignOnPage", LoadSignOnPage);
            Mediator.Subscribe("LoadSignInPage", LoadSignInPage);
            Mediator.Subscribe("LoadUserMainPage", LoadUserMainPage);
            Mediator.Subscribe("LoadTaskDecisionPage", LoadTaskDecisionPage);
            Mediator.Subscribe("LoadSendComplaintPage", LoadSendComplaintPage);
        }




    }
}
