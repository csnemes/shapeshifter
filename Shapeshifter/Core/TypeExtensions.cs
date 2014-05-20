using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shapeshifter.Core
{
    public static class TypeExtensions
    {
        public static IEnumerable<FieldInfo> GetFieldsRecursive(this Type type, BindingFlags flags)
        {
            var flagsWithDeclaredOnly = flags | BindingFlags.DeclaredOnly;

            return type == null 
                ? Enumerable.Empty<FieldInfo>()
                : type.GetFields(flagsWithDeclaredOnly).Concat(type.BaseType.GetFieldsRecursive(flagsWithDeclaredOnly));
        }

        public static IEnumerable<PropertyInfo> GetPropertiesRecursive(this Type type, BindingFlags flags)
        {
            var flagsWithDeclaredOnly = flags | BindingFlags.DeclaredOnly;

            return type == null
                ? Enumerable.Empty<PropertyInfo>()
                : type.GetProperties(flagsWithDeclaredOnly).Concat(type.BaseType.GetPropertiesRecursive(flagsWithDeclaredOnly));
        }

        public static bool IsEnumerable(this Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }
    }
}
