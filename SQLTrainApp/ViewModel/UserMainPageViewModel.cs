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

        public ObservableCollection<BaseCoordinate> Items { get; set; }
        public int CanWidth { get; set; }

        public UserMainPageViewModel()
        {
            UserPhoto = CurrentUser.Photo;
            UserLogin = CurrentUser.Login;
            UserEmail = CurrentUser.Email;
            UserRole = CurrentUser.Role;

            List<Progress> progress = TrainSQL_Commands.GetUserProgress(UserLogin);

            if (progress != null)
            {
                if (progress.Count() != 0)
                {
                    Items = new ObservableCollection<BaseCoordinate>();   
                    //int i = 0;
                    LineVM line;
                    if (progress.Count <= 14)
                    {
                        line = new LineVM()
                        {
                            X1 = 0,
                            Y1 = 210,
                            X2 = 70,
                            Y2 = 210 - progress[0].RightAnswersQuantity * 32
                        };
                    }
                    else
                    {
                        line = new LineVM()
                        {
                            X1 = 0,
                            Y1 = 210 - progress[0].RightAnswersQuantity * 32,
                            X2 = 70,
                            Y2 = 210 - progress[1].RightAnswersQuantity * 32
                        };
                    }
                    if (line.Y1 > line.Y2)
                        line.StrokeColor = Brushes.Lime;
                    else if (line.Y1 < line.Y2)
                        line.StrokeColor = Brushes.Red;
                    else
                        line.StrokeColor = Brushes.Blue;

                    Items.Add(line);

                    Items.Add(new TextBlockVM()
                    {
                        Left = 70 * 0 + 40,
                        Top = 210,
                        TextOut = progress[1].TestDate.ToString()
                    });

                    for (int i = 1; i < progress.Count-1; i++)
                    {
                        line = new LineVM()
                        {
                            X1 = i*70,
                            Y1 = 210 - progress[i].RightAnswersQuantity * 32,
                            X2 = (i+1)*70,
                            Y2 = 210 - progress[i+1].RightAnswersQuantity * 32
                        };

                        if (line.Y1 > line.Y2)
                            line.StrokeColor = Brushes.Lime;
                        else if (line.Y1 < line.Y2)
                            line.StrokeColor = Brushes.Red;
                        else
                            line.StrokeColor = Brushes.Blue;

                        Items.Add(line);

                        Items.Add(new TextBlockVM()
                        {
                            Left = 70 * i + 40, 
                            Top=210,
                            TextOut = progress[i+1].TestDate.ToString()
                        });
                    }                   

                }
                else
                {
                    MessageBox.Show("Элементов нет");
                }
                CanWidth =75 * progress.Count();
                
            }
        }

    }
}
