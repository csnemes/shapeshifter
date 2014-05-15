using System;

namespace Shapeshifter.Core.Converters
{
    internal class SurrogateConverterWrapper : IValueConverter
    {
        private readonly IPackformatSurrogateConverter _surrogateConverter;

        public SurrogateConverterWrapper(IPackformatSurrogateConverter surrogateConverter)
        {
            _surrogateConverter = surrogateConverter;
        }

        public object ConvertToPackformat(object value)
        {
            var result = _surrogateConverter.ConvertToSurrogate(value);
            if (result != null && !IsSurrogateTypeDataContractSerializable(result.GetType()))
            {
                throw Exceptions.InvalidSurrogate(result.GetType());
            }
            return result;
        }

        private bool IsSurrogateTypeDataContractSerializable(Type surrogateType)
        {
            //TODO performance! cache type inspectors statically?
            var typeInfo = new TypeInspector(surrogateType);
            return typeInfo.IsSerializable;
        }

        public object ConvertFromPackformat(ConversionHelpers conversionHelpers, Type targetType, object value)
        {
            return _surrogateConverter.ConvertFromSurrogate(targetType, value);
        }

        public bool CanConvert(Type type)
        {
            return _surrogateConverter.CanConvert(type);
        }
    }
}