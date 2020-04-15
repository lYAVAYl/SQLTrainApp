using System.Drawing;
using System.Windows.Media.Imaging;

namespace SQLTrainApp.Model.Logic
{
    public static class CurrentUser
    {
        public static string Login { get; set; }
        public static string Email { get; set; }
        public static string Role { get; set; }

        public static BitmapImage Photo { get; set; }

        public static string ToString()
        {
            return $"{Login} \n{Email} \n{Role} \n";
        }
    }
}
