using System;
using System.Globalization;
using System.Windows.Data;

namespace RealEstateAgency.Converter
{
   public class FunctionToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string == false) return false;
            if ((string)value == "Admin") return true;
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value) return "Admin";
            else return "Client";
        }
    }
}
