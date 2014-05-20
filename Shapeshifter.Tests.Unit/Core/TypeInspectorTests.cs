using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Core;
using System.Linq;
using System.Runtime.Serialization;

namespace Shapeshifter.Tests.Unit.Core
{
    [TestFixture]
    public class TypeInspectorTests
    {
        [Test]
        public void SerializableItemCandidates_ContainsBaseClassPrivateFieldsAndPropertiesToo()
        {
            var typeInspector = new TypeInspector(typeof (MyClass));
            var items = typeInspector.SerializableItemCandidates.ToList();
            items.Should().Contain(i => i.Name == "_myField");
            items.Should().Contain(i => i.Name == "MyProperty");
        }

        [DataContract]
        public abstract class MyBaseClass
        {
            [DataMember]
            private int _myField;

            [DataMember]
            private int MyProperty { get; set; }
        }

        [DataContract]
        public class MyClass : MyBaseClass
        { }
    }
}
