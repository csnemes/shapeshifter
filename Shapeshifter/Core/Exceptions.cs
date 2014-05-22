using System;
using System.Reflection;
using Newtonsoft.Json;
using Shapeshifter.Core.Deserialization;

namespace Shapeshifter.Core
{
    internal static class Exceptions
    {
        public static Exception CannotFindDeserializer(ObjectProperties properties)
        {
            return
                SafeCreateException(
                    () =>
                        new ArgumentException(String.Format(
                            "Cannot find deserializer for typeName {0} and version {1}.",
                            properties.TypeName, properties.Version)));
        }

        public static Exception InvalidUseOfSerializerAttributeOnMethod(MethodInfo method)
        {
            return
                SafeCreateException(
                    () =>
                        new ShapeshifterException(
                            String.Format("SerializerAttribute parameters are invalid on method {0}.{1}.",
                                method.DeclaringType.Name, method.Name)));
        }

        public static Exception InvalidUseOfDeserializerAttributeOnMethod(MethodInfo method)
        {
            return
                SafeCreateException(
                    () =>
                        new ShapeshifterException(
                            String.Format("DeserializerAttribute parameters are invalid on method {0}.{1}.",
                                method.DeclaringType.Name, method.Name)));
        }

        public static Exception InvalidUseOfDeserializerAttributeOnClass(Type type)
        {
            return
                SafeCreateException(
                    () =>
                        new ShapeshifterException(
                            String.Format("DeserializerAttribute parameters are invalid on class {0}.",
                                type.Name)));
        }

        public static Exception ShapeshifterRootAttributeMissing(Type type)
        {
            return
                SafeCreateException(
                    () => new ShapeshifterException(String.Format("ShapeshifterRootAttribute is missing from class {0}.",
                        type.Name)));
        }


        public static Exception InvalidInputValueForConverter(object value)
        {
            return
                SafeCreateException(
                    () => new ShapeshifterException(String.Format("Invalid input type {0} for the converter.",
                        value.GetType())));
        }

        public static Exception InstanceTypeDoesNotMatchSerializerType(Type serializerType, Type instanceType)
        {
            return
                SafeCreateException(
                    () =>
                        new ShapeshifterException(
                            String.Format("Serializer type {0} does not match the instance type {1}.",
                                serializerType.Name, instanceType.Name)));
        }

        public static Exception InvalidInput()
        {
            return new ShapeshifterException("The input used is not serialized with Shapeshifter.");
        }

        public static Exception KnownTypeMethodNotFound(string methodName, Type type)
        {
            return
                SafeCreateException(
                    () => new ShapeshifterException(String.Format("KnownType static method {0} on type {1} not found.",
                        methodName, type.Name)));
        }

        public static Exception KnownTypeMethodReturnValueIsInvalid(string methodName, Type type)
        {
            return
                SafeCreateException(
                    () =>
                        new ShapeshifterException(
                            String.Format(
                                "KnownType static method {0} on type {1} returns invalid data. It should be IEnumerable<Type>.",
                                methodName, type.Name)));
        }

        public static Exception SerializerResolutionFailed(Type type)
        {
            return
                SafeCreateException(
                    () => new ShapeshifterException(String.Format("Cannot find packer for typeName {0}.",
                        type.Name)));
        }

        public static Exception TheValueForTheKeyIsNotAnObject(string key)
        {
            return
                SafeCreateException(
                    () => new ShapeshifterException(String.Format("The value for the key {0} is not an object.",
                        key)));
        }

        public static Exception OnlyOneSerializerAllowedForAType(Type type)
        {
            return SafeCreateException(
                () => new ShapeshifterException(String.Format("A serializer already exists for type {0}.",
                    type.Name)));
        }

        public static Exception UnexpectedTokenEncountered(JsonToken tokenType)
        {
            return SafeCreateException(
                () => new ShapeshifterException(String.Format("Unexpected token {0} encountered during read.",
                    tokenType)));
        }

        public static Exception InvalidSurrogate(Type type)
        {
            return SafeCreateException(
                () => new ShapeshifterException(String.Format("Type {0} misses DataContract attribute.",
                    type.Name)));
        }

        public static Exception KnownTypeAttributeMissingFromSurrogateConverter(Type type)
        {
            return SafeCreateException(
                () => new ShapeshifterException(String.Format("Surrogate converter type {0} misses KnownType attribute, which is required to describe the surrogate type(s).",
                    type.Name)));
        }

        private static Exception SafeCreateException(Func<Exception> exceptionCreationFunc)
        {
            try
            {
                return exceptionCreationFunc();
            }
            catch (Exception ex)
            {
                return
                    new ApplicationException(
                        String.Format(
                            "Failed to create an exception. An exception occured while trying to create the real" +
                            " exception with {0}.", exceptionCreationFunc), ex);
            }
        }

    }
}