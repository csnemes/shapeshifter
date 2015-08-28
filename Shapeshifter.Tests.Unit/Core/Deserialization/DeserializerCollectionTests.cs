using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Core;
using Shapeshifter.Core.Deserialization;

namespace Shapeshifter.Tests.Unit.Core.Deserialization
{
    [TestFixture]
    public class DeserializerCollectionTests
    {
        [Test]
        public void AddForConcreteVersion_ResolveForAnotherVersion_ReturnsNull()
        {
            var myDeserializer = new CustomDeserializer("MyPackformatName", 1, null);

            var deserializerCollection = (DeserializerCollection)DeserializerCollection.New.Add(myDeserializer);

            deserializerCollection.ResolveDeserializer(new DeserializerKey("MyPackformatName", 2)).Should().BeNull();
        }

        [Test]
        public void AddForConcreteVersion_Resolve_Success()
        {
            var myDeserializer = new CustomDeserializer("MyPackformatName", 1, null);

            var deserializerCollection = (DeserializerCollection) DeserializerCollection.New.Add(myDeserializer);

            deserializerCollection.ResolveDeserializer(new DeserializerKey("MyPackformatName", 1)).Should().Be(myDeserializer);
        }

        [Test]
        public void AddCustomAndDefaultDeserializerForConcreteVersion_InAnyOrder_Resolve_CustomDeserializerIsReturned()
        {
            var myDefaultDeserializer = new DefaultDeserializer(new SerializableTypeInfo(null, "MyPackformatName", 1, new List<FieldOrPropertyMemberInfo>()));
            var myCustomDeserializer = new CustomDeserializer("MyPackformatName", 1, null);

            {
                var deserializerCollection = (DeserializerCollection) DeserializerCollection.New
                    .Add(myCustomDeserializer)
                    .Add(myDefaultDeserializer);

                deserializerCollection.ResolveDeserializer(new DeserializerKey("MyPackformatName", 1)).Should().Be(myCustomDeserializer);
            }
            {
                var deserializerCollection = (DeserializerCollection) DeserializerCollection.New
                    .Add(myDefaultDeserializer)
                    .Add(myCustomDeserializer);

                deserializerCollection.ResolveDeserializer(new DeserializerKey("MyPackformatName", 1)).Should().Be(myCustomDeserializer);
            }
        }

        [Test]
        public void AddTwoCustomDeserializersForTheSameMethod_NotThrows()
        {
            var myCustomDeserializer1 = new CustomDeserializer("MyPackformatName", 1, null);
            var myCustomDeserializer2 = new CustomDeserializer("MyPackformatName", 1, null);

            Action action = () =>
            {
               var deserializerCollection = (DeserializerCollection) DeserializerCollection.New
                    .Add(myCustomDeserializer1)
                    .Add(myCustomDeserializer2);
            };
            action.ShouldNotThrow<ShapeshifterException>();
        }

        [Test]
        public void AddTwoCustomDeserializersForDifferentMethods_Throws()
        {

            var method1 = typeof (DeserializerCollectionTests).GetMethod("Method1", BindingFlags.Instance | BindingFlags.NonPublic);
            var method2 = typeof(DeserializerCollectionTests).GetMethod("Method2", BindingFlags.Instance | BindingFlags.NonPublic);

            var myCustomDeserializer1 = new CustomDeserializer("MyPackformatName", 1, method1);
            var myCustomDeserializer2 = new CustomDeserializer("MyPackformatName", 1, method2);

            Action action = () =>
            {
                var deserializerCollection = (DeserializerCollection)DeserializerCollection.New
                     .Add(myCustomDeserializer1)
                     .Add(myCustomDeserializer2);
            };
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.DeserializerAlreadyExistsId);
        }

        private void Method1() { }
        private void Method2() { }

        [Test]
        public void AddTwoDefaultDeserializers_Throws()
        {
            var myDefaultDeserializer1 = new DefaultDeserializer(new SerializableTypeInfo(null, "MyPackformatName", 1, new List<FieldOrPropertyMemberInfo>()));
            var myDefaultDeserializer2 = new DefaultDeserializer(new SerializableTypeInfo(null, "MyPackformatName", 1, new List<FieldOrPropertyMemberInfo>()));

            Action action = () =>
            {
            var deserializerCollection = (DeserializerCollection)DeserializerCollection.New
                .Add(myDefaultDeserializer1)
                .Add(myDefaultDeserializer2);
            };
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.DeserializerAlreadyExistsId);
        }

        [Test]
        public void AddCustoDeserializerForDerivedType_OverridesCustomDeserializerCreatedFromBaseTypeForAllDescendants_WorksInAnyOrder()
        {
            var explicitDeserializer = new CustomDeserializer("MyPackformatName", 1, null, CustomSerializerCreationReason.Explicit);
            var implicitDeserializer = new CustomDeserializer("MyPackformatName", 1, null, CustomSerializerCreationReason.ImplicitByBaseType, typeof(int));

            {
                var deserializerCollection = (DeserializerCollection)DeserializerCollection.New
                    .Add(explicitDeserializer)
                    .Add(implicitDeserializer);

                deserializerCollection.ResolveDeserializer(new DeserializerKey("MyPackformatName", 1)).Should().Be(explicitDeserializer);
            }
            {
                var deserializerCollection = (DeserializerCollection)DeserializerCollection.New
                    .Add(implicitDeserializer)
                    .Add(explicitDeserializer);

                deserializerCollection.ResolveDeserializer(new DeserializerKey("MyPackformatName", 1)).Should().Be(explicitDeserializer);
            }
        }
    }
}
