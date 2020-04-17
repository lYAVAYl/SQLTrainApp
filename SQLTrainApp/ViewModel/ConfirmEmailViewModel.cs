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
    class ConfirmEmailViewModel:BaseViewModel, IPageViewModel
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

        private ICommand _confirm;
        public ICommand ConfirmCode
        {
            get
            {
                return _confirm ?? (_confirm = new RelayCommand(x =>
                {
                    Mediator.Notify("LoadUserMainPage", "");
                }));
            }
        }

    }
}
