using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Yaio.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    class Negate : IValueConverter
    {

        public static readonly IValueConverter Instance = new Negate();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool foo = (bool)value;
            return !foo;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool foo = (bool)value;
            return !foo;
        }
    }
}
