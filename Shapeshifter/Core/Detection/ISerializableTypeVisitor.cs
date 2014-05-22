using System.Reflection;

namespace Shapeshifter.Core.Detection
{
    internal interface ISerializableTypeVisitor
    {
        void VisitDeserializerOnClass(DeserializerAttribute attribute, TypeInfo typeInfo);

        void VisitSerializerOnClass(TypeInfo typeInfo);

        void VisitDeserializerMethod(DeserializerAttribute attribute, MethodInfo method);

        void VisitSerializerMethod(SerializerAttribute attribute, MethodInfo method);
    }
}