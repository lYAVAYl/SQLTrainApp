using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using SQLTrainApp.Model.Commands;
using SQLTrainApp.Model.Logic;

namespace SQLTrainApp.ViewModel
{
    public class MainWindowViewModel:BaseViewModel
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
        /// Загрузка страницы Регистранции
        /// </summary>
        /// <param name="obj"></param>
        private void LoadSignOnPage(object obj)
        {
            ChangeViewModel(PageViewModels[1]);
        }
        /// <summary>
        /// Загрузка страницы Входа
        /// </summary>
        /// <param name="obj"></param>
        private void LoadSignInPage(object obj)
        {
            ChangeViewModel(PageViewModels[0]);
        }

        public MainWindowViewModel()
        {
            // Добавить доступные страницы и установить команды
            PageViewModels.Add(new SignInPageViewModel());
            PageViewModels.Add(new SignOnPageViewModel());

            // Загрузка первой страницы
            CurrentPageViewModel = PageViewModels[0];

            // Установка команд
            Mediator.Subscribe("LoadSignOnPage", LoadSignOnPage);
            Mediator.Subscribe("LoadSignInPage", LoadSignInPage);
        }




    }
}
