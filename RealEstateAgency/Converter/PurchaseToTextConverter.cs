using RealEstateAgency.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RealEstateAgency.Converter
{
    public class PurchaseToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Equals(Purchase.Flat)) return "ApartamentTransaction";
            else if (value.Equals(Purchase.Home)) return "HomeTransaction";
            else if (value.Equals(Purchase.Site)) return "SiteTransaction";
            else if (value.Equals(Purchase.Commerce)) return "CommerceTransaction";
            throw new Exception("something went wrong");
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
