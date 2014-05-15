using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class CustomPackformatConverterTests
    {
        [Test]
        public void WhenConverterIsMissing_ExceptionIsThrown()
        {
            var input = new ExternalClass() { MyClass = new MyClass() { Value = 42 } };
            var serializer = new ShapeshifterSerializer<ExternalClass>();
            Action invoke = ()=> serializer.Serialize(input);
            invoke.ShouldThrow<ShapeshifterException>().And.Message.Should().Contain("MyClass");
        }

        [Test]
        public void WhenConverterIsPresent_ItIsInvoked()
        {
            var input = new ExternalClass() {MyClass = new MyClass() {Value = 42}};
            var serializer = new ShapeshifterSerializer<ExternalClass>(null, new[] { new MyClassSurrogateConverter() });
            var wireForm = serializer.Serialize(input);
            var result = serializer.Deserialize(wireForm);
            wireForm.Should().Contain("\"42\"");
            result.MyClass.Value.Should().Be(42);
        }

        [Test]
        public void ConversionOfAValueSucceeds()
        {
            var input = new StringClass() { Value = "Hello" };
            var serializer = new ShapeshifterSerializer<StringClass>(null, new [] { new StringClassSurrogateConverter() });
            var wireForm = serializer.Serialize(input);
            var result = serializer.Deserialize(wireForm);
            result.Value.Should().Be("Hello");
        }

        [Test]
        public void ConversionOfNull()
        {
            var input = new StringClass() { Value = null };
            var serializer = new ShapeshifterSerializer<StringClass>(null, new[] { new StringClassSurrogateConverter() });
            var wireForm = serializer.Serialize(input);

            Debug.WriteLine(wireForm);

            var result = serializer.Deserialize(wireForm);
            result.Value.Should().BeNull();
        }

        [KnownType(typeof(string))]
        private class StringClassSurrogateConverter : IPackformatSurrogateConverter
        {
            public object ConvertToSurrogate(object value)
            {
                return ((StringClass) value).Value;
            }

            public object ConvertFromSurrogate(Type targetType, object value)
            {
                return new StringClass() {Value = (string) value};
            }

            public bool CanConvert(Type type)
            {
                return type == typeof (StringClass);
            }
        }

        [DataContract]
        [Serializer]
        private class ExternalStringClass
        {
            [DataMember]
            public StringClass StringClass { get; set; }
        }

        private class StringClass
        {
            public string Value { get; set; }
        }

        [DataContract]
        [Serializer]
        private class ExternalClass
        {
            [DataMember]
            public MyClass MyClass { get; set; }
        }

        private class MyClass
        {
            public int Value { get; set; }
        }

        [KnownType(typeof(string))]
        private class MyClassSurrogateConverter : IPackformatSurrogateConverter
        {
            public object ConvertToSurrogate(object value)
            {
                var myClass = value as MyClass;
                return myClass.Value.ToString(CultureInfo.InvariantCulture);
            }

            public object ConvertFromSurrogate(Type targetType, object value)
            {
                var wireValue = value as string;
                return new MyClass() {Value = Int32.Parse(wireValue)};
            }

            public bool CanConvert(Type type)
            {
                return type == typeof (MyClass);
            }
        }
    }
}
