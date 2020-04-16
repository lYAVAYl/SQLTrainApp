using SQLTrainApp.Model.Logic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SQLTrainApp.ViewModel
{
    public class UserMainPageViewModel : BaseViewModel, IPageViewModel
    {
        public BitmapImage UserPhoto { get { return CurrentUser.Photo; } } 
        public string UserLogin { get { return CurrentUser.Login;} }
        public string UserEmail { get { return CurrentUser.Email;} } 
        public string UserRole { get { return CurrentUser.Role;} }


    }
}
