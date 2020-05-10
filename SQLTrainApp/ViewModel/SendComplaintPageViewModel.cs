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
    class SendComplaintPageViewModel:ValidateBaseViewModel, IPageViewModel
    {
        public int TaskNum { get; set; }
        private string _complaintComment="";
        public string CompliantComment 
        {
            get => _complaintComment;
            set
            {
                if (value.Length < 4 )
                    _complaintComment = value;
                else if (value.Substring(value.Length - 4) != "\r\n\r\n")
                {
                    _complaintComment = value;
                }
            }
        }

        public SendComplaintPageViewModel(int taskID = 1)
        {
            TaskNum = taskID;
        }

        private ICommand _cancelCompliant;
        public ICommand CancelCompliant
        {
            get
            {
                return _cancelCompliant ?? (_cancelCompliant = new RelayCommand(x =>
                {
                    Mediator.Inform("LoadTaskDecisionPage", "");
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
            Complaint complaint = new Complaint()
            {
                Login = CurrentUser.Login,
                TaskID = this.TaskNum,
                Comment = this.CompliantComment                
            };

            if (TrainSQL_Commands.SendComplaint(complaint) == null)
            {
                Mediator.Inform("LoadTaskDecisionPage", "");
            }
            else
            {
                MessageBox.Show("Что-то пошло не так...");
            }

        }

        public override string Validate(string columnName)
        {
            string error = null;

            switch (columnName) 
            {
                case nameof(CompliantComment):
                    if (string.IsNullOrEmpty(CompliantComment))
                    {
                        error = "Введите комментарий к жалобе";
                    }
                    break;
            }

            if (ErrorCollection.ContainsKey(columnName))
                ErrorCollection[columnName] = error;
            else ErrorCollection.Add(columnName, error);

            IsEnableBtn = ErrorCollection[nameof(CompliantComment)] == null;

            OnPropertyChanged(nameof(ErrorCollection));

            return error;
        }
    }
}
