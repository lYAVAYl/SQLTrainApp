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
    class SignInPageViewModel : BaseViewModel, IPageViewModel
    {
        public string myLog { get; set; }
        public string myPass { get; set; }

        public SignInPageViewModel()
        {
            myLog = "";
            myPass = "";
        }

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

        private ICommand _loadRefreshPasswordPage;
        public ICommand LoadRefreshPasswordPage
        {
            get
            {
                return _loadRefreshPasswordPage ?? (_loadRefreshPasswordPage = new RelayCommand(x =>
                {
                    Mediator.Notify("LoadRefreshPasswordPage", "");
                }));
            }
        }

        private ICommand _signIn;
        public ICommand SignIn
        {
            get
            {
                return _signIn ?? (_signIn = new RelayCommand(x =>
                {
                    TryToSignIn(myLog, myPass);
                }));
            }
        }



        private void TryToSignIn(string login = "", string password="")
        {
            // Выполняется поиск пользователя в БД

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
