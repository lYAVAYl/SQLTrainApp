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
    public class SignInPageViewModel : BaseViewModel, IPageViewModel
    {
        public string myLog { get; set; }
        public string myPass { get; set; }


        private ICommand _loadSignOnPage;
        public ICommand LoadSignOnPage
        {
            get
            {
                return _loadSignOnPage ?? (_loadSignOnPage = new RelayCommand(x =>
                {
                    Mediator.Notify("LoadSignOnPage", "");
                }));
            }
        }

        private ICommand _findUser;
        public ICommand FindUser
        {
            get
            {
                return _findUser ?? (_findUser = new RelayCommand(x =>
                {
                    TryToSignIn(new object[2] { myLog, myPass });
                }));
            }
        }



        private void TryToSignIn(object parameters)
        {
            // Выполняется поиск пользователя в БД

            var values = (object[])parameters;
            string login = values[0].ToString();
            string password = values[1].ToString();

            User user = TrainSQL_Commands.FindUser(login);
            if (user != null && user.Password == password)
            {
                CurrentUser.Login = user.Login;
                CurrentUser.Email = user.UserEmail;
                CurrentUser.Role = TrainSQL_Commands.GetUserRole(user);
                CurrentUser.Photo = BytesToBitmapImage(user.Photo);

                Mediator.Notify("LoadUserMainPage", "");
            }

        }


        private BitmapImage BytesToBitmapImage(byte[] bitmapBytes)
        {
            if (bitmapBytes == null) return null;

            using (var ms = new MemoryStream(bitmapBytes))
            {
                var bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = ms;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }





    }
}
