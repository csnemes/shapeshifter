using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Runtime.Serialization;
using Shapeshifter.Core;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class GenericSupportTests:TestsBase
    {
        [Test]
        public void SameArgumentTypeWriteAndReadBack_ShouldWork()
        {
            var serializer = GetSerializer<Generic<int>>();
            var wireFormat = serializer.Serialize(new Generic<int> { Value = 42 });
            var result = serializer.Deserialize(wireFormat);
            result.Value.Should().Be(42);
        }

        [Test]
        public void SameArgumentTypeWriteAndReadBack_SamePackformatSignature_ShouldWork()
        {
            var serializer = GetSerializer<GenericNameDiffTest<int>>();
            var wireFormat = serializer.Serialize(new GenericNameDiffTest<int> {Value = 42});
            var result = serializer.Deserialize(wireFormat);
            result.Value.Should().Be(42);
        }

        [Test]
        public void DifferentArgumentTypeWriteAndReadBack_SamePackformatSignature_ShouldNotWork()
        {
            var serializer = GetSerializer<GenericNameDiffTest<int>>();
            var deserializer = GetSerializer<GenericNameDiffTest<string>>(); 
            var wireFormat = serializer.Serialize(new GenericNameDiffTest<int> { Value = 42 });
            Action action = () => deserializer.Deserialize(wireFormat);
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.CannotFindDeserializerId);
        }

        [Test]
        public void TypeWithGenericMembers_SerializedAndDeserializedCorrectly()
        {
            var source = new TypeWithGenericMembers<string>
            {
                Value1 = new Generic<string> {Value = "MyValue1"},
                Value2 = new Generic<int>() {Value = 42}
            };
            var serializer = GetSerializer<TypeWithGenericMembers<string>>();
            var pack = serializer.Serialize(source);
            var target = serializer.Deserialize(pack);

            target.Value1.Value.Should().Be("MyValue1");
            target.Value2.Value.Should().Be(42);
        }

        [Test]
        public void TypeWithGenericMembers_TypeNameIsPrettyInThePackformat()
        {
            var source = new TypeWithGenericMembers<string>();
            var serializer = GetSerializer<TypeWithGenericMembers<string>>();
            var pack = serializer.Serialize(source);
            var jobj = JObject.Parse(pack);

            jobj[Constants.TypeNameKey].Value<string>().Should().Be("TypeWithGenericMembers<String>");
        }

        [DataContract]
        [ShapeshifterRoot]
        private class GenericNameDiffTest<T>
        {
            [DataMember]
            public int Value { get; set; }
        }

        [DataContract]
        [ShapeshifterRoot]
        private class Generic<T>
        {
            [DataMember]
            public T Value { get; set; }
        }

        [DataContract]
        [ShapeshifterRoot]
        private class TypeWithGenericMembers<T>
        {
            [DataMember]
            public Generic<T> Value1 { get; set; }

            [DataMember]
            public Generic<int> Value2 { get; set; }
        }

        [Test]
        public void UsageOfOpenGenericAsKnownType_Throws()
        {
            Action action = () =>  GetSerializer<TypeWithOpenGenericAsKnownType>().Serialize(new TypeWithOpenGenericAsKnownType());
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.IllegalUsageOfOpenGenericAsKnownTypeId);
        }
        
        [ShapeshifterRoot]
        [DataContract]
        [KnownType(typeof(Generic<>))]
        private class TypeWithOpenGenericAsKnownType
        {
        }
    }
}
