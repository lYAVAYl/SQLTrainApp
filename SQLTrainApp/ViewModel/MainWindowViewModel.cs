using SQLTrainApp.Model.Logic;
using System.Collections.Generic;
using System.Linq;

namespace SQLTrainApp.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region My Shitcode
        //private Page _currPage = null;
        //public Page CurrentPage { get; set; } = new Page();

        //public MainWindowViewModel()
        //{
        //    CurrentPage = new SignInPage();
        //}

        //ICommand _loadSignOnPage = null;
        //public ICommand LoadSignOnPageCmd(object parametr) 
        //    => _loadSignOnPage ?? (_loadSignOnPage = new LoadSignOnPage());
        #endregion

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

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
            //ChangeViewModel(PageViewModels[2]);
            ChangeViewModel(PageViewModels[3]);
        }
        /// <summary>
        /// Загрузка страницы решения заданий
        /// </summary>
        /// <param name="obj"></param>
        private void LoadTaskDecisionPage(object obj)
        {
            ChangeViewModel(PageViewModels[3]);
        }

        public MainWindowViewModel()
        {
            // Добавить доступные страницы и установить команды
            PageViewModels.Add(new SignInPageViewModel());
            PageViewModels.Add(new SignOnPageViewModel());
            PageViewModels.Add(new UserMainPageViewModel());
            PageViewModels.Add(new TaskDecisionPageViewModel());

            // Загрузка первой страницы
            CurrentPageViewModel = PageViewModels[3];

            // Установка команд
            Mediator.Subscribe("LoadSignOnPage", LoadSignOnPage);
            Mediator.Subscribe("LoadSignInPage", LoadSignInPage);
            Mediator.Subscribe("LoadUserMainPage", LoadUserMainPage);
            Mediator.Subscribe("LoadTaskDecisionPage", LoadTaskDecisionPage);
        }




    }
}
