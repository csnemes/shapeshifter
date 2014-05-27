using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Shapeshifter.Core;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class EnumSupportTests : TestsBase
    {
        [Test]
        public void StringBasedEnumTest()
        {
            var machine = GetSerializer<ClassWithEnum>();
            var source = new ClassWithEnum { EnumValue = MyEnum.First };
            string packed = machine.Serialize(source);

            Debug.Print(packed);

            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.EnumValue.Should().Be(MyEnum.First);
        }

        [Test]
        public void BuiltinEnumConverter_CanBeOverriddenForConcreteEnumType()
        {
            var machine = new Shapeshifter<ClassWithEnum>(new[] {typeof (MyEnumConverter)}, new[] {Assembly.GetExecutingAssembly()});
            var source = new ClassWithEnum { EnumValue = MyEnum.First };
            string packed = machine.Serialize(source);

            Debug.Print(packed);

            var jobj = JObject.Parse(packed).GetValue("EnumValue");
            jobj[Constants.TypeNameKey].Value<string>().Should().Be("MyEnumPackname");
            jobj[Constants.VersionKey].Value<uint>().Should().Be(1);
            jobj["MyKey"].Value<int>().Should().Be((int)MyEnum.First);

            var unpacked = machine.Deserialize(packed);
            unpacked.Should().NotBeNull();
            unpacked.EnumValue.Should().Be(MyEnum.First);
        }

        [DataContract]
        [Shapeshifter]
        private class ClassWithEnum
        {
            [DataMember]
            public MyEnum EnumValue { get; set; }
        }

        [Shapeshifter(1)]
        private enum MyEnum
        {
            First,
            Second
        }

        private static class MyEnumConverter
        {
            [Serializer(typeof (MyEnum), "MyEnumPackname", 1)]
            public static void Serialize(IShapeshifterWriter writer, MyEnum myEnum)
            {
                writer.Write("MyKey", (int)myEnum);
            }

            [Deserializer("MyEnumPackname")]
            public static MyEnum Deserialize(IShapeshifterReader reader)
            {
                var enumValueAsInt = reader.Read<int>("MyKey");
                return (MyEnum) enumValueAsInt;
            }
        }
    }
}
