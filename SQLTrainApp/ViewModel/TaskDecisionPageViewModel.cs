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
using System.Data;

namespace SQLTrainApp.ViewModel
{
    class TaskDecisionPageViewModel : BaseViewModel, IPageViewModel
    {
        public string TaskInfo { get; set; }
        public string ResultStr { get; set; }
        public string DBInfo { get; set; }
        public BitmapImage DBImage { get; set; }
        public bool IsRightResult { get; set; }

        public string UserQuery { get; set; }
       // public List<User> ResultList { get; set; }
        public DataSet ResultList { get; set; }
        
        public TaskDecisionPageViewModel()
        {
            IsRightResult = false;
        }

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

        private ICommand _loadNextTask;
        public ICommand LoadNextTask
        {
            get
            {
                return _loadNextTask ?? (_loadNextTask = new RelayCommand(x =>
                {
                    LoadTask();
                }));
            }
        }


        private void ExecuteQuery(string query)
        {

            //using (var context = new TrainSQL_Entities())
            //{
            //    ResultList = context.Users.ToList();
            //}

            
            MessageBox.Show("Вывод результата запроса...");
        }

        private void LoadTask()
        {
            MessageBox.Show("Загрузка следующего задания");
        }

    }
}
