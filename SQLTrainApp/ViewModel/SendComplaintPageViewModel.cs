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
        public int TaskNum { get; set; }
        public string CompliantComment { get; set; }

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

        private ICommand _sendCompliantCmd;
        public ICommand SendCompliantCmd
        {
            get
            {
                return _sendCompliantCmd ?? (_sendCompliantCmd = new RelayCommand(x =>
                {
                    SendCompliant();
                }));
            }
        }

        private void SendCompliant()
        {
            Mediator.Notify("LoadTaskDecisionPage", "");
        }

    }
}
