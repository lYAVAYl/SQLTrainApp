using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using SQLTrainApp.View.Pages;

namespace SQLTrainApp.ViewModel
{
    public class MainWindowViewModel
    {
        private Page _currPage = null;
        public Page CurrentPage { get; set; } = new Page();

        public MainWindowViewModel()
        {
            CurrentPage = new SignInPage();
        }

        ICommand _loadSignOnPage = null;
        //public ICommand LoadSignOnPage() => 

    }
}
