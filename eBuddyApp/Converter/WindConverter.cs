using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace eBuddyApp.Converter
{
    class WindConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double wind = (double)value;

            return String.Format("{0} KMh", wind);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
