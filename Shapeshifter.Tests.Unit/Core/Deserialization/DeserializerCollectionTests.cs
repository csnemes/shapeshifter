using System;
using System.Collections.Generic;
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
        public void AddForUnspecifiedVersion_Resolve_Success()
        {
            var myDeserializer = new CustomDeserializer("MyPackformatName", 0, null);

            var deserializerCollection = (DeserializerCollection)DeserializerCollection.New.Add(myDeserializer);

            deserializerCollection.ResolveDeserializer(new DeserializerKey("MyPackformatName", 1)).Should().Be(myDeserializer);
        }

        [Test]
        public void AddCustomAndDefaultDeserializerForConcreteVersion_InAnyOrder_Resolve_CustomDeserializerIsReturned()
        {
            var myDefaultDeserializer = new DefaultDeserializer(new SerializableTypeInfo(null, "MyPackformatName", 1, new List<SerializableMemberInfo>()));
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
        [ExpectedException(typeof(ShapeshifterException), Handler = "AddTwoCustomDeserializers_CheckExceptionId")]
        public void AddTwoCustomDeserializers_Throws()
        {
            var myCustomDeserializer1 = new CustomDeserializer("MyPackformatName", 1, null);
            var myCustomDeserializer2 = new CustomDeserializer("MyPackformatName", 1, null);

            var deserializerCollection = (DeserializerCollection) DeserializerCollection.New
                .Add(myCustomDeserializer1)
                .Add(myCustomDeserializer2);
        }

        [Test]
        [ExpectedException(typeof(ShapeshifterException), Handler = "AddTwoCustomDeserializers_CheckExceptionId")]
        public void AddTwoDefaultDeserializers_Throws()
        {
            var myDefaultDeserializer1 = new DefaultDeserializer(new SerializableTypeInfo(null, "MyPackformatName", 1, new List<SerializableMemberInfo>()));
            var myDefaultDeserializer2 = new DefaultDeserializer(new SerializableTypeInfo(null, "MyPackformatName", 1, new List<SerializableMemberInfo>()));

            var deserializerCollection = (DeserializerCollection)DeserializerCollection.New
                .Add(myDefaultDeserializer1)
                .Add(myDefaultDeserializer2);
        }

        private void AddTwoCustomDeserializers_CheckExceptionId(Exception exception)
        {
            (exception as ShapeshifterException).Id.Should().Be(Exceptions.DeserializerAlreadyExistsId);
        }
    }
}
