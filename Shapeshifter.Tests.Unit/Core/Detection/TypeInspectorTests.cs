using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Core.Detection;

namespace Shapeshifter.Tests.Unit.Core.Detection
{
#pragma warning disable 169
#pragma warning disable 649
// ReSharper disable UnusedMember.Local

    [TestFixture]
    public class TypeInspectorTests
    {
        [Test]
        public void IsSerializable_ShouldRecognize_DataContractAttirbute()
        {
            var ti = new TypeInspector(typeof(ClassWithDataContract));
            ti.HasDataContractAttribute.Should().BeTrue();
        }

        [Test]
        public void IsSerializable_ShouldRecognize_NoDataContractAttirbute()
        {
            var ti = new TypeInspector(typeof(ClassWithoutDataContract));
            ti.HasDataContractAttribute.Should().BeFalse();
        }

        [Test]
        public void SerializableItemCandidates_ShouldRecognize_DataMemberAttribute_OnPublicField()
        {
            var ti = new TypeInspector(typeof (ClassWithPublicField));
            var result = ti.SerializableMemberCandidates.ToList();
            result.Count.Should().Be(1);
            result[0].Name.Should().Be("_field");
        }

        [Test]
        public void SerializableItemCandidates_ShouldRecognize_DataMemberAttribute_OnPrivateField()
        {
            var ti = new TypeInspector(typeof(ClassWithPrivateField));
            var result = ti.SerializableMemberCandidates.ToList();
            result.Count.Should().Be(1);
            result[0].Name.Should().Be("_field");
        }

        [Test]
        public void SerializableItemCandidates_ShouldNotRecognize_DataMemberAttribute_OnStaticField()
        {
            var ti = new TypeInspector(typeof(ClassWithStaticField));
            var result = ti.SerializableMemberCandidates.ToList();
            result.Count.Should().Be(0);
        }

        [Test]
        public void SerializableItemCandidates_ShouldRecognize_DataMemberAttribute_OnPublicProperty()
        {
            var ti = new TypeInspector(typeof(ClassWithPublicProperty));
            var result = ti.SerializableMemberCandidates.ToList();
            result.Count.Should().Be(1);
            result[0].Name.Should().Be("Property");
        }

        [Test]
        public void SerializableItemCandidates_ShouldRecognize_DataMemberAttribute_OnPrivateProperty()
        {
            var ti = new TypeInspector(typeof(ClassWithPrivateProperty));
            var result = ti.SerializableMemberCandidates.ToList();
            result.Count.Should().Be(1);
            result[0].Name.Should().Be("Property");
        }

        [Test]
        public void SerializableItemCandidates_ShouldRecognize_DataMemberAttribute_OnBaseClasses()
        {
            var ti = new TypeInspector(typeof(DerivedClassWithFields));
            var result = ti.SerializableMemberCandidates.ToList();
            result.Count.Should().Be(2);
        }

        [Test]
        public void DataHolderMembers_PropertyOverrides_ShouldNotCauseException()
        {
            var ti = new TypeInspector(typeof(ClassWithPropertyOverrride));
            var result = ti.DataHolderMembers.ToList();
            result.Count.Should().Be(1);
        }

        [Test]
        public void PackformatName_OfNormalClass_ShouldBeTheClassName()
        {
            var ti = new TypeInspector(typeof (VersionOne));
            ti.PackformatName.Should().Be("VersionOne");
        }

        [Test]
        public void PackformatName_OfGenericClass_ShouldBeTheClassNameWithSingleGenericParameter()
        {
            var ti = new TypeInspector(typeof(GenericClass<int>));
            ti.PackformatName.Should().Be("GenericClass<Int32>");
        }

        [Test]
        public void PackformatName_OfGenericClass_ShouldBeTheClassNameWithTwoGenericParameters()
        {
            var ti = new TypeInspector(typeof(GenericClass<int, string>));
            ti.PackformatName.Should().Be("GenericClass<Int32,String>");
        }

        [Test]
        public void PackformatName_OfGenericClass_ShouldBeTheClassNameWithMultipleGenericParameters()
        {
            var ti = new TypeInspector(typeof(GenericClass<VersionOne, string>));
            ti.PackformatName.Should().Be("GenericClass<VersionOne,String>");
        }

        [Test]
        public void PackformatName_OfNestedGenericClass_ShouldBeTheClassNameWithGenericParameters()
        {
            var ti = new TypeInspector(typeof(GenericClass<GenericClass<int>, string>));
            ti.PackformatName.Should().Be("GenericClass<GenericClass<Int32>,String>");
        }

        [Test]
        public void Version_AdditionalProperty_ShouldBeDifferent()
        {
            var ti1 = new TypeInspector(typeof(VersionOne));
            var ti2 = new TypeInspector(typeof(VersionTwo));

            ti1.Version.Should().NotBe(ti2.Version);
        }

        [Test]
        public void Version_DifferentGenericProperty_ShouldBeDifferent()
        {
            var ti1 = new TypeInspector(typeof(GenericOne));
            var ti2 = new TypeInspector(typeof(GenericTwo));

            ti1.Version.Should().NotBe(ti2.Version);
        }

        [Test]
        public void SerializableItemCandidates_ContainsBaseClassPrivateFieldsAndPropertiesToo()
        {
            var typeInspector = new TypeInspector(typeof(MyClass));
            var items = typeInspector.SerializableMemberCandidates.ToList();
            items.Should().Contain(i => i.Name == "_myField");
            items.Should().Contain(i => i.Name == "MyProperty");
        }

        [Test]
        public void Version_DifferentOfOpenGenericTypes_DifferentVersions()
        {
            var ti1 = new TypeInspector(typeof (GenericClass<>));
            var ti2 = new TypeInspector(typeof(GenericClass2<>));

            ti1.Version.Should().NotBe(ti2.Version);
        }

        [Test]
        public void Version_DifferentEnumTypes_DifferentVersions()
        {
            var ti1 = new TypeInspector(typeof(MyEnumBaseline));
            var ti2 = new TypeInspector(typeof(MyEnumValueAdded));
            var ti3 = new TypeInspector(typeof(MyEnumValueModified));
            var ti4 = new TypeInspector(typeof(MyEnumValueDeleted));

            ti1.Version.Should().NotBe(ti2.Version);
            ti1.Version.Should().NotBe(ti3.Version);
            ti1.Version.Should().NotBe(ti4.Version);
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

        [DataContract]
        private class GenericClass<T>
        {
            [DataMember]
            private T _genericValue;

            [DataMember]
            private string _strValue;
        }

        [DataContract]
        private class GenericClass2<T>
        {
            [DataMember]
            private T _genericValue2;
            [DataMember]
            private string _strValue;
        }

        [DataContract]
        private class GenericClass<T1, T2>
        {

        }

        [DataContract]
        private class GenericOne
        {
            [DataMember]
            private List<int> Ids { get; set; }
        }

        [DataContract]
        private class GenericTwo
        {
            [DataMember]
            private List<string> Ids { get; set; }
        }

        [DataContract]
        private class VersionOne
        {
            [DataMember]
            private int Id { get; set; }
            
        }

        [DataContract]
        private class VersionTwo
        {
            [DataMember]
            private int Id { get; set; }

            [DataMember]
            private string Desc { get; set; }
        }

        [DataContract]
        private class ClassWithDataContract
        {
            
        }

        private class ClassWithoutDataContract
        {

        }


        private class DerivedClassWithFields : ClassWithPublicProperty
        {
            [DataMember]
            private int _derivedField;

            private int _nonMarkedDerivedField;
        }

        private class ClassWithPublicField
        {
            [DataMember]
            public string _field;

            public string _nonMarkedField;
        }

        private class ClassWithPrivateField
        {
            [DataMember]
            private string _field;

            private string _nonMarkedField;
        }

        private class ClassWithStaticField
        {
            [DataMember]
            public static string _field;
        }

        private class ClassWithPublicProperty
        {
            [DataMember]
            public string Property { get; set; }

            public string NonMarkedProperty { get; set; }
        }

        private class ClassWithPrivateProperty
        {
            [DataMember]
            public string Property { get; set; }

            public string NonMarkedProperty { get; set; }
        }

        private enum MyEnumBaseline
        {
            First,
            Second
        }

        private enum MyEnumValueAdded
        {
            First,
            Second,
            Third
        }

        private enum MyEnumValueModified
        {
            First,
            SecondModified,
        }

        private enum MyEnumValueDeleted
        {
            First,
        }

        [DataContract]
        private abstract class ClassWithAbstractProperty
        {
            [DataMember]
            protected abstract int MyProperty { get; }
        }

        [DataContract]
        private class ClassWithPropertyOverrride : ClassWithAbstractProperty
        {
            [DataMember]
            protected override int MyProperty
            {
                get { return 0; }
            }
        }
    }
}
