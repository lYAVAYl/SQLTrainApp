using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using SQLTrainApp.Model.Logic;

namespace SQLTrainApp.Model.Logic
{
    public class MultiValConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values.Length == 2)
            {
                CurrentUser.Login = values[0].ToString();
                CurrentUser.Password = values[1].ToString();

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
