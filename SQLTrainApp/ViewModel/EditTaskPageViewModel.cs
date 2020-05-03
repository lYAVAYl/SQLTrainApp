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

using wForms = System.Windows.Forms;
using System.Collections.ObjectModel;

namespace SQLTrainApp.ViewModel
{
    class EditTaskPageViewModel:ValidateBaseViewModel, IPageViewModel
    {
        public int TaskNum { get; set; }
        public string TaskInfo { get; set; }
        public string RightQuery { get; set; }

        public List<Sheme> EnableDBs { get; set; }
        public ObservableCollection<Complaint> TaskComplaints { get; set; }
        private Task _task;

        public EditTaskPageViewModel(Task task = null)
        {
            if (task != null)
            {
                _task = task;

                TaskNum = _task.TaskID;
                TaskInfo = _task.TaskText;
                RightQuery = _task.RightAnswer;

                TaskComplaints = new ObservableCollection<Complaint>(TrainSQL_Commands.GetComplaintsByTask(_task.TaskID));
            }

        }

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

            wForms.DialogResult result = wForms.MessageBox.Show("Вы действительно хотите сохранить изменения?", "Подтверждение действия", wForms.MessageBoxButtons.YesNo, wForms.MessageBoxIcon.Question);

            if (result == wForms.DialogResult.Yes)
            {
                _task.TaskText = TaskInfo;
                _task.RightAnswer = RightQuery;
                _task.dbID = 1;

                if (TrainSQL_Commands.EditORAddTask(_task) == null)
                {
                    MessageBox.Show("Изменения сохранены!");
                    Mediator.Notify("LoadTableOfTasksPage", "");
                }
                else
                {
                    MessageBox.Show("Что-то пошло не так");
                }
            }
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
            wForms.DialogResult result = wForms.MessageBox.Show("Вы действительно хотите отменить изменения?", "Подтверждение действия", wForms.MessageBoxButtons.YesNo, wForms.MessageBoxIcon.Question);

            if (result == wForms.DialogResult.Yes)
            {
                Mediator.Notify("LoadTableOfTasksPage", "");
            }

        }

        public override string Validate(string columnName)
        {
            string error = null;
            switch (columnName)
            {                
                case nameof(TaskInfo):
                    if (string.IsNullOrEmpty(TaskInfo))
                    {
                        error = "Отсутствует текст задания";
                    }
                    break;
                case nameof(RightQuery):
                    if (string.IsNullOrEmpty(RightQuery))
                    {
                        error = "Отсутствует Запров-решение";
                    }
                    break;
            }


            if (ErrorCollection.ContainsKey(columnName))
                ErrorCollection[columnName] = error;
            else ErrorCollection.Add(columnName, error);


            IsEnableBtn = ErrorCollection[nameof(TaskInfo)] == null
                          && ErrorCollection[nameof(RightQuery)] == null;

            OnPropertyChanged(nameof(ErrorCollection));

            return error;
        }

    }
}
