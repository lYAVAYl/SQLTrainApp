using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SQLTrainApp.Model.Commands;
using SQLTrainApp.Model.Logic;
namespace SQLTrainApp.ViewModel
{
    class SignOnPageViewModel:BaseViewModel, IPageViewModel
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
