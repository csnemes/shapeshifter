using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace Shapeshifter.Core
{
    /// <summary>
    ///     Class containing helper functions for conversion between type of JSON returned data and the target type
    /// </summary>
    internal class ConversionHelpers
    {
        private readonly ConvertersCollection _converters;

        public ConversionHelpers(ConvertersCollection converters)
        {
            _converters = converters;
        }

        public T ConvertValueToTargetType<T>(object value)
        {
            return (T) ConvertValueToTargetType(typeof (T), value);
        }

        //TODO refactor, split into methods of manageable size
        public object ConvertValueToTargetType(Type targetType, object value)
        {
            if (value == null) return null;

            var packedForm = value as ObjectInPackedForm;
            if (packedForm != null)
            {
                value = packedForm.Deserialize();
            }

            //look for custom converters
            IValueConverter converter = ResolveConverter(targetType);
            if (converter != null)
            {
                return converter.ConvertFromPackformat(this, targetType, value);
            }

            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof (Nullable<>))
            {
                Type targetNullableType = Nullable.GetUnderlyingType(targetType);
                object valueToWrapIntoNullable = ConvertValueToTargetType(targetNullableType, value);
                return valueToWrapIntoNullable;
            }

            //handle arrays
            if (targetType.IsArray)
            {
                var source = value as List<object>;
                if (source != null)
                {
                    Type arrayElementType = targetType.GetElementType();
                    Array resultArray = Array.CreateInstance(arrayElementType, source.Count);
                    for (int idx = 0; idx < source.Count; idx++)
                    {
                        resultArray.SetValue(ConvertValueToTargetType(arrayElementType, source[idx]), idx);
                    }
                    return resultArray;
                }
            }

            //handle generic list  //TODO a more efficient way, also refactor such items to internal converters
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof (List<>))
            {
                var source = value as List<object>;
                if (source != null)
                {
                    Type listItemType = targetType.GetGenericArguments()[0];
                    ConstructorInfo defaultConstructor = targetType.GetConstructor(new Type[0]);
                    var result = defaultConstructor.Invoke(new object[0]) as IList;
                    foreach (object item in source)
                    {
                        result.Add(ConvertValueToTargetType(listItemType, item));
                    }
                    return result;
                }
            }

            //handle generic collection  //TODO a more efficient way, also refactor such items to internal converters
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof (Collection<>))
            {
                var source = value as List<object>;
                if (source != null)
                {
                    Type listItemType = targetType.GetGenericArguments()[0];
                    ConstructorInfo defaultConstructor = targetType.GetConstructor(new Type[0]);
                    var result = defaultConstructor.Invoke(new object[0]) as IList;
                    foreach (object item in source)
                    {
                        result.Add(ConvertValueToTargetType(listItemType, item));
                    }
                    return result;
                }
            }

            //handle all items implementing ICollection<T> , this will handle the Dictionary as well
            if (targetType.IsGenericType)
            {
                Type iColInterface =
                    targetType.GetInterfaces()
                        .FirstOrDefault(
                            interfaceDef =>
                                interfaceDef.IsGenericType &&
                                interfaceDef.GetGenericTypeDefinition() == typeof (ICollection<>));
                if (iColInterface != null)
                {
                    //check if we have a default constructor
                    ConstructorInfo defaultConstructor = targetType.GetConstructor(new Type[0]);
                    if (defaultConstructor != null)
                    {
                        object result = defaultConstructor.Invoke(new object[0]);

                        //fill
                        var source = value as List<object>;
                        if (source != null)
                        {
                            MethodInfo addMethodInfo = iColInterface.GetMethod("Add");
                            Type collectionItemType = iColInterface.GetGenericArguments()[0];
                            foreach (object item in source)
                            {
                                addMethodInfo.Invoke(result, new[] {ConvertValueToTargetType(collectionItemType, item)});
                            }
                            return result;
                        }
                    }
                }
            }

            //fix well known type conversions
            if (value is long)
            {
                if (targetType == typeof (int)) //JSON stores int as long
                {
                    return Convert.ToInt32(value);
                }
                if (targetType == typeof (short))
                {
                    return Convert.ToInt16(value);
                }
                if (targetType == typeof (uint))
                {
                    return Convert.ToUInt32(value);
                }
                if (targetType == typeof (ushort))
                {
                    return Convert.ToUInt16(value);
                }
                if (targetType == typeof (byte))
                {
                    return Convert.ToByte(value);
                }
                if (targetType == typeof (sbyte))
                {
                    return Convert.ToSByte(value);
                }
            }
            else if (value is double)
            {
                if (targetType == typeof (float))
                {
                    return Convert.ToSingle(value);
                }
                if (targetType == typeof (decimal))
                {
                    return Convert.ToDecimal(value);
                }
            }
            else if (value is BigInteger)
            {
                if (targetType == typeof (ulong))
                {
                    return (ulong) (BigInteger) value;
                }
            }
            else if (value is string)
            {
                if (targetType == typeof (char))
                {
                    return Convert.ToChar(value);
                }
            }
            else if (value is DateTime)
            {
                if (targetType == typeof (DateTimeOffset))
                {
                    return new DateTimeOffset((DateTime) value);
                }
            }

            return value; //no conversion
        }


        public object ConvertValueToPackformatType(object value)
        {
            //look for custom converters
            IValueConverter converter = ResolveConverter(value.GetType());
            if (converter != null)
            {
                return converter.ConvertToPackformat(value);
            }

            return value; //no conversion
        }


        private IValueConverter ResolveConverter(Type type)
        {
            //TODO caching mechanism for already resolved type-converter pairs
            return _converters.ResolveConverter(type);
        }
    }
}