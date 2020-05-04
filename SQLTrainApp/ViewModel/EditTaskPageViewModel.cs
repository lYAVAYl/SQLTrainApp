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
using System.Windows.Media.Animation;




using wForms = System.Windows.Forms;
using System.Collections.ObjectModel;

namespace SQLTrainApp.ViewModel
{
    class EditTaskPageViewModel:ValidateBaseViewModel, IPageViewModel
    {
        public int TaskNum { get; set; }
        private string _taskInfo;
        public string TaskInfo
        {
            get => _taskInfo;
            set
            {
                if (value.Length < 4)
                    _taskInfo = value;
                else if (value.Substring(value.Length - 4) != "\r\n\r\n")
                {
                    _taskInfo = value;
                }
            }
        }
        private string _rightQuery;
        public string RightQuery
        {
            get => _rightQuery;
            set
            {
                if (value.Length < 4
                    || value.Substring(value.Length - 4) != "\r\n\r\n")
                {
                    _rightQuery = value;
                    IsEnableBtn = false;
                }
                    
            }
        }       
        public List<string> EnableDBs { get; set; }
        public string SelectedDB { get; set; }
        public ObservableCollection<Complaint> TaskComplaints { get; set; }

        public DataTable ResultList { get; set; } // Вывод в запроса
        public string ErrorMsg { get; set; }

        private Task _task;
        private List<TestDatabas> testDBs;
        private bool allCorrect = false;
        private string lastCorrectVersion;

        public EditTaskPageViewModel(Task task = null)
        {
            if (task != null)
            {
                _task = task;

                TaskNum = _task.TaskID;
                TaskInfo = _task.TaskText;
                RightQuery = _task.RightAnswer;

                testDBs = TrainSQL_Commands.GetDatabases();

                if(testDBs!=null && testDBs.Count != 0)
                {
                    EnableDBs = (from t in testDBs
                                 select t.dbName).ToList();

                    if (_task.TaskID != 0)
                        SelectedDB = testDBs.FirstOrDefault(x => x.dbID == _task.dbID).dbName;
                    else SelectedDB = testDBs[0].dbName;
                }


                if (_task.TaskID != 0)
                {
                    TaskComplaints = new ObservableCollection<Complaint>(TrainSQL_Commands.GetComplaintsByTask(_task.TaskID));
                }
            }

        }

        private ICommand _hideError;
        public ICommand HideError
        {
            get
            {
                return _hideError ?? (_hideError = new RelayCommand(x =>
                {

                    if (x is Grid grid)
                    {
                        var opacityAnim = new DoubleAnimation()
                        {
                            From = 1.0,
                            To = 0.0,
                            Duration = new Duration(new TimeSpan(5 * 1000000))
                        };

                        grid.BeginAnimation(Grid.OpacityProperty, opacityAnim);

                        grid.Margin = new Thickness(10, 10, 10, -150);
                    }
                }));
            }
        }

        private ICommand _executeCmd;
        public ICommand ExecuteCmd
        {
            get
            {
                return _executeCmd ?? (_executeCmd = new RelayCommand(x =>
                {
                    if (!string.IsNullOrEmpty(RightQuery))
                    {
                        object[] values = ExecuteQuery(RightQuery, testDBs.FirstOrDefault(t => t.dbName == SelectedDB).dbID);
                        if (values != null
                            && x is Grid grid)
                        {
                            ResultList = (DataTable)values[0];

                            if ((string)values[1] != null)
                            {
                                allCorrect = false;

                                ErrorMsg = (string)values[1];
                                Animate(grid);
                            }
                            else
                            {
                                allCorrect = true;
                                lastCorrectVersion = RightQuery;
                            }
                        }

                        IsEnableBtn = ErrorCollection[nameof(TaskInfo)] == null
                                      && ErrorCollection[nameof(RightQuery)] == null
                                      && allCorrect;
                    }

                }));
            }
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
            if (lastCorrectVersion!=RightQuery)
            {
                MessageBox.Show("Выполните запрос без ошибок для сохрания задания");
                IsEnableBtn = false;
                return;
            }

            wForms.DialogResult result = wForms.MessageBox.Show("Вы действительно хотите сохранить изменения?", "Подтверждение действия", wForms.MessageBoxButtons.YesNo, wForms.MessageBoxIcon.Question);

            if (result == wForms.DialogResult.Yes)
            {
                _task.TaskText = TaskInfo;
                _task.RightAnswer = RightQuery;
                _task.dbID = testDBs.FirstOrDefault(t => t.dbName == SelectedDB).dbID;

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
                          && ErrorCollection[nameof(RightQuery)] == null
                          && allCorrect;

            OnPropertyChanged(nameof(ErrorCollection));

            return error;
        }

        private object[] ExecuteQuery(string query, int dbID)
        {
            object res = TrainSQL_Commands.TaskChecking(query, dbID);
            if (res is object[] values
                && values.Length == 2)
            {
                return values;
            }

            return null;
        }

        private void Animate(Grid grid)
        {
            // Старт анимации
            var marginAnim = new ThicknessAnimation()
            {
                From = new Thickness(10, 10, 10, 0),
                To = new Thickness(10, 10, 10, 10),
            };
            Storyboard.SetTarget(marginAnim, grid);
            Storyboard.SetTargetProperty(marginAnim, new PropertyPath(Grid.MarginProperty));

            var opacityAnim = new DoubleAnimation()
            {
                From = 0.0,
                To = 1.0
            };
            Storyboard.SetTarget(opacityAnim, grid);
            Storyboard.SetTargetProperty(opacityAnim, new PropertyPath(Grid.OpacityProperty));


            Storyboard storyboard = new Storyboard();
            storyboard.Children = new TimelineCollection { marginAnim, opacityAnim };

            storyboard.Duration = new Duration(new TimeSpan(0, 0, 1));

            grid.BeginStoryboard(storyboard);
        }
    }
}
