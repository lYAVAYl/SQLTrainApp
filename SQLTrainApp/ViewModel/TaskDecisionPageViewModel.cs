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
        public string TaskInfo { get; set; } // Текст задания
        public string DBInfo { get; set; } // Информация о БД
        public BitmapImage DBImage { get; set; } // Схема БД
        public bool IsRightResult { get; set; } = false; // IsEnable для кнопки Дальше
        public bool EnableSkip { get; set; } = true; // IsEnable для кнопки Пропустить
        public string UserQuery { get; set; } // Запрос пользователя
        // public List<User> ResultList { get; set; }
        public DataSet ResultList { get; set; } // Вывод в DataSet

        private int _rightAnswersCount = 0;
        private List<Task> tasks = new List<Task>();
        private int _index = 0;
        private Sheme _sheme;
        
        public TaskDecisionPageViewModel(int count = 5)
        {
            tasks = TrainSQL_Commands.GetTestList(count);
            LoadTask(_index);
        }

        public TaskDecisionPageViewModel(Task task)
        {
            tasks.Add(task);
            LoadTask(_index);

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
                    LoadTask(++_index);
                }));
            }
        }


        private void ExecuteQuery(string query)
        {


            
            MessageBox.Show("Вывод результата запроса...");
        }

        private void LoadTask(int index=0)
        {
            if (index < tasks.Count())
            {
                if (IsRightResult)
                {
                    _rightAnswersCount++;
                    IsRightResult = false;
                }
                _sheme = TrainSQL_Commands.GetDbSheme(tasks[index].dbID);

                TaskInfo = $"#{tasks[_index].TaskID} " + tasks[index].TaskText;
                if (_sheme != null)
                {
                    DBImage = Helper.BytesToBitmapImage(_sheme.ShemeImg);
                    DBInfo = _sheme.Info;
                }
                

            }
            else
            {
                EnableSkip = false;
                if (tasks.Count() > 1)
                {
                    // Добавление очков в прогресс юзера

                    MessageBox.Show($"Ваш результат: {_rightAnswersCount}/{tasks.Count()}");
                }
                Mediator.Notify("LoadUserMainPage", "");
            }

            

            //MessageBox.Show("Загрузка следующего задания");
        }

    }
}
