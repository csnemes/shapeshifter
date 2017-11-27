using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class HashSetTests:TestsBase
    {


        [Test]
        public void HashSet_SingleElementTest()
        {
            var machine = GetSerializer<ClassWithHashSet>();
            var set = new HashSet<string>();
            set.Add("Hello");
            var source = new ClassWithHashSet { HashSet = set };
            string packed = machine.Serialize(source);
            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.HashSet.Should().Equal(set);
        }

        [DataContract]
        [ShapeshifterRoot]
        private class ClassWithHashSet
        {
            [DataMember]
            public HashSet<string> HashSet { get; set; }
        }
    }

}
