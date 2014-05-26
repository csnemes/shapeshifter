using System;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Shapeshifter.Core;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class CustomSerializerTests : TestsBase
    {
        [Test]
        public void CustomSerializerAndDeserializer_Success()
        {
            var source = new MyType() { MyProperty = 42 };

            var serializer = GetSerializer<MyType>();
            var pack = serializer.Serialize(source);
            var jobj = JObject.Parse(pack);

            jobj[Constants.TypeNameKey].Value<string>().Should().Be("MyType");
            jobj[Constants.VersionKey].Value<uint>().Should().Be(1);
            jobj["MyKey"].Value<int>().Should().Be(42);

            var target = serializer.Deserialize(pack);
            target.MyProperty.Should().Be(42);
        }

        [Shapeshifter]
        public class MyType
        {
            public int MyProperty { get; set; }

            [Serializer(typeof(MyType), 1)]
            public static void Serializer(IShapeshifterWriter writer, MyType itemToSerialize)
            {
                writer.Write("MyKey", itemToSerialize.MyProperty);
            }

            [Deserializer("MyType", 1)]
            public static object Deserializer(IShapeshifterReader reader)
            {
                return new MyType() {MyProperty = reader.Read<int>("MyKey")};
            }
        }

        [Test]
        [Ignore("Ez bug vagy feature?")]
        public void CustomSerializerWithCustomPackformatName_Success()
        {
            var source = new MyTypeWithCustomPackName() { MyProperty = 42 };

            var serializer = GetSerializer<MyTypeWithCustomPackName>();
            var pack = serializer.Serialize(source);
            var jobj = JObject.Parse(pack);

            jobj[Constants.TypeNameKey].Value<string>().Should().Be("MyPackType");
            jobj[Constants.VersionKey].Value<uint>().Should().Be(1);
            jobj["MyKey"].Value<int>().Should().Be(42);

            var target = serializer.Deserialize(pack);
            target.MyProperty.Should().Be(42);
        }

        [Shapeshifter()]
        public class MyTypeWithCustomPackName
        {
            public int MyProperty { get; set; }

            [Serializer(typeof(MyTypeWithCustomPackName), "MyPackType", 1)]
            public static void Serializer(IShapeshifterWriter writer, MyTypeWithCustomPackName itemToSerialize)
            {
                writer.Write("MyKey", itemToSerialize.MyProperty);
            }

            [Deserializer("MyPackType", 1)]
            public static object Deserializer(IShapeshifterReader reader)
            {
                return new MyTypeWithCustomPackName() { MyProperty = reader.Read<int>("MyKey") };
            }
        }

        [Test]
        public void CustomSerializerOnNonStaticMethod_Throws()
        {
            Action action = () => GetSerializer<MyClassWithNonStaticSerializerMethod>().Serialize(null);
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.InvalidUsageOfAttributeOnInstanceMethodId);
        }

        [Shapeshifter]
        public class MyClassWithNonStaticSerializerMethod
        {
            [Serializer(typeof(MyClassWithNonStaticSerializerMethod))]
            public void NonStaticSerializer(IShapeshifterWriter writer, MyClassWithNonStaticSerializerMethod item)
            {
            }
        }

        [Test]
        public void CustomDeserializerOnNonStaticMethod_Throws()
        {
            Action action = () => GetSerializer<MyClassWithNonStaticDeserializerMethod>().Serialize(null);
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.InvalidUsageOfAttributeOnInstanceMethodId);
        }

        [Shapeshifter]
        public class MyClassWithNonStaticDeserializerMethod
        {
            [Deserializer(typeof(MyClassWithNonStaticDeserializerMethod))]
            public void NonStaticDeserializer(IShapeshifterWriter writer, MyClassWithNonStaticDeserializerMethod item)
            {
            }
        }

        [Test]
        public void SerializerWithWrongSignature_Throws()
        {
            Action action = () => GetSerializer<MyClassWithInvalidSerializerMethodSignature>().Serialize(null);
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.InvalidSerializerMethodSignatureId);
        }

        [Shapeshifter]
        public class MyClassWithInvalidSerializerMethodSignature
        {
            [Serializer(typeof(MyClassWithInvalidSerializerMethodSignature))]
            public static void SerializerWithWrongSignature()
            {
            }
        }

        [Test]
        public void DeserializerWithWrongSignature_Throws()
        {
            Action action = () => GetSerializer<MyClassWithInvalidDeserializerMethodSignature>().Serialize(null);
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.InvalidDeserializerMethodSignatureId);
        }

        [Shapeshifter]
        public class MyClassWithInvalidDeserializerMethodSignature
        {
            [Deserializer(typeof(MyClassWithNonStaticDeserializerMethod))]
            public static void DeserializerWithWrongSignature()
            {
            }
        }
    }
}
