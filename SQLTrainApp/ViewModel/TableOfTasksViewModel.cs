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
    class TableOfTasksViewModel:BaseViewModel, IPageViewModel
    {
        public string SearchedTask { get; set; }
        public Visibility CanEdit { get; set; } = Visibility.Hidden;

        public ObservableCollection<Task> TaskList { get; set; }

        public TableOfTasksViewModel()
        {
            TaskList = new ObservableCollection<Task>(TrainSQL_Commands.GetAllTasks());
            CanEdit = CurrentUser.Role == "Administrator" ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
