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
    class RefreshPasswordViewModel:ValidateBaseViewModel, IPageViewModel
    {
        private User user = new User();
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

        private string _pass1;
        public string UserPass1
        {
            get => _pass1;
            set => _pass1 = value;
        }
        private string _pass2;
        public string UserPass2
        {
            get => _pass2;
            set => _pass2 = value;

        }

        private string _rightCode = null;
        private string _code;
        public string Code
        {
            get => _code;
            set
            {
                if (value.Length != 0)
                {
                    if (value.Length < 6 && char.IsDigit(value.Last()))
                        _code = value;
                }
                else _code = "";
            }

        }

        public int ChancesNum { get; set; } = 5;

        public bool EnableLogin { get; set; } = true;
        public bool EnableSendCode { get; set; } = false;
        public bool EnableConfirm { get; set; } = false;
        public bool EnablePass { get; set; } = false;

        private ICommand _loadSignInPage;
        public ICommand LoadSignInPage
        {
            get
            {
                return _loadSignInPage ?? (_loadSignInPage = new RelayCommand(x =>
                {
                    Mediator.Notify("LoadSignInPage", "");
                }));
            }
        }

        private ICommand _refreshPassword;
        public ICommand RefreshPassword
        {
            get
            {
                return _refreshPassword ?? (_refreshPassword = new RelayCommand(x =>
                {
                    // TODO: Проверка паролей на совпадение
                    SetNewPassword(UserLogin, UserPass1);
                }));
            }
        }

        private ICommand _sendConfirmCode;
        public ICommand SendConfirmCode
        {
            get
            {
                return _sendConfirmCode ?? (_sendConfirmCode = new RelayCommand(x =>
                  {
                      SendCode();
                  }));
            }
        }

        private ICommand _confirm;
        public ICommand ConfirmCode
        {
            get
            {
                return _confirm ?? (_confirm = new RelayCommand(x =>
                {
                    if (Code == _rightCode)
                    {
                        EnableConfirm = false;
                        EnablePass = true;                        
                    }
                    else if (--ChancesNum == 0)
                    {
                        Mediator.Notify("LoadSignOnPage", "");
                    }

                }));
            }
        }
        private void SetNewPassword(string login, string newPass)
        {
            if(UserPass1 == user.Password)
            {
                ErrorCollection[nameof(UserPass1)] = "Новый пароль не может равняться старому";
                IsEnableBtn = false;
                OnPropertyChanged(nameof(ErrorCollection));
            }
            else
            {
                TrainSQL_Commands.ChangeUserProperty(user, nameof(user.Password), UserPass1);
                Mediator.Notify("LoadSignInPage", "");
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
                    else if (int.TryParse(UserLogin, out int n))
                    {
                        error = "Логин не может состоять из одних цифр";
                    }
                    break;
                case nameof(UserPass1):
                    if (string.IsNullOrEmpty(UserPass1))
                    {
                        error = "Пароль не введён";
                    }
                    else if (UserPass1.Length < 6)
                    {
                        error = "Длина пароля - не менее 6 символов";
                    }
                    else if (UserPass1.Length > 32)
                    {
                        error = "Длина пароля - не более 32 символов";
                    }
                    else if (UserPass1.Contains(" "))
                    {
                        error = "Пароль не может содержать пробелы";
                    }
                    else if (!Helper.ValidateInputPassword(UserPass1.Last()))
                    {
                        error = "Пароль может содержать только буквы латинского алфавита, цифры и знак _";
                    }
                    else
                    {
                        for (int i = 0; i < UserPass1.Length - 3; i++)
                        {
                            if (UserPass1[i] == UserPass1[i + 1]
                                && UserPass1[i] == UserPass1[i + 2]
                                && UserPass1[i] == UserPass1[i + 3])
                                error = "Более 3 подряд идущих символов запрещены";
                        }

                        ErrorCollection[nameof(UserPass2)] = UserPass2 != UserPass1 ? "Пароли не совпадают" : null;
                    }
                    break;
                case nameof(UserPass2):
                    if (string.IsNullOrEmpty(UserPass1))
                    {
                        error = "Пароль не введён";
                    }
                    else if (UserPass2 != UserPass1)
                    {
                        error = "Пароли не совпадают";
                    }
                    break;
            }

            if (ErrorCollection.ContainsKey(columnName))
                ErrorCollection[columnName] = error;
            else ErrorCollection.Add(columnName, error);

            EnableSendCode = ErrorCollection[nameof(UserLogin)] == null;

            IsEnableBtn = Code!=null && ErrorCollection[nameof(UserPass1)] == null
                          && ErrorCollection[nameof(UserPass2)] == null;

            OnPropertyChanged(nameof(ErrorCollection));

            return error;
        }

        private void SendCode()
        {
            if (TrainSQL_Commands.IsUserExists(UserLogin))
            {
                user = TrainSQL_Commands.FindUser(UserLogin);
                if(user!= null)
                {
                    EmailConfirm emailConfirm = new EmailConfirm();
                    _rightCode = emailConfirm.SendConfirmCode(user.UserEmail).ToString();


                    MessageBox.Show("На ваш email был отправлен код подтверждения регисрации");
                    EnableLogin = false;
                    EnableSendCode = false;

                    EnableConfirm = true;
                }
            }
            else
            {
                ErrorCollection[nameof(UserLogin)] = "Логин не найден";
                OnPropertyChanged(nameof(ErrorCollection));

            }

        }

    }
}
