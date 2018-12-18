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
    public class PurchaseToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.Equals("Flat"))
            {
                if (value.Equals(Purchase.Flat)) return true;
                else return false;
            }
            else if (parameter.Equals("Home"))
            {
                if (value.Equals(Purchase.Home)) return true;
                else return false;
            }
            else if (parameter.Equals("Site"))
            {
                if (value.Equals(Purchase.Site)) return true;
                else return false;
            }
            else if (parameter.Equals("Commerce"))
            {
                if (value.Equals(Purchase.Commerce)) return true;
                else return false;
            }
            throw new Exception("something went wrong");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.Equals("Flat"))
                if ((bool)value) return Purchase.Flat;
                else return false;

            if (parameter.Equals("Home"))
                if ((bool)value) return Purchase.Home;
                else return false;

            if (parameter.Equals("Site"))
                if ((bool)value) return Purchase.Site;
                else return false;
            if (parameter.Equals("Commerce"))
                if ((bool)value) return Purchase.Commerce;
                else return false;


            throw new Exception("something went wrong");
        }
    }
}
