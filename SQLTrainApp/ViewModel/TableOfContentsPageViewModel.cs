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
    class TableOfContentsPageViewModel:BaseViewModel, IPageViewModel
    {
        public string SearchedChapter { get; set; }
        public Visibility CanEdit { get; set; } = Visibility.Hidden;

        public ObservableCollection<Theme> ContentList { get; set; }

        public TableOfContentsPageViewModel()
        {
            ContentList = new ObservableCollection<Theme>(TrainSQL_Commands.GetAllThemes());
            CanEdit = CurrentUser.Role == "Administrator" ? Visibility.Visible : Visibility.Hidden;
        }

    }
}
