using FluentAssertions;
using NUnit.Framework;
using System;
using System.Runtime.Serialization;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class NullableSupportTests : TestsBase
    {
        [Test]
        public void NullableNumericsContainingNullTest()
        {
            var machine = GetSerializer<ClassWithNullableNumerics>();
            var source = new ClassWithNullableNumerics { IntegerValue = null, DoubleValue = null };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.IntegerValue.HasValue.Should().BeFalse();
            unpacked.DoubleValue.HasValue.Should().BeFalse();
        }

        [Test]
        public void NullableDateTimeContainingNullTest()
        {
            var machine = GetSerializer<ClassWithNullableDateTime>(); 
            var source = new ClassWithNullableDateTime { DateTimeValue = null };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.DateTimeValue.HasValue.Should().BeFalse();
        }

        [Test]
        public void NullableBoolContainingNullTest()
        {
            var machine = GetSerializer<ClassWithNullableBool>(); 
            var source = new ClassWithNullableBool { BoolValue = null };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.BoolValue.HasValue.Should().BeFalse();
        }

        [Test]
        public void NullableNumericsContainingValueTest()
        {
            var machine = GetSerializer<ClassWithNullableNumerics>(); 
            var source = new ClassWithNullableNumerics { IntegerValue = int.MaxValue, DoubleValue = double.MaxValue };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.IntegerValue.HasValue.Should().BeTrue();
            unpacked.IntegerValue.Value.Should().Be(int.MaxValue);
            unpacked.DoubleValue.HasValue.Should().BeTrue();
            unpacked.DoubleValue.Value.Should().Be(double.MaxValue);
        }

        [Test]
        public void NullableDateTimeContainingValueTest()
        {
            var now = DateTime.Now;
            var machine = GetSerializer<ClassWithNullableDateTime>(); 
            var source = new ClassWithNullableDateTime { DateTimeValue = now };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.DateTimeValue.HasValue.Should().BeTrue();
            unpacked.DateTimeValue.Value.Should().Be(now);
        }

        [Test]
        public void NullableBoolContainingValueTest()
        {
            var machine = GetSerializer<ClassWithNullableBool>(); 
            var source = new ClassWithNullableBool { BoolValue = true };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.BoolValue.HasValue.Should().BeTrue();
            unpacked.BoolValue.Value.Should().Be(true);
        }

        [DataContract]
        [ShapeshifterRoot]
        public class ClassWithNullableNumerics
        {
            [DataMember]
            public int? IntegerValue { get; set; }

            [DataMember]
            public double? DoubleValue { get; set; }     
        }

        [DataContract]
        [ShapeshifterRoot]
        public class ClassWithNullableDateTime
        {
            [DataMember]
            public DateTime? DateTimeValue { get; set; }
        }

        [DataContract]
        [ShapeshifterRoot]
        public class ClassWithNullableBool
        {
            [DataMember]
            public bool? BoolValue { get; set; }
        }
    }
}
