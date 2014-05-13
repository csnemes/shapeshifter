using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class GenericSupportTests:TestsBase
    {
        [Test]
        public void SameArgumentTypeWriteAndReadBack_ShouldWork()
        {
            var serializer = GetSerializer<Generic<int>>();
            var wireFormat = serializer.Serialize(new Generic<int> { Value = 42 });
            var result = serializer.Deserialize(wireFormat);
            result.Value.Should().Be(42);
        }

        [Test]
        public void SameArgumentTypeWriteAndReadBack_SamePackformatSignature_ShouldWork()
        {
            var serializer = GetSerializer<GenericNameDiffTest<int>>();
            var wireFormat = serializer.Serialize(new GenericNameDiffTest<int> {Value = 42});
            var result = serializer.Deserialize(wireFormat);
            result.Value.Should().Be(42);
        }

        [Test]
        public void DifferentArgumentTypeWriteAndReadBack_SamePackformatSignature_ShouldNotWork()
        {
            var serializer = GetSerializer<GenericNameDiffTest<int>>();
            var deserializer = GetSerializer<GenericNameDiffTest<string>>(); 
            var wireFormat = serializer.Serialize(new GenericNameDiffTest<int> { Value = 42 });
            Action action = () => deserializer.Deserialize(wireFormat);
            action.ShouldThrow<ArgumentException>();
        }


        [DataContract]
        [Serializer]
        private class GenericNameDiffTest<T>
        {
            [DataMember]
            public int Value { get; set; }
        }

        [DataContract]
        [Serializer]
        private class Generic<T>
        {
            [DataMember]
            public T Value { get; set; }
        }
    }
}
