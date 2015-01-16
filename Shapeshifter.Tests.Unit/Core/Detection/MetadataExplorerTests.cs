using System.Linq;
using System.Xml;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Core;
using Shapeshifter.Core.Deserialization;
using Shapeshifter.Core.Detection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Shapeshifter.Tests.Unit.Core.Detection
{
    [TestFixture]
    public class MetadataExplorerTests
    {

        [Test]
        public void ShouldDetectNonStaticDeserializer()
        {
            var result = MetadataExplorer.CreateFor(typeof(NonStaticDeserializerClass)).Deserializers;
            result.Count().Should().Be(1);
        }

        [Test]
        public void ShouldDetectNonStaticDeserializerAndFailIfNoPublicDefaultConstructor()
        {
            Action action = () => MetadataExplorer.CreateFor(typeof(NonStaticDeserializerClassNoConstructor));
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.TypeHasNoPublicDefaultConstructorId); 
        }

        [Test]
        public void ShouldDetectMemberTypesWithDataMemberAttribute()
        {
            var result = MetadataExplorer.CreateFor(typeof (ExternalClass)).Serializers;
            result.ResolveSerializer(typeof (InnerClass)).Should().NotBeNull();
        }

        [Test]
        public void ShouldDetectItemTypeOfListMembersWithDataMemberAttribute()
        {
            var result = MetadataExplorer.CreateFor(typeof (ClassWithList)).Serializers;
            result.ResolveSerializer(typeof (InnerClass)).Should().NotBeNull();
        }

        [Test]
        public void ShouldDetectKnownTypesAddedDirectly()
        {
            var result = MetadataExplorer.CreateFor(typeof (ClassWithKnownTypes)).Serializers;
            result.ResolveSerializer(typeof (InnerClass)).Should().NotBeNull();
        }

        [Test]
        public void ShouldDetectKnownTypesAddedByMethod()
        {
            var result = MetadataExplorer.CreateFor(typeof (ClassWithKnownTypesMethod)).Serializers;
            result.ResolveSerializer(typeof (InnerClass)).Should().NotBeNull();
        }

        [Test]
        public void AbstractClass_ShouldNotCreateDefaultSerializer()
        {
            var metadataExplorer = MetadataExplorer.CreateFor(typeof (MyAbstractType));
            Action action = () => metadataExplorer.Serializers.ResolveSerializer(typeof (MyAbstractType));
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.SerializerResolutionFailedId);
        }

        [Test]
        public void AbstractClass_ShouldNotCreateDefaultDeserializer()
        {
            var metadataExplorer = MetadataExplorer.CreateFor(typeof (MyAbstractType));
            metadataExplorer.Deserializers.ResolveDeserializer(new DeserializerKey("MyAbstractType", 1)).Should().BeNull();
        }

        [Test]
        public void OpenGenericClass_ShouldNotCreateDefaultSerializer()
        {
            var metadataExplorer = MetadataExplorer.CreateFor(typeof (MyOpenGenericType<>));
            Action action = () => metadataExplorer.Serializers.ResolveSerializer(typeof (MyOpenGenericType<>));
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.SerializerResolutionFailedId);
        }

        [Test]
        public void OpenGenericClass_ShouldNotCreateDefaultDeserializer()
        {
            var metadataExplorer = MetadataExplorer.CreateFor(typeof (MyOpenGenericType<>));
            metadataExplorer.Deserializers.ResolveDeserializer(new DeserializerKey("MyOpenGenericType<T>", 1)).Should().BeNull();
        }

        [Test]
        public void AbstractClass_ShouldNotCreateCustomSerializer()
        {
            var metadataExplorer = MetadataExplorer.CreateFor(typeof (MyAbstractTypeWithCustomSerializer));
            Action action = () => metadataExplorer.Serializers.ResolveSerializer(typeof (MyAbstractTypeWithCustomSerializer));
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.SerializerResolutionFailedId);
        }

        [Test]
        public void AbstractClass_ShouldNotCreateCustomDeserializer()
        {
            var metadataExplorer = MetadataExplorer.CreateFor(typeof (MyAbstractTypeWithCustomSerializer));
            metadataExplorer.Deserializers.ResolveDeserializer(new DeserializerKey("MyAbstractTypeWithCustomSerializer", 1)).Should().BeNull();
        }

        [Test]
        public void OpenGenericClass_ShouldNotCreateCustomSerializer()
        {
            var metadataExplorer = MetadataExplorer.CreateFor(typeof(MyOpenGenericTypeWithCustomSerializer<>));
            Action action = () => metadataExplorer.Serializers.ResolveSerializer(typeof(MyOpenGenericTypeWithCustomSerializer<>));
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.SerializerResolutionFailedId);
        }

        [Test]
        public void OpenGenericClass_ShouldNotCreateCustomDeserializer()
        {
            var metadataExplorer = MetadataExplorer.CreateFor(typeof(MyOpenGenericTypeWithCustomSerializer<>));
            metadataExplorer.Deserializers.ResolveDeserializer(new DeserializerKey("MyOpenGenericTypeWithCustomSerializer<T>", 1)).Should().BeNull();
        }

        [Test]
        public void CustomSerializer_WithNoVersion_ShouldCreateSerializerWithHash()
        {
            var metadataExplorer = MetadataExplorer.CreateFor(typeof(MyTypeWithSerializerWithNoVersion));
            metadataExplorer.Serializers.Should().ContainSingle(i => i.Type == typeof (MyTypeWithSerializerWithNoVersion) && i.Version != 0);
        }

        [Test]
        public void CustomSerializerForChild_WithNoVersion_ShouldCreateSerializerWithHash()
        {
            var metadataExplorer = MetadataExplorer.CreateFor(typeof (MyTypeWithSerializerWithNoVersion), new[] {Assembly.GetExecutingAssembly()});
            metadataExplorer.Serializers.Should().ContainSingle(i => i.Type == typeof(MyChildWithSerializerWithNoVersion) && i.Version != 0);
        }

        [Test]
        public void CustomSerializer_WithExplicitVersion_ShouldCreateSerializerWithExplicitVersion()
        {
            var metadataExplorer = MetadataExplorer.CreateFor(typeof(MyTypeWithSerializerWithExplicitVersion));
            metadataExplorer.Serializers.Should().ContainSingle(i => i.Type == typeof(MyTypeWithSerializerWithExplicitVersion) && i.Version == 1);
        }

        [Test]
        public void CustomSerializerForChild_WithExplicitVersion_ShouldCreateSerializerWithExplicitVersion()
        {
            var metadataExplorer = MetadataExplorer.CreateFor(typeof(MyTypeWithSerializerWithExplicitVersion), new[] { Assembly.GetExecutingAssembly() });
            metadataExplorer.Serializers.Should().ContainSingle(i => i.Type == typeof(MyChildWithSerializerWithExplicitVersion) && i.Version == 1);
        }

        [Test]
        public void CustomDeserializer_WithNoVersion_ShouldCreateDeserializerWithHash()
        {
            var metadataExplorer = MetadataExplorer.CreateFor(typeof(MyTypeWithSerializerWithNoVersion));
            metadataExplorer.Deserializers.Should().ContainSingle(i => i.PackformatName == "MyTypeWithSerializerWithNoVersion" && i.Version != 0);
        }

        [Test]
        public void CustomDeserializerForChild_WithNoVersion_ShouldCreateDeserializerWithHash()
        {
            var metadataExplorer = MetadataExplorer.CreateFor(typeof(MyTypeWithSerializerWithNoVersion), new[] { Assembly.GetExecutingAssembly() });
            metadataExplorer.Deserializers.Should().ContainSingle(i => i.PackformatName == "MyChildWithSerializerWithNoVersion" && i.Version != 0);
        }

        [Test]
        public void CustomDeserializer_WithExplicitVersion_ShouldCreateDeserializerWithExplicitVersion()
        {
            var metadataExplorer = MetadataExplorer.CreateFor(typeof(MyTypeWithSerializerWithExplicitVersion));
            metadataExplorer.Deserializers.Should().ContainSingle(i => i.PackformatName == "MyTypeWithSerializerWithExplicitVersion" && i.Version == 1);
        }

        [Test]
        public void CustomDeserializerForChild_WithExplicitVersion_ShouldCreateDeserializerWithExplicitVersion()
        {
            var metadataExplorer = MetadataExplorer.CreateFor(typeof(MyTypeWithSerializerWithExplicitVersion), new[] { Assembly.GetExecutingAssembly() });
            metadataExplorer.Deserializers.Should().ContainSingle(i => i.PackformatName == "MyChildWithSerializerWithExplicitVersion" && i.Version == 1);
        }

        [Test]
        public void CustomDeserializerWithTypeAndNoVersion_ShouldThrow()
        {
            Action action = () => MetadataExplorer.CreateFor(typeof(MyTypeWithCustomDeserializerWithTypeAndNoVersion));
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.CustomDeserializerMustSpecifyVersionId);
        }

        [Test]
        public void BaseClassWithoutDataContract_ShouldThrow()
        {
            Action action = () => MetadataExplorer.CreateFor(typeof(DerivedClass));
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.DataContractAttributeMissingFromHierarchyId);
        }

        [Test]
        public void BaseClassWithoutDataContractMultiLevelHierarchy_ShouldThrow()
        {
            Action action = () => MetadataExplorer.CreateFor(typeof(MostDerivedClass));
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.DataContractAttributeMissingFromHierarchyId);
        }

        [Test]
        public void StructWithDataContract_HierarchyCheck_ShouldNotThrow()
        {
            Action action = () => MetadataExplorer.CreateFor(typeof(TestStruct));
            action.ShouldNotThrow();
        }


        [ShapeshifterRoot]
        [DataContract]
        private struct TestStruct
        {

        }

        [Test]
        public void SameClassNameInDifferentNamespace_ShouldThrow()
        {
            Action action = () => MetadataExplorer.CreateFor(typeof(OuterClass));
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.PackformatNameCollisionyId);
        }

        [ShapeshifterRoot]
        [DataContract]
        private class OuterClass
        {
            [DataMember]
            public InnerClassWithSameName Item1 { get; set; }
            [DataMember]
            public OtherNamespace.InnerClassWithSameName Item2 { get; set; }
        }

        [DataContract]
        private class InnerClassWithSameName
        {
            [DataMember]
            public string Item { get; set; }
        }

        private class BaseClass
        {
            public string BaseItem { get; set; }
        }

        private class LeastDerivedClass : BaseClass
        {
        }

        [ShapeshifterRoot]
        [DataContract]
        private class MostDerivedClass : LeastDerivedClass
        {
        }

        [ShapeshifterRoot]
        [DataContract]
        private class DerivedClass : BaseClass
        {
            public string DerivedItem { get; set; }
        }


        [DataContract]
        [ShapeshifterRoot]
        private class ExternalClass
        {
            [DataMember]
            public string PrimitiveType { get; set; }

            [DataMember]
            public InnerClass Inner { get; set; }
        }

        [DataContract]
        [ShapeshifterRoot]
        private class ClassWithList
        {
            [DataMember]
            public List<InnerClass> Inner { get; set; }
        }

        [DataContract]
        private class InnerClass
        {

        }

        [DataContract]
        [ShapeshifterRoot]
        [KnownType(typeof (InnerClass))]
        private class ClassWithKnownTypes
        {
            [DataMember]
            public string PrimitiveType { get; set; }
        }

        [DataContract]
        [ShapeshifterRoot]
        [KnownType("GetKnownTypes")]
        private class ClassWithKnownTypesMethod
        {
            [DataMember]
            public string PrimitiveType { get; set; }

            private static Type[] GetKnownTypes()
            {
                return new[] {typeof (InnerClass)};
            }
        }

        [ShapeshifterRoot(1)]
        [DataContract]
        public abstract class MyAbstractType
        {
        }

        [ShapeshifterRoot(1)]
        [DataContract]
        public class MyOpenGenericType<T>
        {
        }

        public abstract class MyAbstractTypeWithCustomSerializer
        {
            [Serializer(typeof (MyAbstractTypeWithCustomSerializer), 1)]
            public static void Serialize(IShapeshifterWriter writer, MyAbstractTypeWithCustomSerializer input)
            {
            }

            [Deserializer(typeof(MyAbstractTypeWithCustomSerializer), 1)]
            public static object Deserialize(IShapeshifterReader writer)
            {
                return null;
            }
        }

        public class MyOpenGenericTypeWithCustomSerializer<T>
        {
            [Serializer(typeof(MyOpenGenericTypeWithCustomSerializer<>), 1)]
            public static void Serialize(IShapeshifterWriter writer, object input)
            {
            }

            [Deserializer(typeof(MyOpenGenericTypeWithCustomSerializer<>), 1)]
            public static object Deserialize(IShapeshifterReader writer)
            {
                return null;
            }
        }

        public class MyTypeWithSerializerWithNoVersion
        {
            [Serializer(typeof (MyTypeWithSerializerWithNoVersion), ForAllDescendants = true)]
            public static void Serialize(IShapeshifterWriter writer, object input)
            {
            }

            [Deserializer(typeof(MyTypeWithSerializerWithNoVersion), ForAllDescendants = true)]
            public static object Deserialize(IShapeshifterReader writer, Type targetType)
            {
                return null;
            }
        }

        public class MyChildWithSerializerWithNoVersion : MyTypeWithSerializerWithNoVersion
        { }

        public class MyTypeWithSerializerWithExplicitVersion
        {
            [Serializer(typeof(MyTypeWithSerializerWithExplicitVersion), 1, ForAllDescendants = true)]
            public static void Serialize(IShapeshifterWriter writer, object input)
            {
            }

            [Deserializer(typeof(MyTypeWithSerializerWithExplicitVersion), 1, ForAllDescendants = true)]
            public static object Deserialize(IShapeshifterReader reader, Type targetType)
            {
                return null;
            }
        }
        public class MyChildWithSerializerWithExplicitVersion : MyTypeWithSerializerWithExplicitVersion
        { }

        public class MyTypeWithCustomDeserializerWithTypeAndNoVersion
        {
            [Deserializer(typeof(int))]
            public static object Deserialize(IShapeshifterReader reader)
            {
                return null;
            }
        }

        public class NonStaticDeserializerClass
        {
            [Deserializer(typeof(int), 1)]
            public object Deserialize(IShapeshifterReader reader)
            {
                return null;
            }
        }

        public class NonStaticDeserializerClassNoConstructor
        {
            public NonStaticDeserializerClassNoConstructor(int i)
            { }

            [Deserializer(typeof(int), 1)]
            public object Deserialize(IShapeshifterReader reader)
            {
                return null;
            }
        }
    }
}


namespace OtherNamespace
{
    [DataContract]
    public class InnerClassWithSameName
    {
        [DataMember]
        public string Item { get; set; }
    }
}


