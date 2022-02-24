using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Yaio.Converters
{


    [ValueConversion(typeof(int), typeof(SolidColorBrush))]
    class IntToNoteColor : IValueConverter
    {

        public static readonly IValueConverter Instance = new IntToNoteColor();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int foo = (int)value;
            switch (foo)
            {
                case 1: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEFBB00"));
                case 2: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5C7EC00"));
                case 3: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C7D6EC00"));
                case 4: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5ECC700"));
                case 5: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ECC9C900"));
                default: return new SolidColorBrush(Colors.White);
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //no need for convertion 
            return 1;
        }
    }
}
