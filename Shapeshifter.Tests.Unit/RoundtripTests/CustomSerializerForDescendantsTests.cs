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
    public class CustomSerializerForDescendantsTests : TestsBase
    {
        [Test]
        public void CustomSerializerForBaseType_SerializesDescendant()
        {
            var source = new MyGrandchild()
            {
                BaseProperty = 42,
            };

            var serializer = GetSerializer<MyBase>();
            var pack = serializer.Serialize(source);
            var jobj = JObject.Parse(pack);

            jobj[Constants.TypeNameKey].Value<string>().Should().Be("MyGrandchild");
            jobj[Constants.VersionKey].Value<uint>().Should().Be(1);
            jobj["MyKey"].Value<int>().Should().Be(42);
        }

        [Test]
        public void CustomSerializerForBaseType_DeserializesDescendant()
        {
            var source = new MyGrandchild()
            {
                BaseProperty = 42,
            };

            var serializer = GetSerializer<MyBase>();
            var pack = serializer.Serialize(source);
            var target = serializer.Deserialize(pack);
            target.Should().BeOfType<MyGrandchild>();
            target.BaseProperty.Should().Be(42);
        }

        [Shapeshifter]
        public abstract class MyBase
        {
            public int BaseProperty { get; set; }

            [Serializer(typeof (MyBase), 1, ForAllDescendants = true)]
            public static void SerializeAnyDescendant(IShapeshifterWriter writer, MyBase item)
            {
                writer.Write("MyKey", item.BaseProperty);
            }

            [Deserializer(typeof(MyBase), 1, ForAllDescendants = true)]
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

        public class MyChild : MyBase
        {
        }

        public class MyGrandchild : MyChild
        {
        }

        [Test]
        public void WrongDeserializerAttribute_Throws()
        {
            Action action = () => GetSerializer<MyBaseWithWrongDeserializerAttribute>().Serialize(null);
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.DeserializerAttributeTargetTypeMustBeSpecifiedForAllDescendantsId);
        }

        [Shapeshifter]
        public class MyBaseWithWrongDeserializerAttribute
        {
            [Deserializer("MyBase", 1, ForAllDescendants = true)]
            public static object Deserialize(IShapeshifterReader reader, Type targetType)
            {
                return null;
            }
        }

        [Test]
        public void WrongDeserializerMethodSignature_Throws()
        {
            Action action = () => GetSerializer<MyBaseWithWrongDeserializerMethodSignature>().Serialize(null);
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.InvalidDeserializerMethodSignatureForAllDescendantsId);
        }

        [Shapeshifter]
        public class MyBaseWithWrongDeserializerMethodSignature
        {
            [Deserializer(typeof(MyBase), ForAllDescendants = true)]
            public static object Deserialize(IShapeshifterReader reader)
            {
                return null;
            }
        }
    }
}
