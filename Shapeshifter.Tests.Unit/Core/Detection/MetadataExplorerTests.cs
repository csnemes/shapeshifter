using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Core;
using Shapeshifter.Core.Deserialization;
using Shapeshifter.Core.Detection;

namespace Shapeshifter.Tests.Unit.Core.Detection
{
    [TestFixture]
    public class MetadataExplorerTests
    {
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


        [DataContract]
        [Shapeshifter]
        private class ExternalClass
        {
            [DataMember]
            public string PrimitiveType { get; set; }

            [DataMember]
            public InnerClass Inner { get; set; }
        }

        [DataContract]
        [Shapeshifter]
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
        [Shapeshifter]
        [KnownType(typeof (InnerClass))]
        private class ClassWithKnownTypes
        {
            [DataMember]
            public string PrimitiveType { get; set; }
        }

        [DataContract]
        [Shapeshifter]
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

        [Shapeshifter(1)]
        [DataContract]
        public abstract class MyAbstractType
        {
        }

        [Shapeshifter(1)]
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
    }
}
