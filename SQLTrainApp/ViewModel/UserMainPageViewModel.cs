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
using System.Collections.ObjectModel;

namespace SQLTrainApp.ViewModel
{
    public class UserMainPageViewModel : BaseViewModel, IPageViewModel
    {
        public BitmapImage UserPhoto { get; set; }
        public string UserLogin { get; set; }
        public string UserEmail { get; set; }
        public string UserRole { get; set; }

        // коллекция данных, для вывода на графике (линия прогресса, дата прохождения теста)
        public ObservableCollection<BaseCoordinate> Items { get; set; }
        public int CanWidth { get; set; } // размер графика

        private User _user;

        public UserMainPageViewModel(string login = "")
        {
            if (login != "")
            {
                // поиск пользователя в БД
                _user = TrainSQL_Commands.FindUser(login);

                // получение данных пользователя
                UserPhoto = Helper.BytesToBitmapImage(_user.Photo);
                UserLogin = _user.Login;
                UserEmail = _user.UserEmail;
                UserRole = TrainSQL_Commands.GetUserRole(_user);

                // прогресс пользователя
                List<Progress> progress = TrainSQL_Commands.GetUserProgress(UserLogin);

                // проверка, прошёл ли пользователь хотя бы 1 тест
                if (progress!=null && progress.Count() > 1)
                {
                    Items = new ObservableCollection<BaseCoordinate>();

                    // построение графика прогресса пользователя
                    for (int i = 0; i < progress.Count - 1; i++)
                    {
                        // создание новой линии
                        LineVM line = new LineVM()
                        {
                            X1 = i * 70,
                            Y1 = 210 - progress[i].RightAnswersQuantity * 32,
                            X2 = (i + 1) * 70,
                            Y2 = 210 - progress[i + 1].RightAnswersQuantity * 32
                        };

                        // выбор цвета линии на графике
                        if (line.Y1 > line.Y2)
                            line.StrokeColor = Brushes.Lime; // зелёный - результаты улучшились
                        else if (line.Y1 < line.Y2)
                            line.StrokeColor = Brushes.Red; // красный - результаты ухудшились
                        else
                            line.StrokeColor = Brushes.Gold; // жёлтый - - результаты не изменились

                        // добавление линии в коллекцию
                        Items.Add(line);

                        // добавление даты прохождения теста
                        Items.Add(new TextBlockVM()
                        {
                            Left = 70 * i + 40,
                            Top = 210,
                            TextOut = progress[i + 1].TestDate.ToString()
                        });
                    }

                    OnPropertyChanged(nameof(Items));

                    CanWidth = 75 * progress.Count();
                }
            }
        }

    }
}
