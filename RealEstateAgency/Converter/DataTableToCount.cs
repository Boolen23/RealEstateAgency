using System;
using System.Data;
using System.Globalization;
using System.Windows.Data;

namespace RealEstateAgency.Converter
{
    public class DataTableToCount : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            return "Строк" + (value as DataTable).Rows.Count;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
