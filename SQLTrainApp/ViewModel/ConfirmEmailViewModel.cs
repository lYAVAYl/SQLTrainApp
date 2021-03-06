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
using System.Drawing;

namespace SQLTrainApp.ViewModel
{
    class ConfirmEmailViewModel:BaseViewModel, IPageViewModel
    {
        public string UserLogin { get; set; }
        public string UserEmail { get; set; }
        
        private string _rightCode = null;
        private string _code;
        public string Code
        {
            get => _code;
            set
            {
                if (value.Length != 0 )
                {
                    if (value.Length < 6 && char.IsDigit(value.Last()))
                        _code = value;
                }
                else _code = "";
            }

        }

        public int ChancesNum { get; set; } = 5;

        private User user = new User();

        public ConfirmEmailViewModel(object obj = null)
        {
            if (obj != null)
            {
                var values = obj as object[];
                if (values != null && values.Length == 2)
                {
                    user = values[0] as User;
                    if (user != null)
                    {
                        UserLogin = user.Login;
                        UserEmail = user.UserEmail;
                    }
                    _rightCode = values[1].ToString();
                }
            }
                
        }

        private ICommand _loadSignOnPage;
        public ICommand LoadSignOnPage
        {
            get
            {
                return _loadSignOnPage ?? (_loadSignOnPage = new RelayCommand(x =>
                {
                    Mediator.Inform("LoadSignOnPage", "");
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
                    if(Code == _rightCode)
                    {
                        CurrentUser.Login = user.Login;
                        CurrentUser.Email = user.UserEmail;
                        CurrentUser.Role = TrainSQL_Commands.GetUserRole(user);
                        
                        CurrentUser.Photo = user.Photo == null ? new BitmapImage(new Uri("pack://application:,,,/Resources/defaultPhoto.jpg"))
                                                               : Helper.BytesToBitmapImage(user.Photo);

                        object ret = TrainSQL_Commands.AddUser(user);
                        if(ret == null)
                            Mediator.Inform("LoadUserMainPage", UserLogin);
                        else
                        {
                            MessageBox.Show(ret.ToString());
                            Mediator.Inform("LoadSignOnPage", CurrentUser.Login);
                        }
                    }
                    else if(--ChancesNum==0)
                    {
                        Mediator.Inform("LoadSignOnPage", "");
                    }

                }));
            }
        }
    }
}
