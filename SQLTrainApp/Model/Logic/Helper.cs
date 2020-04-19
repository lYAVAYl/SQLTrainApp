using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace SQLTrainApp.Model.Logic
{
    public static class Helper
    {

        public static bool ValidateInputLogin(char c)
        {
            return !char.IsWhiteSpace(c) && (char.IsDigit(c) ||
                    ((c >= 'a') && (c <= 'z')) || ((c >= 'A') && (c <= 'Z')));
        }
        public static bool ValidateInputPassword(char c)
        {
            return !char.IsWhiteSpace(c) && (char.IsDigit(c) || c == '_'
                    || ((c >= 'a') && (c <= 'z')) || ((c >= 'A') && (c <= 'Z')));
        }

        public static BitmapImage BytesToBitmapImage(byte[] bitmapBytes)
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

        

        public static string FindFile(string filter)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = filter;
            var result = dialog.ShowDialog();
            //System.Window.Forms.DialogResult.OK;
            if (result == DialogResult.OK)
                return dialog.FileName;
            else return null;

        }
    }
}
