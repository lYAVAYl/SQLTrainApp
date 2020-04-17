using SQLTrainApp.Model.Commands;
using SQLTrainApp.Model.Logic;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using TrainSQL_DAL;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using System.Drawing;

namespace SQLTrainApp.ViewModel
{
    class RefreshPasswordViewModel:BaseViewModel, IPageViewModel
    {
        public string UserLogin { get; set; }

        public string Code { get; set; }

        public string UserPass1 { get; set; }
        public string UserPass2 { get; set; }
        

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

        private ICommand _refreshPassword;
        public ICommand RefreshPassword
        {
            get
            {
                return _refreshPassword ?? (_refreshPassword = new RelayCommand(x =>
                {
                    // TODO: Проверка паролей на совпадение
                    SetNewPassword(UserLogin, UserPass1);
                }));
            }
        }

        private void SetNewPassword(string login, string newPass)
        {


            Mediator.Notify("LoadSignInPage", "");
        }
    }
}
