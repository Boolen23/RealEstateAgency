using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Data;

namespace RealEstateAgency.Converter
{
    public class RowsCountToEnabled : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int i = (value as List<DataRow>).Count;
            if (i == 0) return false;
            else return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
