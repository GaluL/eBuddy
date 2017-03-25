using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;


namespace eBuddyApp.Converter
{
    class BoolToRunColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool bValue = (bool) value;

            return bValue ? new SolidColorBrush(Colors.DarkGreen) : new SolidColorBrush(Colors.OrangeRed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
