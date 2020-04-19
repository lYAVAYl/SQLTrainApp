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
        public string UserLogin
        {
            get => _login;
            set
            {
                if (value.Length != 0)
                {
                    if (Helper.ValidateInputLogin(value.Last()))
                        _login = value;
                }
                else _login = "";
            }
        }

        private string _password;
        public string UserPassword
        {
            get => _password;
            set => _password = value;
        }


        public SignInPageViewModel()
        {
            UserLogin = "";
            UserPassword = "";
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
                    TryToSignIn();
                }));
            }
        }

        public override string Validate(string columnName)
        {
            string error = null;
            switch (columnName)
            {
                case nameof(UserLogin):
                    if (string.IsNullOrEmpty(UserLogin))
                    {
                        error = "Логин не введён";
                    }
                    else if (UserLogin.Length < 5)
                    {
                        error = "Длина логина - не менее 5 символов";
                    }
                    else if (UserLogin.Length > 30)
                    {
                        error = "Длина логина - не более 30 символов";
                    }
                    break;
                case nameof(UserPassword):
                    if (string.IsNullOrEmpty(UserPassword))
                    {
                        error = "Пароль не введён";
                    }
                    else if (UserPassword.Length < 6)
                    {
                        error = "Длина пароля - не менее 6 символов";
                    }
                    else if (UserPassword.Length > 32)
                    {
                        error = "Длина пароля - не более 32 символов";
                    }
                    else if(UserPassword.Contains(" "))
                    {
                        error = "Пароль не может содержать пробелы";
                    }
                    else if (!Helper.ValidateInputPassword(UserPassword.Last()))
                    {
                        error = "Пароль может содержать только буквы латинского алфавита, цифры и знак _";
                    }
                    break;

            }

            if (ErrorCollection.ContainsKey(columnName))
                ErrorCollection[columnName] = error;
            else if (error != null) ErrorCollection.Add(columnName, error);

            IsEnableBtn = ErrorCollection[nameof(UserLogin)] == null
                          && ErrorCollection[nameof(UserPassword)] == null;

            OnPropertyChanged(nameof(ErrorCollection));


            return error;
        }


        private void TryToSignIn()
        {
            // Выполняется поиск пользователя в БД

            User user = TrainSQL_Commands.FindUser(_login);
            if (user != null && user.Password == _password)
            {
                CurrentUser.Login = user.Login;
                CurrentUser.Email = user.UserEmail;
                CurrentUser.Role = TrainSQL_Commands.GetUserRole(user);
                CurrentUser.Photo = Helper.BytesToBitmapImage(user.Photo);

                Mediator.Notify("LoadUserMainPage", "");
            }
            else Error = "Неверный логин или пароль";

        }








    }
}
