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
    public class GuidSupportTests : TestsBase
    {
        [Test]
        public void StringBasedGuidTest()
        {
            var guid = Guid.NewGuid();
            var machine = GetSerializer<ClassWithGuid>();
            var source = new ClassWithGuid { GuidValue = guid };
            string packed = machine.Serialize(source);

            Debug.Print(packed);

            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.GuidValue.Should().Be(guid);
        }


        [DataContract]
        [Serializer]
        private class ClassWithGuid
        {
            [DataMember]
            public Guid GuidValue { get; set; }
        }
    }

}
