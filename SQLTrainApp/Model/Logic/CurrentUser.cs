using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace SQLTrainApp.Model.Logic
{
    public static class CurrentUser
    {
        public static string Email { get; set; } = "";
        public static string Login { get; set; } = "";
        public static string Password { get; set; } = "";
        public static string Role { get; set; } = "";

        public static BitmapImage Photo { get; set; } = null;

        public static void RemoveData()
        {
            Login = ""; Email = ""; Role = ""; Photo = null;
        }

        public static string ToString() 
        {
            return $"{Login} \n{Email} \n{Role} \n";
        }

       
    }
}
