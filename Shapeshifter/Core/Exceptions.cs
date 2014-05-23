using System;
using System.Reflection;
using Newtonsoft.Json;
using Shapeshifter.Core.Deserialization;

namespace Shapeshifter.Core
{
    internal static class Exceptions
    {
        public const string CannotFindDeserializerId = "CannotFindDeserializer";
        public static Exception CannotFindDeserializer(ObjectProperties properties)
        {
            return SafeCreateException(() => new ShapeshifterException(CannotFindDeserializerId,
                String.Format("Cannot find deserializer for typeName {0} and version {1}.", properties.TypeName, properties.Version)));
        }

        public const string ShapeshifterAttributeMissingId = "ShapeshifterAttributeMissing";
        public static Exception ShapeshifterAttributeMissing(Type type)
        {
            return SafeCreateException(() => new ShapeshifterException(ShapeshifterAttributeMissingId,
                String.Format("ShapeshifterAttribute is missing from class {0}.", type.Name)));
        }

        public const string InvalidInputValueForConverterId = "InvalidInputValueForConverter";
        public static Exception InvalidInputValueForConverter(object value)
        {
            return SafeCreateException(() => new ShapeshifterException(InvalidInputValueForConverterId,
                String.Format("Invalid input type {0} for the converter.", value.GetType())));
        }

        public const string InstanceTypeDoesNotMatchSerializerTypeId = "InstanceTypeDoesNotMatchSerializerType";
        public static Exception InstanceTypeDoesNotMatchSerializerType(Type serializerType, Type instanceType)
        {
            return SafeCreateException(() => new ShapeshifterException(InstanceTypeDoesNotMatchSerializerTypeId,
                String.Format("Serializer type {0} does not match the instance type {1}.", serializerType.Name, instanceType.Name)));
        }

        public const string InvalidInputId = "InvalidInput";
        public static Exception InvalidInput()
        {
            return new ShapeshifterException(InvalidInputId, "The input used is not serialized with Shapeshifter.");
        }

        public const string KnownTypeMethodNotFoundId = "KnownTypeMethodNotFound";
        public static Exception KnownTypeMethodNotFound(string methodName, Type type)
        {
            return SafeCreateException(() => new ShapeshifterException(KnownTypeMethodNotFoundId,
                String.Format("KnownType static method {0} on type {1} not found.", methodName, type.Name)));
        }

        public const string KnownTypeMethodReturnValueIsInvalidId = "KnownTypeMethodReturnValueIsInvalid";
        public static Exception KnownTypeMethodReturnValueIsInvalid(string methodName, Type type)
        {
            return SafeCreateException(() => new ShapeshifterException(KnownTypeMethodReturnValueIsInvalidId,
                String.Format("KnownType static method {0} on type {1} returns invalid data. It should be IEnumerable<Type>.", methodName, type.Name)));
        }

        public const string SerializerResolutionFailedId = "SerializerResolutionFailed";
        public static Exception SerializerResolutionFailed(Type type)
        {
            return SafeCreateException(() => new ShapeshifterException(SerializerResolutionFailedId,
                String.Format("Cannot find packer for typeName {0}.", type.Name)));
        }

        public const string TheValueForTheKeyIsNotAnObjectId = "TheValueForTheKeyIsNotAnObject";
        public static Exception TheValueForTheKeyIsNotAnObject(string key)
        {
            return SafeCreateException(() => new ShapeshifterException(TheValueForTheKeyIsNotAnObjectId,
                String.Format("The value for the key {0} is not an object.", key)));
        }

        public const string OnlyOneSerializerAllowedForATypeId = "OnlyOneSerializerAllowedForAType";
        public static Exception OnlyOneSerializerAllowedForAType(Type type)
        {
            return SafeCreateException(() => new ShapeshifterException(OnlyOneSerializerAllowedForATypeId,
                String.Format("A serializer already exists for type {0}.", type.Name)));
        }

        public const string UnexpectedTokenEncounteredId = "UnexpectedTokenEncountered";
        public static Exception UnexpectedTokenEncountered(JsonToken tokenType)
        {
            return SafeCreateException(() => new ShapeshifterException(UnexpectedTokenEncounteredId,
                String.Format("Unexpected token {0} encountered during read.", tokenType)));
        }

        public const string InvalidUsageOfAttributeOnInstanceMethodId = "InvalidUsageOfAttributeOnInstanceMethod";
        public static Exception InvalidUsageOfAttributeOnInstanceMethod(Attribute attribute, MethodInfo methodInfo)
        {
            return SafeCreateException(() => new ShapeshifterException(InvalidUsageOfAttributeOnInstanceMethodId,
                String.Format("Invalid usage of attribute {0} on instance method {1}.{2}.",
                    attribute.GetType().Name, methodInfo.DeclaringType == null ? null : methodInfo.DeclaringType.FullName, methodInfo.Name)));
        }

        public const string UnexpectedEndOfTokenStreamId = "UnexpectedEndOfTokenStream";
        public static Exception UnexpectedEndOfTokenStream()
        {
            return new ShapeshifterException(UnexpectedEndOfTokenStreamId, "Unexpected end of token stream");
        }

        public const string DeserializerAttributeTargetTypeMustBeSpecifiedForAllDescendantsId = "DeserializerAttributeTargetTypeMustBeSpecifiedForAllDescendants";
        public static Exception DeserializerAttributeTargetTypeMustBeSpecifiedForAllDescendants(DeserializerAttribute attribute, MethodInfo methodInfo)
        {
            return SafeCreateException(() => new ShapeshifterException(DeserializerAttributeTargetTypeMustBeSpecifiedForAllDescendantsId,
                String.Format("DeserializerAttribute on method {0}.{1} must be specified with Type (instead of PackformatName) because ForAllDescendants is set to true.",
                    methodInfo.DeclaringType == null ? null : methodInfo.DeclaringType.FullName, methodInfo.Name)));
        }

        public const string InvalidSerializerMethodSignatureId = "InvalidSerializerMethodSignature";
        public static Exception InvalidSerializerMethodSignature(SerializerAttribute attribute, MethodInfo methodInfo, Type targetType)
        {
            return SafeCreateException(() => new ShapeshifterException(InvalidSerializerMethodSignatureId,
                String.Format("Serializer method {0}.{1} must be Action<IShapeshifterWriter,{2}>.",
                    methodInfo.DeclaringType == null ? null : methodInfo.DeclaringType.FullName, methodInfo.Name, targetType.Name)));
        }

        public const string InvalidDeserializerMethodSignatureId = "InvalidDeserializerMethodSignature";
        public static Exception InvalidDeserializerMethodSignature(DeserializerAttribute attribute, MethodInfo methodInfo)
        {
            return SafeCreateException(() => new ShapeshifterException(InvalidDeserializerMethodSignatureId,
                String.Format("Deserializer method {0}.{1} must be Func<object,IShapeshifterReader>.",
                    methodInfo.DeclaringType == null ? null : methodInfo.DeclaringType.FullName, methodInfo.Name)));
        }

        public const string InvalidDeserializerMethodSignatureForAllDescendantsId = "InvalidDeserializerMethodSignatureForAllDescendants";
        public static Exception InvalidDeserializerMethodSignatureForAllDescendants(DeserializerAttribute attribute, MethodInfo methodInfo)
        {
            return SafeCreateException(() => new ShapeshifterException(InvalidDeserializerMethodSignatureForAllDescendantsId,
                String.Format("Deserializer method {0}.{1} must be Func<object,IShapeshifterReader,Type> because ForAllDescendants is set to true.",
                    methodInfo.DeclaringType == null ? null : methodInfo.DeclaringType.FullName, methodInfo.Name)));
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