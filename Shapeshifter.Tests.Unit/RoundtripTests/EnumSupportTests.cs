using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
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
        [Serializer]
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
