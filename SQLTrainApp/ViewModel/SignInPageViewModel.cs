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
using System.ComponentModel;

namespace SQLTrainApp.ViewModel
{
    class SignInPageViewModel : ValidateBaseViewModel, IPageViewModel
    {
        private string _login;
        public string myLog
        {
            get => _login;
            set
            {
                if (value.Length != 0)
                {
                    char last = value.Last();
                    if (!char.IsWhiteSpace(last)
                        && (char.IsDigit(last) || char.IsLetter(last))) _login = value;
                }
                else _login = "";
            }
        }

        private string _password;
        public string myPass
        {
            get => _password;
            set
            {
                if (value.Length != 0)
                {
                    char last = value.Last();
                    if (!char.IsWhiteSpace(last)
                        && (char.IsDigit(last) || char.IsLetter(last))) _password = value;
                }
                else _password = "";
            }
        }

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

        public Dictionary<string, string> ErrorCollection { get; set; } = new Dictionary<string, string>();
        public override string Validate(string columnName)
        {
            string error = null;

            switch (columnName)
            {
                case nameof(myLog):
                    if (string.IsNullOrEmpty(myLog))
                    {
                        error = "Логин не введён";
                    }
                    else if (myLog.Length < 5)
                    {
                        error = "Длина логина - не менее 5 символов";
                    }
                    else if (myLog.Length > 30)
                    {
                        error = "Длина логина - не более 30 символов";
                    }
                    break;
                case nameof(myPass):
                    if (string.IsNullOrEmpty(myPass))
                    {
                        error = "Пароль не введён";
                    }
                    else if (myPass.Length < 6)
                    {
                        error = "Длина пароля - не менее 6 символов";
                    }
                    else if (myPass.Length > 32)
                    {
                        error = "Длина пароля - не более 32 символов";
                    }
                    break;

            }


            if (ErrorCollection.ContainsKey(columnName))
                ErrorCollection[columnName] = error;
            else if (error != null) ErrorCollection.Add(columnName, error);


            OnPropertyChanged(nameof(ErrorCollection));


            return error;
        }


        //public Dictionary<string, string> ErrorCollection { get; set;}
        //public string this[string columnName]
        //{
        //    get
        //    {
        //        string result = null;

        //        switch (columnName)
        //        {
        //            case nameof(myLog):
        //                if (string.IsNullOrEmpty(myLog))
        //                {
        //                    result = "Логин не может быть пустым";
        //                }
        //                else if (myLog.Length<5)
        //                {
        //                    result = "Минимальная длина логина - 5 символов";
        //                }
        //                else if (myLog.Length > 30)
        //                {
        //                    result = "Максимальная длина логина - 30 символов";
        //                }

        //                break;
        //            case nameof(myPass):
        //                if (string.IsNullOrEmpty(myPass))
        //                {
        //                    result = "Пароль не может быть пустым";
        //                }
        //                break;

        //        }

        //        if (ErrorCollection.ContainsKey(columnName))
        //            ErrorCollection[columnName] = result;
        //        else if (result != null) ErrorCollection.Add(columnName, result);

        //        //OnPropertyChanged(nameof(ErrorCollection));

        //        return result;

        //    }
        //}



        private void TryToSignIn(string login = "", string password = "")
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
