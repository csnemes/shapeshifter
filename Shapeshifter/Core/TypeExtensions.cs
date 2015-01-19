using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Shapeshifter.Core
{
    internal static class TypeExtensions
    {
        public static bool HasAttributeOfType<TAttribute>(this Type type)
        {
            return type.GetCustomAttributes(typeof (TAttribute), false).Any();
        }

        public static bool HasAttributeOfType<TAttribute>(this MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes(typeof(TAttribute), false).Any();
        }

        public static FieldInfo GetFieldRecursive(this Type type, string fieldName, BindingFlags flags)
        {
            var flagsWithDeclaredOnly = flags | BindingFlags.DeclaredOnly;

            return type == null
                ? null
                : type.GetField(fieldName, flagsWithDeclaredOnly) ?? type.BaseType.GetFieldRecursive(fieldName, flags);
        }

        public static IEnumerable<FieldInfo> GetAllFieldsRecursive(this Type type, BindingFlags flags)
        {
            var flagsWithDeclaredOnly = flags | BindingFlags.DeclaredOnly;

            return type == null 
                ? Enumerable.Empty<FieldInfo>()
                : type.GetFields(flagsWithDeclaredOnly).Concat(type.BaseType.GetAllFieldsRecursive(flagsWithDeclaredOnly));
        }

        public static PropertyInfo GetPropertyRecursive(this Type type, string fieldName, BindingFlags flags)
        {
            var flagsWithDeclaredOnly = flags | BindingFlags.DeclaredOnly;

            return type == null
                ? null
                : type.GetProperty(fieldName, flagsWithDeclaredOnly) ?? type.BaseType.GetPropertyRecursive(fieldName, flags);
        }

        public static IEnumerable<PropertyInfo> GetAllPropertiesRecursive(this Type type, BindingFlags flags)
        {
            var flagsWithDeclaredOnly = flags | BindingFlags.DeclaredOnly;

            return type == null
                ? Enumerable.Empty<PropertyInfo>()
                : type.GetProperties(flagsWithDeclaredOnly).Concat(type.BaseType.GetAllPropertiesRecursive(flagsWithDeclaredOnly));
        }

        public static bool IsMostDerivedIn(this PropertyInfo property, IEnumerable<PropertyInfo> properties)
        {
            return !properties.Any(property.IsBaseOf);
        }

        private static bool IsBaseOf(this PropertyInfo property, PropertyInfo otherProperty)
        {
            return property.Name == otherProperty.Name &&
                   property != otherProperty &&
                   property.DeclaringType != null &&
                   otherProperty.DeclaringType != null &&
                   otherProperty.DeclaringType.IsSubclassOf(property.DeclaringType);
        }

        public static bool IsConcreteType(this Type type)
        {
            return type != null && !type.IsAbstract && !type.IsGenericTypeDefinition;
        }

        public static MethodInfo GetMethodRecursive(this Type type, string methodName, BindingFlags flags, Type[] parameterTypes)
        {
            var flagsWithDeclaredOnly = flags | BindingFlags.DeclaredOnly;

            return type == null
                ? null
                : type.GetMethod(methodName, flagsWithDeclaredOnly, null, parameterTypes, null)
                  ?? type.BaseType.GetMethodRecursive(methodName, flags, parameterTypes);
        }

        public static bool IsEnumerable(this Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            return type == null || type.BaseType == null
                ? Enumerable.Empty<Type>()
                : type.BaseType.GetBaseTypes().Union(new[] {type.BaseType});
        }

        public static bool IsConstructedFromOpenGeneric(this Type type, Type openGenericType)
        {
            return openGenericType.IsGenericTypeDefinition &&
                   type.IsGenericType &&
                   type.GetGenericTypeDefinition() == openGenericType;
        }

        public static bool IsSameAsOrConstructedFrom(this Type type, Type otherType)
        {
            return type == otherType || type.IsConstructedFromOpenGeneric(otherType);
        }

        public static string GetPrettyName(this Type type)
        {
            if (!type.IsGenericType) return type.Name;

            Type[] genericArguments = type.GetGenericArguments();
            var sb = new StringBuilder(type.Name.Substring(0, type.Name.IndexOf('`')));
            sb.Append("<");
            for (int idx = 0; idx < genericArguments.Length; idx++)
            {
                Type argType = genericArguments[idx];
                sb.Append(argType.GetPrettyName());
                if (idx < genericArguments.Length - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append(">");
            return sb.ToString();
        }
    }
}
