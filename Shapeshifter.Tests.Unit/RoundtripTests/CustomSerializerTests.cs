using System;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Core;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class CustomSerializerTests : TestsBase
    {
        [Test]
        [ExpectedException(typeof(ShapeshifterException), Handler = "CustomSerializerOnNonStaticMethod_CheckExceptionId")]
        public void CustomSerializerOnNonStaticMethod_Throws()
        {
            GetSerializer<MyClassWithNonStaticSerializerMethod>().Serialize(null);
        }

        public void CustomSerializerOnNonStaticMethod_CheckExceptionId(Exception exception)
        {
            (exception as ShapeshifterException).Id.Should().Be(Exceptions.InvalidUsageOfAttributeOnInstanceMethodId);
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
        [ExpectedException(typeof(ShapeshifterException), Handler = "CustomDeserializerOnNonStaticMethod_CheckExceptionId")]
        public void CustomDeserializerOnNonStaticMethod_Throws()
        {
            GetSerializer<MyClassWithNonStaticDeserializerMethod>().Serialize(null);
        }

        public void CustomDeserializerOnNonStaticMethod_CheckExceptionId(Exception exception)
        {
            (exception as ShapeshifterException).Id.Should().Be(Exceptions.InvalidUsageOfAttributeOnInstanceMethodId);
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
        [ExpectedException(typeof(ShapeshifterException), Handler = "SerializerWithWrongSignature_CheckExceptionId")]
        public void SerializerWithWrongSignature_Throws()
        {
            GetSerializer<MyClassWithInvalidSerializerMethodSignature>().Serialize(null);
        }

        public void SerializerWithWrongSignature_CheckExceptionId(Exception exception)
        {
            (exception as ShapeshifterException).Id.Should().Be(Exceptions.InvalidSerializerMethodSignatureId);
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
        [ExpectedException(typeof(ShapeshifterException), Handler = "DeserializerWithWrongSignature_CheckExceptionId")]
        public void DeserializerWithWrongSignature_Throws()
        {
            GetSerializer<MyClassWithInvalidDeserializerMethodSignature>().Serialize(null);
        }

        public void DeserializerWithWrongSignature_CheckExceptionId(Exception exception)
        {
            (exception as ShapeshifterException).Id.Should().Be(Exceptions.InvalidDeserializerMethodSignatureId);
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
