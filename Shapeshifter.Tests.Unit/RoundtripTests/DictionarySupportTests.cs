using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    [Ignore("Nincs implementálva a KeyValuePair kezelés.")]
    public class DictionarySupportTests : TestsBase
    {

        [Test]
        public void Dictionary_NullTest()
        {
            var machine = GetSerializer<ClassWithPrimitiveDictionary>();
            var source = new ClassWithPrimitiveDictionary { Dictionary = null };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.Dictionary.Should().BeNull();
        }

        [Test]
        public void Dictionary_MultiElementTest()
        {
            var machine = GetSerializer<ClassWithPrimitiveDictionary>();
            var dict = new Dictionary<int, string>() {{1, "One"}, {2, "Two"}};
            var source = new ClassWithPrimitiveDictionary { Dictionary = dict};
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.Dictionary.Should().Equal(dict);
        }

        [Test]
        public void ObjectDictionary_MultiElementTest()
        {
            var machine = GetSerializer<ClassWithObjectDictionary>();
            var dict = new Dictionary<KeyClass, ValueClass>() { { new KeyClass() {Value = 1}, new ValueClass() {Value = "One"}},
            {new KeyClass() {Value = 2}, new ValueClass() {Value = "Two"}} };
            var source = new ClassWithObjectDictionary { Dictionary = dict };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.Dictionary.Should().Equal(dict);
        }

        [DataContract]
        [Shapeshifter]
        private class ClassWithPrimitiveDictionary
        {
            [DataMember]
            public Dictionary<int, string> Dictionary { get; set; } 
        }

        [DataContract]
        [Shapeshifter]
        private class ClassWithObjectDictionary
        {
            [DataMember]
            public Dictionary<KeyClass, ValueClass> Dictionary { get; set; }
        }

        [DataContract]
        private class KeyClass
        {
            [DataMember]
            public int Value { get; set; }

            protected bool Equals(KeyClass other)
            {
                return Value == other.Value;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((KeyClass) obj);
            }

            public override int GetHashCode()
            {
                return Value;
            }

            public static bool operator ==(KeyClass left, KeyClass right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(KeyClass left, KeyClass right)
            {
                return !Equals(left, right);
            }
        }

        [DataContract]
        private class ValueClass
        {
            [DataMember]
            public string Value { get; set; }

            protected bool Equals(ValueClass other)
            {
                return string.Equals(Value, other.Value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ValueClass) obj);
            }

            public override int GetHashCode()
            {
                return (Value != null ? Value.GetHashCode() : 0);
            }

            public static bool operator ==(ValueClass left, ValueClass right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(ValueClass left, ValueClass right)
            {
                return !Equals(left, right);
            }
        }
    }
}
