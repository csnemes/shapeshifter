using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Core;
using Shapeshifter.Core.Serialization;

namespace Shapeshifter.Tests.Unit.Core.Serialization
{
    [TestFixture]
    public class SerializerCollectionTests
    {
        [Test]
        [ExpectedException(typeof(ShapeshifterException), Handler = "ResolveForNotAddedType_CheckExcpetionId")]
        public void ResolveForNotAddedType_Thrown()
        {
            var serializerCollection = (SerializerCollection)SerializerCollection.New;

            serializerCollection.ResolveSerializer(typeof(int)).Should().BeNull();
        }

        private static void ResolveForNotAddedType_CheckExcpetionId(Exception exception)
        {
            (exception as ShapeshifterException).Id.Should().Be(Exceptions.SerializerResolutionFailedId);
        }

        [Test]
        public void Add_Resolve_Success()
        {
            var mySerializer = new CustomSerializer(typeof(int), 1, null);

            var serializerCollection = (SerializerCollection) SerializerCollection.New.Add(mySerializer);

            serializerCollection.ResolveSerializer(typeof(int)).Should().Be(mySerializer);
        }

        [Test]
        public void AddCustomAndDefaultSerializer_InAnyOrder_Resolve_CustomSerializerIsReturned()
        {
            var myDefaultSerializer = new DefaultSerializer(new SerializableTypeInfo(typeof(int), "MyInt", 1, new List<SerializableMemberInfo>()));
            var myCustomSerializer = new CustomSerializer(typeof(int), 1, null);

            {
                var serializerCollection = (SerializerCollection) SerializerCollection.New
                    .Add(myDefaultSerializer)
                    .Add(myCustomSerializer);

                serializerCollection.ResolveSerializer(typeof(int)).Should().Be(myCustomSerializer);
            }
            {
                var serializerCollection = (SerializerCollection)SerializerCollection.New
                    .Add(myCustomSerializer)
                    .Add(myDefaultSerializer);

                serializerCollection.ResolveSerializer(typeof(int)).Should().Be(myCustomSerializer);
            }
        }

        [Test]
        [ExpectedException(typeof(ShapeshifterException), Handler = "AddTwoSerializers_CheckExceptionId")]
        public void AddTwoCustomSerializers_Throws()
        {
            var myCustomSerializer1 = new CustomSerializer(typeof(int), 1, null);
            var myCustomSerializer2 = new CustomSerializer(typeof(int), 1, null);

            var serializerCollection = (SerializerCollection) SerializerCollection.New
                .Add(myCustomSerializer1)
                .Add(myCustomSerializer2);
        }

        [Test]
        [ExpectedException(typeof(ShapeshifterException), Handler = "AddTwoSerializers_CheckExceptionId")]
        public void AddTwoDefaultSerializers_Throws()
        {
            var myDefaultSerializer1 = new DefaultSerializer(new SerializableTypeInfo(typeof(int), "MyInt", 1, new List<SerializableMemberInfo>()));
            var myDefaultSerializer2 = new DefaultSerializer(new SerializableTypeInfo(typeof(int), "MyInt", 1, new List<SerializableMemberInfo>()));

            var serializerCollection = (SerializerCollection) SerializerCollection.New
                .Add(myDefaultSerializer1)
                .Add(myDefaultSerializer2);
        }

        private void AddTwoSerializers_CheckExceptionId(Exception exception)
        {
            (exception as ShapeshifterException).Id.Should().Be(Exceptions.SerializerAlreadyExistsId);
        }
    }
}
