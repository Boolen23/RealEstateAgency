using System;
using System.Globalization;
using System.Windows.Data;

namespace RealEstateAgency.Converter
{
    public class SliderValueToSearchSet : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double temp = (double)value;
            int result = (int)temp;

            if (parameter == null) return result;
            else if (int.Parse(parameter.ToString()) == result) return null;
            else return result;
        }
    }
}
