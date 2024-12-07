using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reader.Services
{
    public class ExpandRotationConverter : IValueConverter
    {
        private readonly Dictionary<bool, double> _cache = new()
    {
        { false, 0 },
        { true, 180 }
    };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return _cache[boolValue];
            }
            return 0; // Default fallback value
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
