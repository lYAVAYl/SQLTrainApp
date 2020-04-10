using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using SQLTrainApp.View.Pages;

namespace SQLTrainApp.Model.Commands
{
    public class RegisterNewUser : CommandBase
    {
        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter)
        {
            
        }
    }
}
