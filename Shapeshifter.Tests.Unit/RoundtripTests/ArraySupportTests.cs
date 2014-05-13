using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.SqlServer.Server;
using NUnit.Framework;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class ArraySupportTests : TestsBase
    {
        [Test]
        public void IntegerArray_NullArrayTest()
        {
            var machine = GetSerializer<ClassWithNumericArrays>();
            var source = new ClassWithNumericArrays { IntArrayValue = null };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.IntArrayValue.Should().BeNull();
        }

        [Test]
        public void IntegerArray_MultiElementArrayTest()
        {
            var machine = GetSerializer<ClassWithNumericArrays>(); 
            var source = new ClassWithNumericArrays { IntArrayValue = new int[] { 2, 4, 8 } };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.IntArrayValue.Should().Equal(new int[] { 2, 4, 8 });
        }

        [Test]
        public void IntegerArray_SingleElementArrayTest()
        {
            var machine = GetSerializer<ClassWithNumericArrays>();
            var source = new ClassWithNumericArrays { IntArrayValue = new int[] { 42 } };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.IntArrayValue.Should().Equal(new int[] { 42 });
        }

        [Test]
        public void IntegerArray_EmptyArrayTest()
        {
            var machine = GetSerializer<ClassWithNumericArrays>();
            var source = new ClassWithNumericArrays { IntArrayValue = new int[0] };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.IntArrayValue.Should().BeEmpty();
        }

        [Test]
        public void DecimalArray_MultiElementArrayTest()
        {
            var machine = GetSerializer<ClassWithNumericArrays>();
            var source = new ClassWithNumericArrays { DecimalArrayValue = new decimal[] { 2.4m, 8.16m, 32.64m } };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.DecimalArrayValue.Should().Equal(new decimal[] { 2.4m, 8.16m, 32.64m });
        }

        [Test]
        public void StringArray_MultiElementArrayTest()
        {
            var machine = GetSerializer<ClassWithStringArray>();
            var source = new ClassWithStringArray { StringArrayValue = new string[] { "one", "two", "four" } };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.StringArrayValue.Should().Equal(new string[] { "one", "two", "four" });
        }

        [Test]
        public void ObjectArray_MultiElementArrayTest()
        {
            var machine = GetSerializer<ClassWithObjectArray>();
            var personArray = new []
            {
                new Person() {Name = "Jack"},
                new Person() {Name = "Jane"},
            };

            var source = new ClassWithObjectArray { PersonArray = personArray};
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.PersonArray.Should().Equal(personArray, (p1, p2) => p1.Name == p2.Name);
        }

        [Test]
        public void NestedArrayTest()
        {
            var machine = GetSerializer<ClassWithNestedArrays>();
            var input = new[]
            {
                new[] {"one", "two"},
                new[] {"four"},
                new string[0],
                new[] {"eight", "nine", "ten"}
            };

            var source = new ClassWithNestedArrays { NestedStrings = input };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.NestedStrings.Should().Equal(input, StringArrayEquals);
        }

        [DataContract]
        [Serializer]
        private class ClassWithNumericArrays
        {
            [DataMember]
            public int[] IntArrayValue { get; set; }

            [DataMember]
            public decimal[] DecimalArrayValue { get; set; }
        }

        [DataContract]
        [Serializer]
        private class ClassWithStringArray
        {
            [DataMember]
            public string[] StringArrayValue { get; set; }
        }

        [DataContract]
        [Serializer]
        private class ClassWithObjectArray
        {
            [DataMember]
            public Person[] PersonArray { get; set; }
        }

        [DataContract]
        [Serializer]
        private class ClassWithNestedArrays
        {
            [DataMember]
            public string[][] NestedStrings { get; set; }
        }
    }
}
