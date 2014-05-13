using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class ListSupportTests : TestsBase
    {
        [Test]
        public void IntegerList_NullTest()
        {
            var machine = GetSerializer<ClassWithNumericLists>();
            var source = new ClassWithNumericLists { IntegerList = null };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.IntegerList.Should().BeNull();
        }

        [Test]
        public void IntegerList_MultiElementTest()
        {
            var machine = GetSerializer<ClassWithNumericLists>();
            var source = new ClassWithNumericLists { IntegerList = new List<int>() { 2, 4, 8 } };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.IntegerList.Should().Equal(new List<int>() { 2, 4, 8 });
        }

        [Test]
        public void IntegerList_EmptyListTest()
        {
            var machine = GetSerializer<ClassWithNumericLists>();
            var source = new ClassWithNumericLists { IntegerList = new List<int>() };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.IntegerList.Should().Equal(new List<int>());
        }

        [Test]
        public void ObjectList_MultiElementListTest()
        {
            var machine = GetSerializer<ClassWithObjectList>();
            var personList = new List<Person>()
            {
                new Person() {Name = "Jack"},
                new Person() {Name = "Jane"},
            };

            var source = new ClassWithObjectList { PersonList = personList };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.PersonList.Should().Equal(personList, (p1, p2) => p1.Name == p2.Name);
        }

        [DataContract]
        [Serializer]
        private class ClassWithNumericLists
        {
            [DataMember]
            public List<int> IntegerList { get; set; }
        }

        [DataContract]
        [Serializer]
        private class ClassWithObjectList
        {
            [DataMember]
            public List<Person> PersonList { get; set; }
        }
    }
}
