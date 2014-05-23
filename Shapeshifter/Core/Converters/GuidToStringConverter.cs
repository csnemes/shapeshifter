using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shapeshifter.Core.Deserialization;

namespace Shapeshifter.Core.Converters
{
    internal class GuidToStringConverter
    {
        public object ConvertToPackformat(object value)
        {
            var guidVal = (Guid)value;
            return guidVal.ToString();
        }

        public object ConvertFromPackformat(Type targetType, object value)
        {
            return Guid.Parse((string) value);
        }

        public bool CanConvert(Type type)
        {
            return type == typeof (Guid);
        }
    }
}
