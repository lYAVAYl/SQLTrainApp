using SQLTrainApp.Model.Commands;
using SQLTrainApp.Model.Logic;
using System.Windows.Input;
namespace SQLTrainApp.ViewModel
{
    class SignOnPageViewModel : BaseViewModel, IPageViewModel
    {
        private ICommand _loadSignInPage;
        public ICommand LoadSignInPage
        {
            get
            {
                return _loadSignInPage ?? (_loadSignInPage = new RelayCommand(x =>
                {
                    Mediator.Notify("LoadSignInPage", "");
                }));
            }
        }
    }
}
