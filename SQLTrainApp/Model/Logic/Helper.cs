using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        // валидация логина (только буквы латинского алфавита или цифры, пробелы запрещены)
        public static bool ValidateInputLogin(char c)
        {
            return !char.IsWhiteSpace(c) && (char.IsDigit(c) ||
                    ((c >= 'a') && (c <= 'z')) || ((c >= 'A') && (c <= 'Z')));
        }

        // валидация пароля (только буквы латинского алфавита или цифры, пробелы запрещены)
        public static bool ValidateInputPassword(char c)
        {
            return !char.IsWhiteSpace(c) && (char.IsDigit(c) || c == '_'
                    || ((c >= 'a') && (c <= 'z')) || ((c >= 'A') && (c <= 'Z')));
        }

        /// <summary>
        /// Конвертация byte[] в тип BitmapImage
        /// </summary>
        /// <param name="bitmapBytes">массив байтов</param>
        /// <returns></returns>
        public static BitmapImage BytesToBitmapImage(byte[] bitmapBytes)
        {
            // если массив пуст, то вернуть стандартную картинку
            if (bitmapBytes == null) 
                return new BitmapImage(new Uri("pack://application:,,,/Resources/defaultPhoto.jpg"));

            // byte[] -> BitmapImage
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

        /// <summary>
        /// Конвертация BitmapImage в тип byte[]
        /// </summary>
        /// <param name="fileName">Путь к картинке</param>
        /// <returns></returns>
        public static byte[] ConvertImageToByteArray(string fileName)
        {
            Bitmap bitMap = new Bitmap(fileName);
            ImageFormat bmpFormat = bitMap.RawFormat;
            var imageToConvert = Image.FromFile(fileName);
            using (MemoryStream ms = new MemoryStream())
            {
                imageToConvert.Save(ms, bmpFormat);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Диалоговое окно с выбором файла 
        /// </summary>
        /// <param name="filter">Фильтры файлов</param>
        /// <returns></returns>
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
