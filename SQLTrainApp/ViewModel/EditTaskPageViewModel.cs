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
    class EditTaskPageViewModel:BaseViewModel, IPageViewModel
    {
        public int TaskNum { get; set; }
        public string TaskInfo { get; set; }
        public string RightQuery { get; set; }

        public List<Sheme> EnableDBs { get; set; }

        private ICommand _executeQuery;
        public ICommand ExecuteQuery
        {
            get
            {
                return _executeQuery ?? (_executeQuery = new RelayCommand(x =>
                  {
                      Execute(RightQuery);
                  }));
            }
        }
        private void Execute(string query)
        {
            MessageBox.Show("Выполнение запроса...");
        }

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
