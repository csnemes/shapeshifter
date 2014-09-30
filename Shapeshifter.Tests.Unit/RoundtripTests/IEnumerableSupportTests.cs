using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class IEnumerableSupportTests : TestsBase
    {
        [Test]
        public void IEnumerableField_ShouldWork()
        {
            var classWithIEnumerableField = new ClassWithIEnumerableField
            {
                MyField = new List<int> {1, 2},
                MyProperty = new List<IEnumerable<string>>
                {
                    new List<string>() {"a", "b"},
                    new [] {"c"},
                    new string[0]
                }
            };

            var serializer = GetSerializer<ClassWithIEnumerableField>();
            var wireFormat = serializer.Serialize(classWithIEnumerableField);
            var result = serializer.Deserialize(wireFormat);
            result.MyField.Should().ContainInOrder(1,2);
            result.MyProperty.Should().HaveCount(3);
            result.MyProperty.ToList()[0].Should().Contain("a", "b");
            result.MyProperty.ToList()[1].Should().Contain("c");
            result.MyProperty.ToList()[2].Should().BeEmpty();
        }

        [Test]
        public void IEnumerableDeclaredWithObject_ShouldWriteTypeInfo()
        {
            var source = new List<object> {"a", 42};

            var serializer = GetSerializer<List<object>>();
            var wireFormat = serializer.Serialize(source); 
            var result = serializer.Deserialize(wireFormat);
            result[0].Should().BeOfType<string>().And.Be("a");
            result[1].Should().BeOfType<int>().And.Be(42);
        }

        [DataContract]
        [Shapeshifter]
        private class ClassWithIEnumerableField
        {
            [DataMember] 
            public IEnumerable<int> MyField;

            [DataMember]
            public IEnumerable<IEnumerable<string>> MyProperty { get; set; }
        }

    }
}
