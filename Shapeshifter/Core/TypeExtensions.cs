﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shapeshifter.Core
{
    public static class TypeExtensions
    {
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
    }
}