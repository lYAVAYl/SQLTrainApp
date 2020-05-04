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

namespace SQLTrainApp.ViewModel
{
    class TaskDecisionPageViewModel : BaseViewModel, IPageViewModel
    {
        public string TaskInfo { get; set; } // Текст задания
        public string DBInfo { get; set; } // Информация о БД
        public BitmapImage DBImage { get; set; } // Схема БД
        public bool IsRightResult { get; set; } = false; // IsEnable для кнопки Дальше
        public bool EnableSkip { get; set; } = true; // IsEnable для кнопки Пропустить
        public string UserQuery { get; set; } // Запрос пользователя

        public DataTable ResultList { get; set; } // Вывод запроса
        public string ErrorMsg { get; set; }

        private int _rightAnswersCount = 0;
        private List<Task> tasks = new List<Task>();
        private int _index = 0;
        private Sheme _sheme;
        
        public TaskDecisionPageViewModel(int count = 5)
        {
            tasks = TrainSQL_Commands.GetTestList(count);
            LoadTask(tasks[_index]);
        }

        public TaskDecisionPageViewModel(Task task)
        {
            tasks.Add(task);
            LoadTask(tasks[_index]);

        }

        private ICommand _executeCmd;
        public ICommand ExecuteCmd
        {
            get
            {
                return _executeCmd ?? (_executeCmd = new RelayCommand(x =>
                {
                    if (!string.IsNullOrEmpty(UserQuery))
                    {
                        object[] values = ExecuteQuery(UserQuery);
                        if (values != null
                            && x is Grid grid)
                        {
                            ResultList = (DataTable)values[0];

                            if ((string)values[1] == null)
                            {
                                if (!IsRightResult)
                                {
                                    IsRightResult = true;
                                    _rightAnswersCount++;
                                }
                            }
                            else
                            {
                                ErrorMsg = (string)values[1];
                                Animate(grid);
                            }
                        }
                    }

                }));
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

                        grid.Margin = new Thickness(10, 10, 10, -100);
                    }
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
                    Mediator.Notify("LoadSendComplaintPage", tasks[_index].TaskID);
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
                    ++_index;
                    LoadTask(_index < tasks.Count() ? tasks[_index] : null);
                }));
            }
        }


        private object[] ExecuteQuery(string query)
        {
            Task currTask = tasks[_index];

            object res = TrainSQL_Commands.IsCorrectQuery(query, currTask);

            if(res is object[] values
                && values.Length==2)
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


        private void LoadTask(Task task=null)
        {
            IsRightResult = false;
            UserQuery = "";
            ResultList = null;

            if (task!=null)
            {                
                _sheme = TrainSQL_Commands.GetDbSheme(task.dbID);

                TaskInfo = $"#{task.TaskID} " + task.TaskText;
                if (_sheme != null)
                {
                    DBImage = Helper.BytesToBitmapImage(_sheme.ShemeImg);
                    DBInfo = _sheme.Info;
                }
            }
            else
            {
                if (tasks.Count() > 1)
                {
                    TrainSQL_Commands.AddUserProgress(CurrentUser.Login, _rightAnswersCount, tasks.Count());
                        
                    MessageBox.Show($"Ваш результат: {_rightAnswersCount}/{tasks.Count()}");
                }
                Mediator.Notify("LoadUserMainPage", CurrentUser.Login);
            }

            

            //MessageBox.Show("Загрузка следующего задания");
        }

    }
}
