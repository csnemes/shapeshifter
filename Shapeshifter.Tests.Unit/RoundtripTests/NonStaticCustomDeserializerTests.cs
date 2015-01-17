using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Core;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class NonStaticCustomDeserializerTests : TestsBase
    {
        [Test]
        public void CustomDeserializerOverridesData()
        {
            var packed = @"{""__typeName"":""MyClass"",""__version"":1041101216,""Inner"":{""__typeName"":""InnerClass"",""__version"":849758624,""Value"":""42""},""OtherInner"":{""__typeName"":""OtherInnerClass"",""__version"":849758624,""Value"":""ABC""}}";
            var serializer = new ShapeshifterSerializer<MyClass>(new[] { typeof(DeserializerClass) });
            var result = serializer.Deserialize(packed);
            result.Inner.Value.Should().Be("OVER");
            result.OtherInner.Value.Should().Be("OVER");
        }

        [Test]
        public void CustomDeserializerHasSingleInstanceForTheSerialization()
        {
            var packed = @"{""__typeName"":""MyClass"",""__version"":1041101216,""Inner"":{""__typeName"":""InnerClass"",""__version"":849758624,""Value"":""42""},""OtherInner"":{""__typeName"":""OtherInnerClass"",""__version"":849758624,""Value"":""ABC""}}";
            var serializer = new ShapeshifterSerializer<MyClass>(new[] { typeof(SpecialDeserializerClass) });
            var result = serializer.Deserialize(packed);
            result.OtherInner.Value.Should().Be("42");
        }

        [Test]
        public void DeserializerEndingIsCalled()
        {
            var packed = @"{""__typeName"":""MyClass"",""__version"":1041101216,""Inner"":{""__typeName"":""InnerClass"",""__version"":849758624,""Value"":""42""},""OtherInner"":{""__typeName"":""OtherInnerClass"",""__version"":849758624,""Value"":""ABC""}}";
            var serializer = new ShapeshifterSerializer<MyClass>(new[] { typeof(DeserializerClassWithDeserializationEnding) });
            var result = serializer.Deserialize(packed);
            result.Inner.Value.Should().Be("ENDING"); 
        }

        [Test]
        public void DeserializerEndingIsCalledAndThrows()
        {
            var packed = @"{""__typeName"":""MyClass"",""__version"":1041101216,""Inner"":{""__typeName"":""InnerClass"",""__version"":849758624,""Value"":""42""},""OtherInner"":{""__typeName"":""OtherInnerClass"",""__version"":849758624,""Value"":""ABC""}}";
            var serializer = new ShapeshifterSerializer<MyClass>(new[] { typeof(DeserializerClassWithDeserializationEndingThrows) });
            Action action = () => serializer.Deserialize(packed);
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.FailedToRunDeserializationEndingId);
        }


        [ShapeshifterRoot]
        [DataContract]
        public class MyClass
        {
            [DataMember]
            public InnerClass Inner { get; set; }
            [DataMember]
            public OtherInnerClass OtherInner { get; set; }
        }

        [DataContract]
        public class InnerClass
        {
            [DataMember]
            public string Value { get; set; }            
        }

        [DataContract]
        public class OtherInnerClass
        {
            [DataMember]
            public string Value { get; set; }            
        }

        public class DeserializerClass
        {
            [Deserializer("InnerClass", 849758624)]
            public object InnerClassDeserializer(IShapeshifterReader reader)
            {
                return new InnerClass() { Value = "OVER" };
            }

            [Deserializer("OtherInnerClass", 849758624)]
            public object OtherInnerDeserializer(IShapeshifterReader reader)
            {
                return new OtherInnerClass() { Value = "OVER" };
            }
        }

        public class DeserializerClassWithDeserializationEnding
        {
            private InnerClass _dataSavedForLaterAccess;

            public void DeserializationEnding()
            {
                _dataSavedForLaterAccess.Value = "ENDING";
            }

            [Deserializer("InnerClass", 849758624)]
            public object InnerClassDeserializer(IShapeshifterReader reader)
            {
                _dataSavedForLaterAccess = new InnerClass() { Value = "OVER" };
                return _dataSavedForLaterAccess;
            }

            [Deserializer("OtherInnerClass", 849758624)]
            public object OtherInnerDeserializer(IShapeshifterReader reader)
            {
                return new OtherInnerClass() { Value = "OVER" };
            }
        }

        public class DeserializerClassWithDeserializationEndingThrows
        {
            public void DeserializationEnding()
            {
                throw new ApplicationException("Failure");
            }

            [Deserializer("InnerClass", 849758624)]
            public object InnerClassDeserializer(IShapeshifterReader reader)
            {
                return new InnerClass() { Value = "OVER" };
            }

            [Deserializer("OtherInnerClass", 849758624)]
            public object OtherInnerDeserializer(IShapeshifterReader reader)
            {
                return new OtherInnerClass() { Value = "OVER" };
            }
        }

        public class SpecialDeserializerClass
        {
            private string _slotToPassOverData;

            [Deserializer("InnerClass", 849758624)]
            public object InnerClassDeserializer(IShapeshifterReader reader)
            {
                _slotToPassOverData = reader.Read<string>("Value");
                return new InnerClass() { Value = "OVER" };
            }

            [Deserializer("OtherInnerClass", 849758624)]
            public object OtherInnerDeserializer(IShapeshifterReader reader)
            {
                return new OtherInnerClass() { Value = _slotToPassOverData };
            }
        }
    }
}
