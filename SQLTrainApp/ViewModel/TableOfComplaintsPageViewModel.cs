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
    class TableOfComplaintsPageViewModel:BaseViewModel, IPageViewModel
    {
        private ComplaintsCollection _contentList;
        private bool _isAdmin;

        public ComplaintsCollection ComplaintList // здесь хранится ToC
        {
            get => _contentList;
            set
            {
                _contentList = value;
                OnPropertyChanged();
            }
        }

        public bool IsAdmin // права админа
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                OnPropertyChanged();
            }
        }

        public TableOfComplaintsPageViewModel()
        {
            if (CurrentUser.Login!="")
            {
                IsAdmin = CurrentUser.Role == "Administrator"; // сделать админом

                if (IsAdmin)
                    ComplaintList = new ComplaintsCollection(TrainSQL_Commands.GetAllComplaints());
                else ComplaintList = new ComplaintsCollection(TrainSQL_Commands.GetComplaintsByLogin(CurrentUser.Login));
            }
        }
    }



    public class ComplaintsCollection : ObservableCollection<Complaint>
    {
        public ComplaintsCollection(List<Complaint> complaints = null) : base(complaints)
        {

        }
        
        private ICommand _deleteItemCommand;
        private ICommand _openItemCommand;
        private ICommand _showUserCommand;
        private ICommand _showTaskCommand;

        // эта команда реализована и работает, остальные команды приведены как демо   
        public ICommand DeleteItemCommand => _deleteItemCommand ?? (_deleteItemCommand = new RelayCommand(parameter =>
        {
            if (parameter is Complaint item)
            {
                if (MessageBox.Show($"Вы действительно хотите удалить жалобу #{item.ComplaintID}?", "Удалить?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    TrainSQL_Commands.DeleteComplaint(item);
                    Remove(item);
                }
            }
        }));               

        public ICommand OpenItemCommand => _openItemCommand ?? (_openItemCommand = new RelayCommand(parameter =>
        {
            if (parameter is Complaint item)
            {
                if(CurrentUser.Role == "Administrator")
                    Mediator.Inform("LoadEditTaskPage", TrainSQL_Commands.GetTaskByID(item.TaskID));
                else
                    Mediator.Inform("LoadTaskDecisionPage", TrainSQL_Commands.GetTaskByID(item.TaskID));
            }
        }));

        public ICommand ShowUserCommand => _showUserCommand ?? (_showUserCommand = new RelayCommand(parameter =>
        {
            if (parameter is Complaint item)
            {
                Mediator.Inform("LoadUserMainPage", item.Login);
            }
        }));

        public ICommand ShowTaskCommand => _showTaskCommand ?? (_showTaskCommand = new RelayCommand(parameter =>
        {
            if (parameter is Complaint item)
            {
                Mediator.Inform("LoadTaskDecisionPage", TrainSQL_Commands.GetTaskByID(item.TaskID));
            }
        }));
    }
}
