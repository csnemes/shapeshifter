using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class IEnumerableImplementingClassTests : TestsBase
    {

        [Test]
        public void NullTest()
        {
            var machine = GetSerializer<ClassWithInnerClass>();
            var source = new ClassWithInnerClass { InnerItems = null };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.InnerItems.Should().BeNull();
        }

        [Test]
        public void EmptyTest()
        {
            var machine = GetSerializer<ClassWithInnerClass>();
            var source = new ClassWithInnerClass { InnerItems = new List<InnerClass>() };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.InnerItems.Should().BeEmpty();
        }

        [Test]
        public void SingleItemTest()
        {
            var machine = GetSerializer<ClassWithInnerClass>();
            var source = new ClassWithInnerClass { InnerItems = new List<InnerClass>() { new InnerClass() { Items = new List<string>() { "Item1" }, OtherItem = "OtherItem"} } };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();

            unpacked.InnerItems.Count.Should().Be(1);
            var inner = unpacked.InnerItems[0];
            inner.OtherItem.Should().Be("OtherItem");
            inner.Items.Count.Should().Be(1);
        }

        [DataContract]
        [Shapeshifter]
        private class ClassWithInnerClass
        {
            [DataMember]
            public List<InnerClass> InnerItems { get; set; }
        }


        [DataContract]
        private class InnerClass : IEnumerable<string>
        {
            [DataMember]
            public string OtherItem { get; set; }

            [DataMember]
            public List<string> Items { get; set; }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerator<string> GetEnumerator()
            {
                return Items.GetEnumerator();
            }
        }
    }
}
