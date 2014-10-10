using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class BuiltInTypesAsObjectsTests : TestsBase
    {

        [Test]
        public void NumericShortTest()
        {
            var serializer = GetSerializer<DataHolder>();
            var input = new DataHolder() {Value = (short) 42};

            var packed = serializer.Serialize(input);
            Debug.Print(packed);

            var unpacked = serializer.Deserialize(packed);

            unpacked.Value.Should().BeOfType<short>().And.Be((short)42);
        }

        [Test]
        public void NumericLongTest()
        {
            var serializer = GetSerializer<DataHolder>();
            var input = new DataHolder() { Value = (long)42 };

            var packed = serializer.Serialize(input);
            Debug.Print(packed);

            var unpacked = serializer.Deserialize(packed);

            unpacked.Value.Should().BeOfType<long>().And.Be((long)42);
        }

        [Test]
        public void StringTest()
        {
            var serializer = GetSerializer<DataHolder>();
            var input = new DataHolder() { Value = "Hello" };

            var packed = serializer.Serialize(input);
            Debug.Print(packed);

            var unpacked = serializer.Deserialize(packed);

            unpacked.Value.Should().BeOfType<string>().And.Be("Hello");
        }

        [Test]
        public void DateTimeTest()
        {
            var serializer = GetSerializer<DataHolder>();
            var dateTime = DateTime.Now;
            var input = new DataHolder() { Value = dateTime };

            var packed = serializer.Serialize(input);
            Debug.Print(packed);

            var unpacked = serializer.Deserialize(packed);

            unpacked.Value.Should().BeOfType<DateTime>().And.Be(dateTime);
        }

        [Test]
        public void GuidTest()
        {
            var serializer = GetSerializer<DataHolder>();
            var guid = Guid.NewGuid();
            var input = new DataHolder() { Value = guid };

            var packed = serializer.Serialize(input);
            Debug.Print(packed);

            var unpacked = serializer.Deserialize(packed);

            unpacked.Value.Should().BeOfType<Guid>().And.Be(guid);
        }

        [ShapeshifterRoot]
        [DataContract]
        private class DataHolder
        {
            [DataMember]
            public object Value { get; set; }
        }
    }
}
