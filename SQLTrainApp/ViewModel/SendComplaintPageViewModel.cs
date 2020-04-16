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
    class SendComplaintPageViewModel:BaseViewModel, IPageViewModel
    {
        private ICommand _cancelCompliant;
        public ICommand CancelCompliant
        {
            get
            {
                return _cancelCompliant ?? (_cancelCompliant = new RelayCommand(x =>
                {
                    Mediator.Notify("LoadTaskDecisionPage", "");
                }));
            }
        }
    }
}
