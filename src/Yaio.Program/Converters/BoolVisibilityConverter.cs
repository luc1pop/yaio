using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Yaio.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    class BoolVisibilityConverter : IValueConverter
    {

        public static readonly IValueConverter Instance = new BoolVisibilityConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool foo = (bool)value;
            if(foo)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility foo = (Visibility)value;
            if (foo == Visibility.Collapsed)
                return true;
            return false;
        }
    }
}
