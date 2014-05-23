using FluentAssertions;
using NUnit.Framework;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    [Ignore("Nincs implementálva az Enum támogatás")]
    public class EnumSupportTests : TestsBase
    {
        [Test]
        public void StringBasedEnumTest()
        {
            var machine = GetSerializer<ClassWithEnum>();
            var source = new ClassWithEnum { StringEnumValue = MyStringEnum.First };
            string packed = machine.Serialize(source);

            Debug.Print(packed);

            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.StringEnumValue.Should().Be(MyStringEnum.First);
        }


        [DataContract]
        [Shapeshifter]
        private class ClassWithEnum
        {
            [DataMember]
            public MyStringEnum StringEnumValue { get; set; }
        }

        private enum MyStringEnum
        {
            First,
            Second,
            Third
        }

    }


}
