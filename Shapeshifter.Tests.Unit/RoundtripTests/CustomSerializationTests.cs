using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class CustomSerializationTests
    {
        [Test]
        public void SameVersionWriteAndReadBack_OriginalVersion()
        {
            var serializer = new ShapeshifterSerializer<NonDataContractClass>(new[] { typeof(SerializationForNonDataContractClassVersion1) });
            var serialized = serializer.Serialize(new NonDataContractClass() { Value = "42"});
            var result = serializer.Deserialize(serialized);
            result.Value.Should().Be("42");
        }

        [Test]
        public void SameVersionWriteAndReadBack_NewVersion()
        {
            var serializer = new ShapeshifterSerializer<NonDataContractClass>(new[] { typeof(SerializationForNonDataContractClassVersion2) });
            var serialized = serializer.Serialize(new NonDataContractClass() { Value = "42" });
            var result = serializer.Deserialize(serialized);
            result.Value.Should().Be("42");
        }

        [Test]
        public void OldVersionReadBack_NewVersion()
        {
            var oldSerializer = new ShapeshifterSerializer<NonDataContractClass>(new[] { typeof(SerializationForNonDataContractClassVersion1) });
            var serialized = oldSerializer.Serialize(new NonDataContractClass() { Value = "42" });

            var serializer = new ShapeshifterSerializer<NonDataContractClass>(new[] { typeof(SerializationForNonDataContractClassVersion2) });
            var result = serializer.Deserialize(serialized);
            result.Value.Should().Be("42");
        }

        private class SerializationForNonDataContractClassVersion1
        {
            [Serializer(typeof(NonDataContractClass), 1)]
            private static void SerializerVersion1(IPackformatValueWriter writer, NonDataContractClass itemToSerialize)
            {
                writer.SetValue("Value", itemToSerialize.Value);
            }

            [Deserializer(typeof(NonDataContractClass), 1)]
            private static NonDataContractClass DeserializerVersion1(IPackformatValueReader reader)
            {
                var val = reader.GetValue<string>("Value");
                return new NonDataContractClass() {Value = val};
            }
        }


        private class SerializationForNonDataContractClassVersion2
        {
            [Serializer(typeof(NonDataContractClass), 2)]
            private static void SerializerVersion2(IPackformatValueWriter writer, NonDataContractClass itemToSerialize)
            {
                //switched to a different scheme
                var val = Int32.Parse(itemToSerialize.Value);
                writer.SetValue("Value", val);
            }

            [Deserializer(typeof(NonDataContractClass), 2)]
            private static NonDataContractClass DeserializerVersion2(IPackformatValueReader reader)
            {
                var val = reader.GetValue<int>("Value");
                return new NonDataContractClass() { Value = val.ToString() };
            }

            [Deserializer(typeof (NonDataContractClass), 1)]
            private static NonDataContractClass DeserializerVersion1(IPackformatValueReader reader)
            {
                var val = reader.GetValue<string>("Value");
                return new NonDataContractClass() { Value = val };
            }
        }
        
        private class NonDataContractClass
        {
            public string Value { get; set; }
        }
    }


}
