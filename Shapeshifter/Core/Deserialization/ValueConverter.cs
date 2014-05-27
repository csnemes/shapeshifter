using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Shapeshifter.Core.Deserialization
{
    /// <summary>
    ///     Class containing helper functions for conversion between type of JSON returned data and the target type
    /// </summary>
    internal static class ValueConverter
    {
        public static T ConvertValueToTargetType<T>(object value)
        {
            return (T) ConvertValueToTargetType(typeof (T), value);
        }

        //TODO refactor, split into methods of manageable size
        public static object ConvertValueToTargetType(Type targetType, object value)
        {
            var packedForm = value as ObjectInPackedForm;
            if (packedForm != null)
            {
                value = packedForm.Deserialize();
            }

            //TODO skip native types which can not be subject of conversion

            if (value == null) return null;

            //handle Nullable<>
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof (Nullable<>))
            {
                Type targetNullableType = Nullable.GetUnderlyingType(targetType);
                object valueToWrapIntoNullable = ConvertValueToTargetType(targetNullableType, value);
                return valueToWrapIntoNullable;
            }

            //handle KeyValuePair<,>
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof (KeyValuePair<,>))
            {
                var valArray = value as IList;
                if (valArray == null)
                {
                    throw Exceptions.InvalidInputValueForConverter(value);
                }

                //TODO replace with generate code (speed opt)
                var constructor = targetType.GetConstructors()[0];
                var keyType = targetType.GetGenericArguments()[0];
                var valueType = targetType.GetGenericArguments()[1];
                var result = constructor.Invoke(new[]
                {
                    ConvertValueToTargetType(keyType, valArray[0]),
                    ConvertValueToTargetType(valueType, valArray[1])
                });
                return result;
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

            //handle generic IEnumerables
            if (targetType.IsEnumerable())
            {
                var source = value as List<object>;
                if (source != null)
                {
                    Type arrayElementType = targetType.GetGenericArguments().First();
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
            return ImplicitConversionHelper.ConvertValue(targetType, value);
        }

    }
}