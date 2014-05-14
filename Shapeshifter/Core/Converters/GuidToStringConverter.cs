using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shapeshifter.Core.Converters
{
    internal class GuidToStringConverter : IValueConverter
    {
        public object ConvertToPackformat(object value)
        {
            var guidVal = (Guid)value;
            return guidVal.ToString();
        }

        public object ConvertFromPackformat(ConversionHelpers conversionHelpers, Type targetType, object value)
        {
            return Guid.Parse((string) value);
        }

        public bool CanConvert(Type type)
        {
            return type == typeof (Guid);
        }
    }
}
