using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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


    }
}
