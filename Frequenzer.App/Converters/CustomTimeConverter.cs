using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Frequenzer.App.Converters
{
    public class CustomTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double doubleValue = (double)value;
            int seconds = (int)doubleValue;

            if (seconds <= 0)
            {
                return "0";
            }
            else
            {
                return string.Format("{0}:{1:00}", seconds / 60, seconds % 60);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
