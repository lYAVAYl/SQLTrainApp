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
    class TaskDecisionPageViewModel : BaseViewModel, IPageViewModel
    {
        public string UserQuery { get; set; }
        public List<User> ResultList { get; set; }


        private ICommand _executeCmd;
        public ICommand ExecuteCmd
        {
            get
            {
                return _executeCmd ?? (_executeCmd = new RelayCommand(x =>
                {
                    ExecuteQuery(UserQuery);
                }));
            }
        }

        private ICommand _sendCompliant;
        public ICommand SendCompliant
        {
            get
            {
                return _sendCompliant ?? (_sendCompliant = new RelayCommand(x =>
                {
                    Mediator.Notify("LoadSendComplaintPage", "");
                }));
            }
        }


        private void ExecuteQuery(string query)
        {
            using (var context = new TrainSQL_Entities())
            {
                ResultList = context.Users.ToList();
            }
        }

    }
}
