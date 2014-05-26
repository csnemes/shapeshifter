using System;
using System.Runtime.Serialization;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Shapeshifter.Core;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class DefaultSerializerTests :TestsBase
    {
        [Test]
        public void TypeWithDataContractWithoutShapeshifterAttribute_Throws()
        {
            Action action = () => GetSerializer<TypeWithoutShapeshifterAttribute>().Serialize(null);
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.ShapeshifterAttributeMissingId);
        }

        [Test]
        public void TypeWithShapeshifterWithoutDataContractAttribute_NoDefaultSerializerCreated()
        {
            var source = new TypeWithoutDataContractAttribute();
            var serializer = GetSerializer<TypeWithoutDataContractAttribute>();
            Action action = () => serializer.Serialize(source);
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.SerializerResolutionFailedId);
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

        [Test]
        public void TypeWithCustomPackformatNameAndVersion_SuccessfulRoundtrip()
        {
            var source = new TypeWithCustomPackformatName() { MyProperty = 42 };
            var serializer = GetSerializer<TypeWithCustomPackformatName>();
            var pack = serializer.Serialize(source);

            var jobj = JObject.Parse(pack);

            jobj[Constants.TypeNameKey].Value<string>().Should().Be("MyPackformatName");
            jobj[Constants.VersionKey].Value<uint>().Should().Be(1);
            jobj["MyProperty"].Value<int>().Should().Be(42);

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

        [Shapeshifter("MyPackformatName", 1)]
        [DataContract]
        public class TypeWithCustomPackformatName
        {
            [DataMember]
            public int MyProperty { get; set; }
        }
    }
}
