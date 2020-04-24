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
using System.Collections.ObjectModel;

namespace SQLTrainApp.ViewModel
{
    class TableOfCompliantsViewModel:BaseViewModel, IPageViewModel
    {
        public string SearchedCompliant { get; set; }
        public Visibility CanEdit { get; set; } = Visibility.Hidden;

        public ObservableCollection<Complaint> ComplaintList { get; set; }

        public TableOfCompliantsViewModel()
        {
            if (CurrentUser.Login!="")
            {
                if (CurrentUser.Role == "Administrator")
                {
                    ComplaintList = new ObservableCollection<Complaint>(TrainSQL_Commands.GetAllComplaints());
                    CanEdit = Visibility.Visible;
                }
                else
                {
                    ComplaintList = new ObservableCollection<Complaint>(TrainSQL_Commands.GetUsersCompliants(CurrentUser.Login));
                }
            }                
        }
    }
}
