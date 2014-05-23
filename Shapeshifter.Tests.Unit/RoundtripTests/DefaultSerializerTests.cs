using System;
using System.Runtime.Serialization;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Core;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class DefaultSerializerTests :TestsBase
    {
        [Test]
        [ExpectedException(typeof(ShapeshifterException), Handler = "TypeWithDataContractWithoutShapeshifterAttribute_CheckExceptionId")]
        public void TypeWithDataContractWithoutShapeshifterAttribute_Throws()
        {
            GetSerializer<TypeWithoutShapeshifterAttribute>().Serialize(null);
        }

        public void TypeWithDataContractWithoutShapeshifterAttribute_CheckExceptionId(Exception exception)
        {
            (exception as ShapeshifterException).Id.Should().Be(Exceptions.ShapeshifterAttributeMissingId);
        }

        [Test]
        public void TypeWithShapeshifterWithoutDataContractAttribute_NoErrorButNoPropertySerialized()
        {
            var source = new TypeWithoutDataContractAttribute();
            var serializer = GetSerializer<TypeWithoutDataContractAttribute>();
            var pack = serializer.Serialize(source);
            var target = serializer.Deserialize(pack);
            target.MyProperty.Should().Be(default(int));
        }

        [Test]
        public void TypeWithSerializerAndDataContractAttribute_SuccessfulRoundtrip()
        {
            var source = new TypeWithShapeshifterAndDataContractAttribute() {MyProperty = 42};
            var serializer = GetSerializer<TypeWithShapeshifterAndDataContractAttribute>();
            var pack = serializer.Serialize(source);
            var target = serializer.Deserialize(pack);
            target.MyProperty.Should().Be(42);
        }

        [Shapeshifter]
        public class TypeWithoutDataContractAttribute
        {
            public int MyProperty { get; set; }
        }

        [DataContract]
        public class TypeWithoutShapeshifterAttribute
        {
            [DataMember]
            public int MyProperty { get; set; }
        }

        [Shapeshifter]
        [DataContract]
        public class TypeWithShapeshifterAndDataContractAttribute
        {
            [DataMember]
            public int MyProperty { get; set; }
        }
    }
}
