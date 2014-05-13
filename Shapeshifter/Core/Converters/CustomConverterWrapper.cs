using System;

namespace Shapeshifter.Core.Converters
{
    internal class CustomConverterWrapper : IValueConverter
    {
        private readonly ICustomPackformatConverter _customConverter;

        public CustomConverterWrapper(ICustomPackformatConverter customConverter)
        {
            _customConverter = customConverter;
        }

        public object ConvertToPackformat(object value)
        {
            return _customConverter.ConvertToPackformat(value);
        }

        public object ConvertFromPackformat(ConversionHelpers conversionHelpers, Type targetType, object value)
        {
            return _customConverter.ConvertFromPackformat(targetType, value);
        }

        public bool CanConvert(Type type)
        {
            return _customConverter.CanConvert(type);
        }
    }
}