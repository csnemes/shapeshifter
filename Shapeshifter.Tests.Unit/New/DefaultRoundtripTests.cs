using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Tests.Unit.RoundtripTests;
using System.Runtime.Serialization;

namespace Shapeshifter.Tests.Unit.New
{
    [TestFixture]
    public class DefaultRoundtripTests :TestsBase
    {
        [Test]
        [ExpectedException(typeof(ShapeshifterException))]
        public void TypeWithoutShapeshifterRootAttribute_Throws()
        {
            var source = new TypeWithoutShapeshifterRootAttribute();
            var serializer = GetSerializer<TypeWithoutShapeshifterRootAttribute>();
            serializer.Serialize(source);
        }

        [Test]
        [ExpectedException(typeof(ShapeshifterException))]
        public void TypeWithoutDataContractAttribute_Throws()
        {
            var source = new TypeWithoutDataContractAttribute();
            var serializer = GetSerializer<TypeWithoutDataContractAttribute>();
            serializer.Serialize(source);
        }

        [Test]
        public void TypeWithSerializerAndDataContractAttribute_SuccessfulRoundtrip()
        {
            var source = new TypeWithShapeshifterRootAndDataContractAttribute() {MyProperty = 42};
            var serializer = GetSerializer<TypeWithShapeshifterRootAndDataContractAttribute>();
            var pack = serializer.Serialize(source);
            var target = serializer.Deserialize(pack);
            target.MyProperty.Should().Be(42);
        }

        [ShapeshifterRoot]
        public class TypeWithoutDataContractAttribute
        {
            public int MyProperty { get; set; }
        }

        [DataContract]
        public class TypeWithoutShapeshifterRootAttribute
        {
            [DataMember]
            public int MyProperty { get; set; }
        }

        [ShapeshifterRoot]
        [DataContract]
        public class TypeWithShapeshifterRootAndDataContractAttribute
        {
            [DataMember]
            public int MyProperty { get; set; }
        }
    }
}
