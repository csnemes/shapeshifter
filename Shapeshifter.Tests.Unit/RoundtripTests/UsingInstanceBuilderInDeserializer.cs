using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Builder;
using System.Runtime.Serialization;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class UsingInstanceBuilderInDeserializer : TestsBase
    {
        [Test]
        public void InstanceBuilderWorksRecursively_Success()
        {
            var input = new MyClassWithComplexField()
            {
                _myComplexField = new MyClassWithSimpleField()
                {
                    _mySimpleField = 42
                },
            };

            var machine = GetSerializer<MyClassWithComplexField>();
            var packed = machine.Serialize(input);
            var unpacked = machine.Deserialize(packed);

            unpacked._myComplexField._mySimpleField.Should().Be(42);
            unpacked._myIntField.Should().Be(44);
        }

        [Shapeshifter(1)]
        [DataContract]
        private class MyClassWithComplexField
        {
            [DataMember]
            public MyClassWithSimpleField _myComplexField;

            public int _myIntField = 0;

            [Deserializer(typeof (MyClassWithComplexField), 1)]
            public static MyClassWithComplexField Deserialize(IShapeshifterReader reader)
            {
                var builder = new InstanceBuilder<MyClassWithComplexField>(reader);
                builder.SetMember("_myIntField", 44);
                return builder.GetInstance();
            }
        }

        [DataContract]
        private class MyClassWithSimpleField
        {
            [DataMember]
            public int _mySimpleField;
        }
    }
}
