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
        public void ResolveForNotAddedType_Thrown()
        {
            var serializerCollection = (SerializerCollection)SerializerCollection.New;

            Action action = () => serializerCollection.ResolveSerializer(typeof(int));
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.SerializerResolutionFailedId);
        }

        [Test]
        public void Add_Resolve_Success()
        {
            var mySerializer = new CustomSerializer(typeof(int), null, 1, null);

            var serializerCollection = (SerializerCollection) SerializerCollection.New.Add(mySerializer);

            serializerCollection.ResolveSerializer(typeof(int)).Should().Be(mySerializer);
        }

        [Test]
        public void AddCustomAndDefaultSerializer_InAnyOrder_Resolve_CustomSerializerIsReturned()
        {
            var myDefaultSerializer = new DefaultSerializer(new SerializableTypeInfo(typeof(int), "MyInt", 1, new List<SerializableMemberInfo>()));
            var myCustomSerializer = new CustomSerializer(typeof(int), null, 1, null);

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
        public void AddTwoCustomSerializers_Throws()
        {
            var myCustomSerializer1 = new CustomSerializer(typeof(int), null, 1, null);
            var myCustomSerializer2 = new CustomSerializer(typeof(int), null, 1, null);

            Action action = () =>
            {
                var serializerCollection = (SerializerCollection) SerializerCollection.New
                    .Add(myCustomSerializer1)
                    .Add(myCustomSerializer2);
            };
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.SerializerAlreadyExistsId);
        }

        [Test]
        public void AddTwoDefaultSerializers_Throws()
        {
            var myDefaultSerializer1 = new DefaultSerializer(new SerializableTypeInfo(typeof(int), "MyInt", 1, new List<SerializableMemberInfo>()));
            var myDefaultSerializer2 = new DefaultSerializer(new SerializableTypeInfo(typeof(int), "MyInt", 1, new List<SerializableMemberInfo>()));

            Action action = () =>
            {
            var serializerCollection = (SerializerCollection) SerializerCollection.New
                .Add(myDefaultSerializer1)
                .Add(myDefaultSerializer2);
            };
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.SerializerAlreadyExistsId);
        }

        [Test]
        public void AddCustomSerializerForDerivedType_OverridesCustomSerializerCreatedFromBaseTypeForAllDescendants_WorksInAnyOrder()
        {
            var explicitSerializer = new CustomSerializer(typeof (int), null, 1, null, CustomSerializerCreationReason.Explicit);
            var implicitSerializer = new CustomSerializer(typeof (int), null, 1, null, CustomSerializerCreationReason.ImplicitByBaseType);

            {
                var serializerCollection = (SerializerCollection) SerializerCollection.New
                    .Add(explicitSerializer)
                    .Add(implicitSerializer);

                serializerCollection.ResolveSerializer(typeof (int)).Should().Be(explicitSerializer);
            }
            {
                var serializerCollection = (SerializerCollection) SerializerCollection.New
                    .Add(implicitSerializer)
                    .Add(explicitSerializer);

                serializerCollection.ResolveSerializer(typeof (int)).Should().Be(explicitSerializer);
            }
        }
    }
}
