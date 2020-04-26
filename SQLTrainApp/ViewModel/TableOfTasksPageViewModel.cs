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
    class TableOfTasksPageViewModel:BaseViewModel, IPageViewModel
    {
        private TableOfTasksViewModel _taskList;
        private bool _isAdmin;

        public TableOfTasksViewModel TaskList // здесь хранится ToC
        {
            get => _taskList;
            set
            {
                _taskList = value;
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

        public TableOfTasksPageViewModel()
        {
            IsAdmin = CurrentUser.Role == "Administrator"; // сделать админом

            TaskList = new TableOfTasksViewModel(TrainSQL_Commands.GetAllTasks());
        }

        private ICommand _addItem;
        public ICommand AddItem => _addItem ?? (_addItem = new RelayCommand(parameter =>
        {
            Mediator.Notify("LoadEditTaskPage", new Task()
                                                    {
                                                        TaskID = -1,
                                                        TaskText = "",
                                                        RightAnswer = ""
                                                    }
            );
        }));



        //public string SearchedTask { get; set; }
        //public Visibility CanEdit { get; set; } = Visibility.Hidden;

        //public ObservableCollection<Task> TaskList { get; set; }

        //public TableOfTasksPageViewModel()
        //{
        //    TaskList = new ObservableCollection<Task>(TrainSQL_Commands.GetAllTasks());
        //    CanEdit = CurrentUser.Role == "Administrator" ? Visibility.Visible : Visibility.Hidden;
        //}
    }


    public class TableOfTasksViewModel : ObservableCollection<Task>
    {
        public TableOfTasksViewModel(List<Task> themes = null) : base(themes)
        {

        }
        

        private ICommand _deleteItemCommand;
        private ICommand _editItemCommand;
        private ICommand _openItemCommand;

        // эта команда реализована и работает, остальные команды приведены как демо   
        public ICommand DeleteItemCommand => _deleteItemCommand ?? (_deleteItemCommand = new RelayCommand(parameter =>
        {
            if (parameter is Task item)
            {
                if (MessageBox.Show(item.ToString(), "Удалить?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {                    
                    TrainSQL_Commands.DeleteTask(item);
                    Remove(item);
                }
            }
        }));

        public ICommand EditItemCommand => _editItemCommand ?? (_editItemCommand = new RelayCommand(parameter =>
        {
            if (parameter is Task item)
            {
                Mediator.Notify("LoadEditTaskPage", item);
            }
        }));

        public ICommand OpenItemCommand => _openItemCommand ?? (_openItemCommand = new RelayCommand(parameter =>
        {
            if (parameter is Task item)
            {
                Mediator.Notify("LoadTaskDecisionPage", item);
            }
        }));
    }
}
