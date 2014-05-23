using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Shapeshifter.Core;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class CustomSerializerForGenericsTests : TestsBase
    {
        [Test]
        public void CustomSerializerForConcreteType_Success()
        {
            var source = new MyType() {MyProperty = 42};

            var serializer = GetSerializer<MyType>();
            var pack = serializer.Serialize(source);
            var jobj = JObject.Parse(pack);

            jobj[Constants.TypeNameKey].Value<string>().Should().Be("MyType");
            jobj[Constants.VersionKey].Value<uint>().Should().Be(1);
            jobj["MyKey"].Value<int>().Should().Be(42);
        }

        [Test]
        [Ignore("Még nincs implementálva a funkció")]
        public void CustomSerializerForUnboundGenericType_Success()
        {
            var source = new MyType<string>() {MyProperty = 42};

            var serializer = GetSerializer<MyType<string>>();
            var pack = serializer.Serialize(source);
            var jobj = JObject.Parse(pack);

            jobj[Constants.TypeNameKey].Value<string>().Should().Be("MyType<string>");
            jobj[Constants.VersionKey].Value<uint>().Should().Be(1);
            jobj["UnboundGenericType.MyKey"].Value<int>().Should().Be(42);
        }

        [Test]
        [Ignore("Még nincs implementálva a funkció")]
        public void CustomSerializerForBoundGenericType_Success()
        {
            var source = new MyType<int>() {MyProperty = 42};

            var serializer = GetSerializer<MyType<int>>();
            var pack = serializer.Serialize(source);
            var jobj = JObject.Parse(pack);

            jobj[Constants.TypeNameKey].Value<string>().Should().Be("MyType<int>");
            jobj[Constants.VersionKey].Value<uint>().Should().Be(1);
            jobj["BoundGenericType.MyKey"].Value<int>().Should().Be(42);
        }

        [Shapeshifter]
        public class MyType
        {
            public int MyProperty { get; set; }

            // Minek a verziószám, miért nem generál egyet?
            [Serializer(typeof (MyType), 1)]
            public static void Serializer(IShapeshifterWriter writer, MyType itemToSerialize)
            {
                writer.Write("MyKey", itemToSerialize.MyProperty);
            }
        }

        [Shapeshifter]
        public class MyType<T>
        {
            public int MyProperty { get; set; }

            [Serializer(typeof (MyType<>), 1)]
            public static void UnboundGenericTypeSerializer(IShapeshifterWriter writer, MyType itemToSerialize)
            {
                writer.Write("UnboundGenericType.MyKey", itemToSerialize.MyProperty);
            }

            [Serializer(typeof (MyType<int>), 1)]
            public static void BoundGenericTypeSerializer(IShapeshifterWriter writer, MyType itemToSerialize)
            {
                writer.Write("BoundGenericType.MyKey", itemToSerialize.MyProperty);
            }
        }
    }
}
