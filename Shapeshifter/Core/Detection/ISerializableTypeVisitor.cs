using System.Reflection;

namespace Shapeshifter.Core.Detection
{
    internal interface ISerializableTypeVisitor
    {
        void VisitSerializableClass(SerializableTypeInfo serializableTypeInfo);

        void VisitDeserializerMethod(DeserializerAttribute attribute, MethodInfo methodInfo);

        void VisitSerializerMethod(SerializerAttribute attribute, MethodInfo methodInfo);
    }
}