using RealEstateAgency.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RealEstateAgency.Converter
{
    public class PurchaseToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.Equals("Floor"))
            {
                if (value.Equals(Purchase.Flat)) return Visibility.Visible;
                else return Visibility.Collapsed;
            }
            if (parameter.Equals("Rooms"))
            {
                if (value.Equals(Purchase.Flat)) return Visibility.Visible;
                else return Visibility.Collapsed;
            }
            if (parameter.Equals("HouseFloor"))
            {
                if (value.Equals(Purchase.Home)) return Visibility.Visible;
                else return Visibility.Collapsed;
            }
            throw new Exception("something went wrong");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
