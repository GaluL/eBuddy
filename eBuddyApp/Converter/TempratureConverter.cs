using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace eBuddyApp.Converter
{
    class TempratureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double temp = (double)value;

            return String.Format("{0}°C", temp);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
