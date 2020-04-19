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
using System.Net.Mail;
using System.Net;

namespace SQLTrainApp.ViewModel
{
    class SignOnPageViewModel : ValidateBaseViewModel, IPageViewModel
    {
        private string _email;
        public string UserEmail 
        {
            get => _email;
            set
            {
                if (value.Length != 0)
                {
                    if (!char.IsWhiteSpace(value.Last()))
                        _email = value;
                }
                else _email = "";
            }
        }

        private string _login;
        public string UserLogin
        {
            get => _login;
            set
            {
                if (value.Length!=0)
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

        private BitmapImage _defaultPhoto = new BitmapImage(new Uri("pack://application:,,,/Resources/defaultPhoto.jpg"));
        public BitmapImage UserPhoto { get; set; } = new BitmapImage();

        public SignOnPageViewModel()
        {
            UserEmail = "";
            UserLogin = "";
            UserPass1 = "";
            UserPass2 = "";
            UserPhoto = _defaultPhoto;
        }

        //DONE
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
        // DONE
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

        private ICommand _tryRegister;
        public ICommand TryRegister
        {
            get
            {
                return _tryRegister ?? (_tryRegister = new RelayCommand(x =>
                {
                    RegisterNewUser();
                }));
            }
        }
        // DONE
        private ICommand _loadNewPhoto;
        public ICommand LoadNewPhoto
        {
            get
            {
                return _loadNewPhoto ?? (_loadNewPhoto = new RelayCommand(x =>
                {
                    GetPhoto();
                }));
            }
        }
        // DONE
        private ICommand _loadDefaultPhoto;
        public ICommand LoadDefaultPhoto
        {
            get
            {
                return _loadDefaultPhoto ?? (_loadDefaultPhoto = new RelayCommand(x =>
                {
                    UserPhoto = _defaultPhoto;
                }));
            }
        }


        private void GetPhoto()
        {
            string photoWay = Helper.FindFile("Файлы изображений (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png");
            if(photoWay!=null)
                UserPhoto = new BitmapImage(new Uri(photoWay, UriKind.Relative));
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="email"></param>
        /// <param name="login"></param>
        /// <param name="pass1"></param>
        /// <param name="pass2"></param>
        /// <param name="photo"></param>
        private void RegisterNewUser()
        {
            if (TrainSQL_Commands.IsEmailRegistered(UserEmail))
            {
                ErrorCollection[nameof(UserEmail)] = "Такой email уже зарегестрирован";
            }
            if (TrainSQL_Commands.IsUserExists(UserLogin))
            {
                ErrorCollection[nameof(UserLogin)] = "Логин занят";
            }

            OnPropertyChanged(nameof(ErrorCollection));

            if (ErrorCollection[nameof(UserEmail)] == null
                && ErrorCollection[nameof(UserLogin)] == null)
            {
                User user = new User()
                {
                    Login = UserLogin,
                    UserEmail = this.UserEmail,
                    Password = this.UserPass1,
                    RoleID = 1,

                    Photo = UserPhoto == _defaultPhoto ? new byte[0] : new byte[0]
                };

                EmailConfirm emailConfirm = new EmailConfirm();
                object sentCode = emailConfirm.SendConfirmCode(this.UserEmail);

                MessageBox.Show("На ваш email был отправлен код подтверждения регисрации");

                Mediator.Notify("LoadConfirmEmailPage", new object[] { user, sentCode });
            }
            else IsEnableBtn = false;


        }

        public override string Validate(string columnName)
        {
            string error = null;
            switch (columnName)
            {                
                case nameof(UserEmail):
                    if (string.IsNullOrEmpty(UserEmail))
                    {
                        error = "Email не введён";
                    }
                    else if(UserEmail.Split('@').Length-1 != 1)
                    {
                        error = "Email не соответствует стандарту";
                    }
                    break;
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
                    break;
                case nameof(UserPass2):
                    if (string.IsNullOrEmpty(UserPass1))
                    {
                        error = "Пароль не введён";
                    }
                    else if (UserPass2 != UserPass1)
                        error = "Пароли не совпадают";
                    break;
            }

            if (ErrorCollection.ContainsKey(columnName))
                ErrorCollection[columnName] = error;
            else if (error != null) ErrorCollection.Add(columnName, error);

            IsEnableBtn = ErrorCollection[nameof(UserEmail)] == null
                          && ErrorCollection[nameof(UserLogin)] == null
                          && ErrorCollection[nameof(UserPass1)] == null
                          && ErrorCollection[nameof(UserPass2)] == null;

            OnPropertyChanged(nameof(ErrorCollection));            

            return error;
        }
    }
}
