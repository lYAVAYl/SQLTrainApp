﻿using SQLTrainApp.Model.Commands;
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
        public BitmapImage UserPhoto { get; set; } = new BitmapImage();


        private BitmapImage _defaultPhoto = new BitmapImage(new Uri("pack://application:,,,/Resources/defaultPhoto.jpg"));
        private byte[] _userPhoto = null;
        

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
                    Mediator.Inform("LoadSignInPage", "");
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
                    Mediator.Inform("LoadRefreshPasswordPage", "");
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
            if (photoWay != null)
            {
                _userPhoto = Helper.ConvertImageToByteArray(photoWay);
                UserPhoto = Helper.BytesToBitmapImage(_userPhoto);
            }
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
                    Login = this.UserLogin,
                    UserEmail = this.UserEmail,
                    Password = this.UserPass1,
                    RoleID = 1,

                    Photo = _userPhoto
                };

                EmailConfirm emailConfirm = new EmailConfirm();
                object sentCode = emailConfirm.SendConfirmCode(this.UserEmail);

                MessageBox.Show("На ваш email был отправлен код подтверждения регистрации");

                Mediator.Inform("LoadConfirmEmailPage", new object[] { user, sentCode });
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
                    else if (int.TryParse(UserLogin, out int n))
                    {
                        error = "Логин не может состоять из одних цифр";
                    }
                    else
                    {
                        int i = 0;
                        for (; i < UserLogin.Length; i++)
                        {
                            if (!char.IsDigit(UserLogin[i]) && !char.IsLetter(UserLogin[i])) break;
                        }
                        
                        if (i < UserLogin.Length) 
                            error = "Разрешены только цифры и буквы латинского алфавита";
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
                        for (int i = 0; i < UserPass1.Length-3; i++)
                        {
                            if (UserPass1[i] == UserPass1[i + 1]
                                && UserPass1[i] == UserPass1[i + 2]
                                && UserPass1[i] == UserPass1[i + 3])
                                error = "Более 3 подряд идущих символов запрещены";
                        }

                        ErrorCollection[nameof(UserPass2)] = UserPass2 != UserPass1 ? "Пароли не совпадают":null;                    

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

            IsEnableBtn = ErrorCollection[nameof(UserEmail)] == null
                          && ErrorCollection[nameof(UserLogin)] == null
                          && ErrorCollection[nameof(UserPass1)] == null
                          && ErrorCollection[nameof(UserPass2)] == null;

            OnPropertyChanged(nameof(ErrorCollection));            

            return error;
        }
    }
}
