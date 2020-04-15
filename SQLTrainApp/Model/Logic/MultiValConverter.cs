using System;
using System.Globalization;
using System.Windows.Data;

namespace SQLTrainApp.Model.Logic
{
    public class MultiValConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {

                return values;
            }
            return new ArgumentOutOfRangeException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
