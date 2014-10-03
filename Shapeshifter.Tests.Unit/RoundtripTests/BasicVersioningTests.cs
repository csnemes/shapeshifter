using System.Security.Cryptography;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter;
using Shapeshifter.Core.Detection;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Version1;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class BasicVersioningTests : TestsBase
    {
        [Test]
        public void ReadOldVersionIntoNewVersion()
        {
            var input = new MyClassVersionOne() {Id = "42"};
            var serializer = GetSerializer<MyClassVersionOne>();
            var deserializer = GetSerializer<MyClassVersionTwo>();
            var packed = serializer.Serialize(input);

            var result = deserializer.Deserialize(packed);

            result.Id.Should().Be(42);
        }

        [Test]
        public void HandleMultipleVersions()
        {
            var inputOne = new MyClassVersionOne() { Id = "42" };
            var inputTwo = new MyClassVersionTwo() { Id = 100 };

            var serializerOne = GetSerializer<MyClassVersionOne>();
            var serializerTwo = GetSerializer<MyClassVersionTwo>();
            var deserializer = GetSerializer<MyClassVersionThree>();
            var packedOne = serializerOne.Serialize(inputOne);
            var packedTwo = serializerTwo.Serialize(inputTwo);

            var resultOne = deserializer.Deserialize(packedOne);
            var resultTwo = deserializer.Deserialize(packedTwo);

            resultOne.UserId.Should().Be("42");
            resultTwo.UserId.Should().Be("100");
        }

        [Test]
        public void DetectTheVersionNumberHelper()
        {
            var ti = new TypeInspector(typeof(Version1.TestEnum));
            Debug.Print(ti.Version.ToString());
        }

        [Test]
        public void DeserializeDifferentEnumVersion()
        {
            var inp =
                @"{""__typeName"":""TestClass"",""__version"":3211725878,""Enum"":{""__typeName"":""TestEnum"",""__version"":1060625644,""__enumValue"":""Two""}}";

            var serializer = GetSerializer<Version1.TestClass>();

            var result = serializer.Deserialize(inp);

            result.Enum.Should().Be(TestEnum.One);
        }
    }
    
    [DataContract]
    [Shapeshifter(1)]
    public class MyClassVersionOne
    {
        [DataMember]
        public string Id { get; set; }
    }

    [DataContract]
    [Shapeshifter(2)]
    public class MyClassVersionTwo //example for switching property type
    {
        [DataMember]
        public int Id { get; set; }

        [Deserializer("MyClassVersionOne", 1)]
        private static object TransformVersionOne(IShapeshifterReader reader)
        {
            var oldFormatValue = reader.Read<string>("Id");
            var newFormatValue = Int32.Parse(oldFormatValue);
            return new MyClassVersionTwo() {Id = newFormatValue};
        }
    }

    [DataContract]
    [Shapeshifter(3)]
    public class MyClassVersionThree //example for switching property name
    {
        [DataMember]
        public string UserId { get; set; }

        [Deserializer("MyClassVersionOne", 1)]
        private static object TransformVersionOne(IShapeshifterReader reader)
        {
            var value = reader.Read<string>("Id");
            return new MyClassVersionThree() { UserId = value };
        }

        [Deserializer("MyClassVersionTwo", 2)]
        private static object TransformVersionTwo(IShapeshifterReader reader)
        {
            var value = reader.Read<int>("Id");
            return new MyClassVersionThree() { UserId = value.ToString() };
        }
    }


}

namespace Version1
{
    [Shapeshifter()]
    [DataContract]
    public class TestClass
    {
        [DataMember]
        public TestEnum Enum { get; set; }

        [Deserializer("TestEnum", 1060625644)]
        public static object DeserializerForOldVersion(IShapeshifterReader reader)
        {
            var val = reader.Read<string>("__enumValue");
            TestEnum result;
            if (TestEnum.TryParse(val, out result))
            {
                return TestEnum.One;
            }
            return result;
        }
    }

    [DataContract]
    public enum TestEnum
    {
        [EnumMember]
        One
    }
}


