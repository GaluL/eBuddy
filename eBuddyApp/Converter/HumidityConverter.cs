using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace eBuddyApp.Converter
{
    class HumidityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double humidity = (double)value;

            return String.Format("{0}%", humidity);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
