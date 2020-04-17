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
    class EditThemePageViewModel:BaseViewModel, IPageViewModel
    {
        public int ThemeID { get; set; }
        public string ThemeName { get; set; }
        public string ThemeInfo { get; set; }


        private ICommand _saveChanges;
        public ICommand SaveChanges
        {
            get
            {
                return _saveChanges ?? (_saveChanges = new RelayCommand(x =>
                {
                    Save();
                }));
            }
        }
        private void Save()
        {
            MessageBox.Show("Изменения сохранены!");
        }

        private ICommand _cancelChanges;
        public ICommand CancelChanges
        {
            get
            {
                return _cancelChanges ?? (_cancelChanges = new RelayCommand(x =>
                {
                    Cancel();
                }));
            }
        }
        private void Cancel()
        {
            MessageBox.Show("Изменения отменены");
        }
    }
}
