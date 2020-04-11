using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLTrainApp.Model.Logic;
using SQLTrainApp.Model.Commands;
using System.Windows.Input;

namespace SQLTrainApp.ViewModel
{
    public class SignInPageViewModel : BaseViewModel, IPageViewModel
    {
        private ICommand _loadSignOnPage;
        public ICommand LoadSignOnPage
        {
            get
            {
                return _loadSignOnPage ?? (_loadSignOnPage = new RelayCommand(x =>
                {
                    Mediator.Notify("LoadSignOnPage", "");
                }));
            }
        }
    }
}
