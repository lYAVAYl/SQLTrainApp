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
using System.Net.Mail;
using System.Net;

namespace SQLTrainApp.ViewModel
{
    class SignOnPageViewModel : BaseViewModel, IPageViewModel
    {
        public string UserEmail { get; set; }
        public string UserLogin { get; set; }
        public string UserPass1 { get; set; }
        public string UserPass2 { get; set; }

        public BitmapImage UserPhoto { get; set; }

        

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
                    RegisterNewUser(UserEmail, UserLogin, UserPass1, UserPass2, UserPhoto);
                }));
            }
        }

        private ICommand _loadUserPhoto;
        public ICommand LoadUserPhoto
        {
            get
            {
                return _loadUserPhoto ?? (_loadUserPhoto = new RelayCommand(x =>
                {
                    GetPhoto();
                }));
            }
        }
        private void GetPhoto()
        {

        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="email"></param>
        /// <param name="login"></param>
        /// <param name="pass1"></param>
        /// <param name="pass2"></param>
        /// <param name="photo"></param>
        private void RegisterNewUser(string email="", string login="", string pass1="", string pass2="", BitmapImage photo=null)
        {
            //if (email != "")
            //{
            //    EmailConfirm emailConfirm = new EmailConfirm();
            //    object res = emailConfirm.SendConfirmCode(email);
            //    if (res == null)
            //    {
            //        MessageBox.Show("Письмо успешно отправлено!");
            //    }
            //    else
            //    {
            //        MessageBox.Show(res.ToString());
            //    }
            //}

            Mediator.Notify("LoadConfirmEmailPage", "");
        }

        

    }
}
