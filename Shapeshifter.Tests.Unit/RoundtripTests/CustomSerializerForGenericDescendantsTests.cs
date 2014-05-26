using System;
using System.Reflection;
using System.Runtime.Serialization;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Shapeshifter.Core;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class CustomSerializerForGenericDescendantsTests : TestsBase
    {
        [Test]
        public void CustomSerializerForGenericBaseType_SerializesDescendant()
        {
            var source = new MyGrandchild
            {
                BaseProperty = 42,
            };

            var serializer = GetSerializer<MyGrandchild>();
            var pack = serializer.Serialize(source);
            var jobj = JObject.Parse(pack);

            jobj[Constants.TypeNameKey].Value<string>().Should().Be("MyGrandchild");
            jobj[Constants.VersionKey].Value<uint>().Should().Be(1);
            jobj["MyKey"].Value<int>().Should().Be(42);
        }

        [Test]
        public void CustomSerializerForGenericBaseType_DeserializesDescendant()
        {
            var source = new MyGrandchild
            {
                BaseProperty = 42,
            };

            var serializer = GetSerializer<MyGrandchild>();
            var pack = serializer.Serialize(source);
            var target = serializer.Deserialize(pack);
            target.Should().BeOfType<MyGrandchild>();
            target.BaseProperty.Should().Be(42);
        }

        [Shapeshifter]
        public abstract class MyBase<T>
        {
            public T BaseProperty { get; set; }

            [Serializer(typeof (MyBase<>), 1, ForAllDescendants = true)]
            public static void SerializeAnyDescendant(IShapeshifterWriter writer, MyBase<T> item)
            {
                writer.Write("MyKey", item.BaseProperty);
            }

            [Deserializer(typeof(MyBase<>), ForAllDescendants = true)]
            public static object DeserializeAnyDescendant(IShapeshifterReader reader, Type targetType)
            {
                var value = reader.Read<int>("MyKey");

                var valueProperty = targetType.GetPropertyRecursive("BaseProperty", BindingFlags.Public | BindingFlags.Instance);
                if (valueProperty == null)
                    throw new Exception(string.Format("BaseProperty not found on type {0}.", targetType.FullName));

                var result = FormatterServices.GetUninitializedObject(targetType);
                valueProperty.SetValue(result, value, null);
                return result;
            }
        }

        public class MyChild<T> : MyBase<T>
        {
        }

        public class MyChildWithInt : MyChild<int>
        {
        }

        public class MyGrandchild : MyChildWithInt
        {
        }

    }
}
