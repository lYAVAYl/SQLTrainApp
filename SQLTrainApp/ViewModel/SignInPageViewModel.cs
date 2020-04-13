using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLTrainApp.Model.Logic;
using SQLTrainApp.Model.Commands;
using System.Windows.Input;
using System.Windows;

namespace SQLTrainApp.ViewModel
{
    public class SignInPageViewModel : BaseViewModel, IPageViewModel 
    {
        public string myLog { get; set; }
        public string myPass { get; set; }


        private ICommand _loadSignOnPage;
        public ICommand LoadSignOnPage
        {
            get
            {
                return _loadSignOnPage ?? (_loadSignOnPage = new RelayCommand(x =>
                {
                    Mediator.Notify("LoadSignOnPage", new object[2] { CurrentUser.Login, CurrentUser.Password });
                }));
            }
        }

        private ICommand _findUser;
        public ICommand FindUser
        {
            get
            {
                return _findUser ?? (_findUser = new RelayCommand(x =>
                {
                    //Mediator.Notify("FindUser",new object[2] { myLog, myPass });
                    FindUserr(new object[2] { myLog, myPass });
                }));
            }
        }

        

        private void FindUserr(object parameters)
        {
            // Выполняется поиск пользователя в БД
            if (parameters is object[] && ((object[])parameters).Length == 2)
            {
                var objs = (object[])parameters;
                string log = objs[0].ToString();
                string pas = objs[1].ToString();

                MessageBox.Show($"Log: {log} \nPas: {pas}\n");
            }
            else return;
        }
    }
}
