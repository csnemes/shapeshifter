using FluentAssertions;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    [Ignore("Nincs implementálva a Guid support")]
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
        [Shapeshifter]
        private class ClassWithGuid
        {
            [DataMember]
            public Guid GuidValue { get; set; }
        }
    }

}
