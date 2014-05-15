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
    public class NestingPackformatConvertersTests : TestsBase
    {
        [Test]
        public void TestSingleNesting()
        {
            var guid = Guid.NewGuid();
            var machine = GetSerializer<MyValue>(new MyValueSurrogateConverter());
            var source = new MyValue(guid);

            string packed = machine.Serialize(source);

            Debug.Print(packed);

            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.Guid.Should().Be(guid);
        }

        [Test]
        public void TestNestingNull()
        {
            MyValue inp = null;
            var machine = GetSerializer<MyOuterValue>(new MyValueSurrogateConverter(), new MyOuterValueSurrogateConverter());
            var source = new MyOuterValue() { InnerValue = inp};

            string packed = machine.Serialize(source);

            Debug.Print(packed);

            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.InnerValue.Should().Be(inp);
        }

        [Test]
        public void TestDeepNesting()
        {
            var guid = Guid.NewGuid();
            var inp = new MyValue(guid);
            var machine = GetSerializer<MyOuterValue>(new MyValueSurrogateConverter(), new MyOuterValueSurrogateConverter());
            var source = new MyOuterValue() { InnerValue = inp };

            string packed = machine.Serialize(source);

            Debug.Print(packed);

            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.InnerValue.Guid.Should().Be(guid);
        }

        [KnownType(typeof(MyOuterValueSurrogate))]
        private class MyOuterValueSurrogateConverter : IPackformatSurrogateConverter
        {
            public object ConvertToSurrogate(object value)
            {
                var typedValue = value as MyOuterValue;
                return new MyOuterValueSurrogate() {Value = typedValue.InnerValue };
            }

            public object ConvertFromSurrogate(Type targetType, object value)
            {
                if (targetType != typeof(MyOuterValue)) throw new ArgumentException("Wrong target type");
                var typedValue = (MyOuterValueSurrogate)value;
                return new MyOuterValue() { InnerValue = typedValue.Value };
            }

            public bool CanConvert(Type type)
            {
                return (type == typeof(MyOuterValue));
            }
        }

        [KnownType(typeof(MyValueSurrogate))]
        private class MyValueSurrogateConverter : IPackformatSurrogateConverter
        {
            public object ConvertToSurrogate(object value)
            {
                var typedValue = value as MyValue;
                return new MyValueSurrogate() {Value = typedValue.Guid};
            }

            public object ConvertFromSurrogate(Type targetType, object value)
            {
                if (targetType != typeof(MyValue)) throw new ArgumentException("Wrong target type");
                if (value == null) return null;
                var typedValue = (MyValueSurrogate)value;
                return new MyValue(typedValue.Value);
            }

            public bool CanConvert(Type type)
            {
                return (type == typeof (MyValue));
            }
        }

        [DataContract]
        private class MyOuterValueSurrogate
        {
            [DataMember]
            public MyValue Value { get; set; }
        }

        private class MyOuterValue
        {
            public MyValue InnerValue { get; set; }
        }

        [DataContract]
        private class MyValueSurrogate
        {
            [DataMember]
            public Guid Value { get; set; }
        }

        private class MyValue
        {
            private readonly Guid _guid;

            public MyValue(Guid guid)
            {
                _guid = guid;
            }

            public Guid Guid
            {
                get { return _guid; }
            }
        }
    }
}
