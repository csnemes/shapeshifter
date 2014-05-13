using System;
using System.Collections.Generic;
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
            var serializer = new ShapeshifterSerializer<ExternalClass>(null, new[] { new MyClassConverter() });
            var wireForm = serializer.Serialize(input);
            var result = serializer.Deserialize(wireForm);
            wireForm.Should().Contain("\"42\"");
            result.MyClass.Value.Should().Be(42);
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

        private class MyClassConverter : ICustomPackformatConverter
        {
            public object ConvertToPackformat(object value)
            {
                var myClass = value as MyClass;
                return myClass.Value.ToString(CultureInfo.InvariantCulture);
            }

            public object ConvertFromPackformat(Type targetType, object value)
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
