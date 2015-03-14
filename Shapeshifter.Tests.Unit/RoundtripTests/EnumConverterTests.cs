using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class EnumConverterTests
    {
        [Test]
        public void TestEnumConverter()
        {
            var serializer = new ShapeshifterSerializer<MyClassWithEnum>(); //EnumConverter is set implicitly
            var input = new MyClassWithEnum()
            {
                EnumVal = MyEnum.Second
            };

            var wireFormat = serializer.Serialize(input);
            var result = serializer.Deserialize(wireFormat);

            result.EnumVal.Should().Be(MyEnum.Second);
        }

        [Test]
        public void TestEnumConverterIfEnumHasDataContractAttribute()
        {
            var serializer = new ShapeshifterSerializer<MyClassWithEnumAttributed>(); //EnumConverter is set implicitly
            var input = new MyClassWithEnumAttributed()
            {
                EnumVal = MyEnumAttributed.Second
            };

            var wireFormat = serializer.Serialize(input);
            var result = serializer.Deserialize(wireFormat);

            result.EnumVal.Should().Be(MyEnumAttributed.Second);
        }

        [DataContract]
        [ShapeshifterRoot]
        private class MyClassWithEnum
        {
            [DataMember]
            public MyEnum EnumVal { get; set; }
        }

        private enum MyEnum
        {
            First,
            Second
        }

        [DataContract]
        [ShapeshifterRoot]
        private class MyClassWithEnumAttributed
        {
            [DataMember]
            public MyEnumAttributed EnumVal { get; set; }
        }

        [DataContract]
        private enum MyEnumAttributed
        {
            First,
            Second
        }
    }
}
