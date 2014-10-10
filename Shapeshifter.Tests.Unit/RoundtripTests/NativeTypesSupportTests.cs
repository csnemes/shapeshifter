using FluentAssertions;
using NUnit.Framework;
using System;
using System.Runtime.Serialization;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class NativeTypesSupportTests : TestsBase
    {
        [Test]
        public void BoolTest()
        {
            var machine = GetSerializer<ClassWithNativeBool>();
            var source = new ClassWithNativeBool {BoolValue = true};
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.BoolValue.Should().BeTrue();
        }

        [Test]
        public void BytesTest()
        {
            var machine = GetSerializer<ClassWithNativeBytes>();
            var source = new ClassWithNativeBytes
            {
                CharValue = char.MaxValue,
                ByteValue = byte.MaxValue,
                ShortByteValue = sbyte.MaxValue
            };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.CharValue.Should().Be(char.MaxValue);
            unpacked.ByteValue.Should().Be(byte.MaxValue);
            unpacked.ShortByteValue.Should().Be(sbyte.MaxValue);
        }

        [Test]
        public void DateTest()
        {
            DateTime now = DateTime.Now;
            var offset = new DateTimeOffset(now);
            var machine = GetSerializer<ClassWithDateTypes>();
            var source = new ClassWithDateTypes {DateTimeValue = now, DateTimeOffsetValue = offset};
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.DateTimeValue.Should().Be(now);
            unpacked.DateTimeOffsetValue.Should().Be(offset);
        }

        [Test]
        public void FloatTest()
        {
            var machine = GetSerializer<ClassWithNativeFloats>();
            var source = new ClassWithNativeFloats
            {
                FloatValue = float.MaxValue,
                DoubleValue = double.MaxValue,
                DecimalValue = 10000.4356m
            };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.FloatValue.Should().Be(float.MaxValue);
            unpacked.DoubleValue.Should().Be(double.MaxValue);
            unpacked.DecimalValue.Should().Be(10000.4356m);
        }

        [Test]
        public void IntegerTest()
        {
            var machine = GetSerializer<ClassWithNativeIntegers>();
            var source = new ClassWithNativeIntegers
            {
                IntegerValue = Int32.MaxValue,
                LongValue = Int64.MaxValue,
                ShortValue = Int16.MaxValue
            };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.IntegerValue.Should().Be(Int32.MaxValue);
            unpacked.LongValue.Should().Be(Int64.MaxValue);
            unpacked.ShortValue.Should().Be(Int16.MaxValue);
        }

        [Test]
        public void NullTest()
        {
            var machine = GetSerializer<ClassWithNativeString>();
            var source = new ClassWithNativeString {StringValue = null};
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.StringValue.Should().BeNull();
        }

        [Test]
        public void StringTest()
        {
            var machine = GetSerializer<ClassWithNativeString>();
            var source = new ClassWithNativeString {StringValue = "Hello"};
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.StringValue.Should().Be("Hello");
        }

        [Test]
        public void UnsignedIntegerTest()
        {
            var machine = GetSerializer<ClassWithNativeUnsignedIntegers>();
            var source = new ClassWithNativeUnsignedIntegers
            {
                IntegerValue = UInt32.MaxValue,
                LongValue = UInt64.MaxValue,
                ShortValue = UInt16.MaxValue
            };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.IntegerValue.Should().Be(UInt32.MaxValue);
            unpacked.LongValue.Should().Be(UInt64.MaxValue);
            unpacked.ShortValue.Should().Be(UInt16.MaxValue);
        }


        [DataContract]
        [ShapeshifterRoot]
        public class ClassWithNativeString
        {
            [DataMember]
            public string StringValue { get; set; }
        }

        [DataContract]
        [ShapeshifterRoot]
        public class ClassWithNativeBool
        {
            [DataMember]
            public bool BoolValue { get; set; }
        }

        [DataContract]
        [ShapeshifterRoot]
        public class ClassWithNativeIntegers
        {
            [DataMember]
            public int IntegerValue { get; set; }

            [DataMember]
            public long LongValue { get; set; }

            [DataMember]
            public short ShortValue { get; set; }
        }

        [DataContract]
        [ShapeshifterRoot]
        public class ClassWithNativeUnsignedIntegers
        {
            [DataMember]
            public uint IntegerValue { get; set; }

            [DataMember]
            public ulong LongValue { get; set; }

            [DataMember]
            public ushort ShortValue { get; set; }
        }

        [DataContract]
        [ShapeshifterRoot]
        public class ClassWithNativeFloats
        {
            [DataMember]
            public double DoubleValue { get; set; }

            [DataMember]
            public float FloatValue { get; set; }

            [DataMember]
            public decimal DecimalValue { get; set; }
        }

        [DataContract]
        [ShapeshifterRoot]
        public class ClassWithNativeBytes
        {
            [DataMember]
            public char CharValue { get; set; }

            [DataMember]
            public byte ByteValue { get; set; }

            [DataMember]
            public sbyte ShortByteValue { get; set; }
        }

        [DataContract]
        [ShapeshifterRoot]
        public class ClassWithDateTypes
        {
            [DataMember]
            public DateTime DateTimeValue { get; set; }

            [DataMember]
            public DateTimeOffset DateTimeOffsetValue { get; set; }
        }
    }
}