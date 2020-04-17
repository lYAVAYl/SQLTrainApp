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
    public class UserMainPageViewModel : BaseViewModel, IPageViewModel
    {
        public BitmapImage UserPhoto { get { return CurrentUser.Photo; } } 
        public string UserLogin { get { return CurrentUser.Login; } }
        public string UserEmail { get { return CurrentUser.Email;} } 
        public string UserRole { get { return CurrentUser.Role;} }


    }
}
