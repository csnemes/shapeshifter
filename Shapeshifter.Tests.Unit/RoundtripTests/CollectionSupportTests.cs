using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class CollectionSupportTests : TestsBase
    {
        [Test]
        public void IntegerCollection_NullTest()
        {
            var machine = GetSerializer<ClassWithNumericCollections>();
            var source = new ClassWithNumericCollections { IntegerCollection = null };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.IntegerCollection.Should().BeNull();
        }

        [Test]
        public void IntegerCollection_MultiElementTest()
        {
            var machine = GetSerializer<ClassWithNumericCollections>();
            var source = new ClassWithNumericCollections { IntegerCollection = new Collection<int>() { 2, 4, 8 } };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.IntegerCollection.Should().Equal(new List<int>() { 2, 4, 8 });
        }

        [Test]
        public void IntegerCollection_EmptyCollectionTest()
        {
            var machine = GetSerializer<ClassWithNumericCollections>();
            var source = new ClassWithNumericCollections { IntegerCollection = new Collection<int>() };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.IntegerCollection.Should().Equal(new List<int>());
        }

        [Test]
        public void ObjectCollection_MultiElementTest()
        {
            var machine = GetSerializer<ClassWithObjectCollections>();
            var personList = new Collection<Person>()
            {
                new Person() {Name = "Jack"},
                new Person() {Name = "Jane"},
            };

            var source = new ClassWithObjectCollections { PersonCollection = personList };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.PersonCollection.Should().Equal(personList, (p1, p2) => p1.Name == p2.Name);
        }

        [DataContract]
        [ShapeshifterRoot]
        private class ClassWithNumericCollections
        {
            [DataMember]
            public Collection<int> IntegerCollection { get; set; }
        }

        [DataContract]
        [ShapeshifterRoot]
        private class ClassWithObjectCollections
        {
            [DataMember]
            public Collection<Person> PersonCollection { get; set; }
        }
    }
}
