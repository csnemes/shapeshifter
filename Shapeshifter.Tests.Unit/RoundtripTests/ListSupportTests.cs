using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.Serialization;

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

        [Test]
        public void DirectIntegerList_MultiElementTest()
        {
            var machine = GetSerializer<List<int>>();
            var source = new List<int>() { 2, 4, 8 };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.Should().Equal(new List<int>() { 2, 4, 8 });
        }


        [Test]
        public void ReproducedLiveIssueWithListsWithinDictionary()
        {
            var machine = GetSerializer<DictionaryIssueTestClass>();
            var source = new DictionaryIssueTestClass();
            source.Dict.Add("key", "stringValue");
            source.Dict.Add("key2", new List<int>() { 2, 4, 8 });
            source.Dict.Add("key3", new List<Person>() { new Person() { Name = "Joe" }, new Person() { Name = "Jim"} });

            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
        }

        [DataContract]
        [ShapeshifterRoot]
        private class DictionaryIssueTestClass
        {
            public DictionaryIssueTestClass()
            {
                Dict = new Dictionary<string, object>();
            }

            public Dictionary<string, object> Dict { get; set; }             
        }

        [DataContract]
        [ShapeshifterRoot]
        private class ClassWithNumericLists
        {
            [DataMember]
            public List<int> IntegerList { get; set; }
        }

        [DataContract]
        [ShapeshifterRoot]
        private class ClassWithObjectList
        {
            [DataMember]
            public List<Person> PersonList { get; set; }
        }
    }
}
