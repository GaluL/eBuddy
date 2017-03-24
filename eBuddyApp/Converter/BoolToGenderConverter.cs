using System;
using Windows.UI.Xaml.Data;

namespace eBuddyApp.Converter
{
    public class BoolToGenderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool) value)
            {
                return "Male";
            }


            return "Female";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (((string) value).Equals("Male"))
            {
                return true;
            }

            return false;
        }
    }
}