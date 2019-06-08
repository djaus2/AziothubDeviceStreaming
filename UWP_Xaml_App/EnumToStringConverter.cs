using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UWPXamlApp
{
    public class EnumToStringConverter : IValueConverter
    {
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    return "";
        //}

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    throw new NotImplementedException();
        //}

        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Microsoft.Azure.Devices.Client.TransportType)
            {
                Microsoft.Azure.Devices.Client.TransportType trans = (Microsoft.Azure.Devices.Client.TransportType)value;
                return trans.ToString();
            }
            else
                return "unknown";
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
