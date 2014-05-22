using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Tests.Unit.RoundtripTests;

namespace Shapeshifter.Tests.Unit.New
{
    [TestFixture]
    public class DefaultRoundtripTests :TestsBase
    {
        [Test]
        [ExpectedException(typeof(ShapeshifterException))]
        public void TypeWithoutSerializerAttribute_Throws()
        {
            var source = new TypeWithoutSerializerAttribute();
            var serializer = GetSerializer<TypeWithoutSerializerAttribute>();
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
            var source = new TypeWithSerializerAndDataContractAttribute() {MyProperty = 42};
            var serializer = GetSerializer<TypeWithSerializerAndDataContractAttribute>();
            var pack = serializer.Serialize(source);
            var target = serializer.Deserialize(pack);
            target.MyProperty.Should().Be(42);
        }

        [Serializer]
        public class TypeWithoutDataContractAttribute
        {
            public int MyProperty { get; set; }
        }

        [DataContract]
        public class TypeWithoutSerializerAttribute
        {
            [DataMember]
            public int MyProperty { get; set; }
        }

        [Serializer]
        [DataContract]
        public class TypeWithSerializerAndDataContractAttribute
        {
            [DataMember]
            public int MyProperty { get; set; }
        }
    }
}
