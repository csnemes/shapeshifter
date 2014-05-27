using FluentAssertions;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class GuidSupportTests : TestsBase
    {
        [Test]
        public void StringBasedGuidTest()
        {
            var guid = Guid.NewGuid();
            var serializer = GetSerializer<ClassWithGuid>();
            var source = new ClassWithGuid { GuidValue = guid };
            string packed = serializer.Serialize(source);

            Debug.Print(packed);

            var unpacked = serializer.Deserialize(packed);
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
