using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RealEstateAgency.Converter
{
    public class DataTableToIdList : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if ((string)parameter == "TypeTransaction")
                {
                    var list = (from row in (value as DataTable).AsEnumerable()
                                select row.Field<string>((string)parameter)).ToList().Distinct();
                    return list;
                }
                else
                {
                    var list = (from row in (value as DataTable).AsEnumerable()
                                select row.Field<int>((string)parameter)).Distinct().ToList();
                    list.Sort();
                    return list;
                }
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
