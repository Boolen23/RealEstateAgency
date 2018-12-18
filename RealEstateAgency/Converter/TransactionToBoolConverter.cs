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
    public class TransactionToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(parameter.ToString()=="Buy")
            {
                if (value.Equals(Transaction.Sale)) return true;
                else return false;
            }
            else if(parameter.ToString() == "Rent")
            {
                if (value.Equals(Transaction.Rent)) return true;
                else return false;
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.ToString() == "Rent")
            {
                if ((bool)value) return Transaction.Rent;
                else return Transaction.Sale;
            }
            if (parameter.ToString() == "Buy")
            {
                if ((bool)value) return Transaction.Sale;
                else return Transaction.Rent;
            }
            return Transaction.Rent;


        }
    }
}
