using FluentAssertions;
using NUnit.Framework;
using System;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class CustomSerializerForVersioningTests
    {
        [Test]
        public void SameVersionWriteAndReadBack_OriginalVersion()
        {
            var serializer = new Shapeshifter<NonDataContractClass>(new[] { typeof(SerializationForNonDataContractClassVersion1) });
            var serialized = serializer.Serialize(new NonDataContractClass() { Value = "42"});
            var result = serializer.Deserialize(serialized);
            result.Value.Should().Be("42");
        }

        [Test]
        public void SameVersionWriteAndReadBack_NewVersion()
        {
            var serializer = new Shapeshifter<NonDataContractClass>(new[] { typeof(SerializationForNonDataContractClassVersion2) });
            var serialized = serializer.Serialize(new NonDataContractClass() { Value = "42" });
            var result = serializer.Deserialize(serialized);
            result.Value.Should().Be("42");
        }

        [Test]
        public void OldVersionReadBack_NewVersion()
        {
            var oldSerializer = new Shapeshifter<NonDataContractClass>(new[] { typeof(SerializationForNonDataContractClassVersion1) });
            var serialized = oldSerializer.Serialize(new NonDataContractClass() { Value = "42" });

            var serializer = new Shapeshifter<NonDataContractClass>(new[] { typeof(SerializationForNonDataContractClassVersion2) });
            var result = serializer.Deserialize(serialized);
            result.Value.Should().Be("42");
        }

        private class SerializationForNonDataContractClassVersion1
        {
            [Serializer(typeof(NonDataContractClass), 1)]
            private static void SerializerVersion1(IShapeshifterWriter writer, NonDataContractClass itemToSerialize)
            {
                writer.Write("Value", itemToSerialize.Value);
            }

            [Deserializer(typeof(NonDataContractClass), 1)]
            private static NonDataContractClass DeserializerVersion1(IShapeshifterReader reader)
            {
                var val = reader.Read<string>("Value");
                return new NonDataContractClass() {Value = val};
            }
        }


        private class SerializationForNonDataContractClassVersion2
        {
            [Serializer(typeof(NonDataContractClass), 2)]
            private static void SerializerVersion2(IShapeshifterWriter writer, NonDataContractClass itemToSerialize)
            {
                //switched to a different scheme
                var val = Int32.Parse(itemToSerialize.Value);
                writer.Write("Value", val);
            }

            [Deserializer(typeof(NonDataContractClass), 2)]
            private static NonDataContractClass DeserializerVersion2(IShapeshifterReader reader)
            {
                var val = reader.Read<int>("Value");
                return new NonDataContractClass() { Value = val.ToString() };
            }

            [Deserializer(typeof (NonDataContractClass), 1)]
            private static NonDataContractClass DeserializerVersion1(IShapeshifterReader reader)
            {
                var val = reader.Read<string>("Value");
                return new NonDataContractClass() { Value = val };
            }
        }
        
        private class NonDataContractClass
        {
            public string Value { get; set; }
        }
    }


}
