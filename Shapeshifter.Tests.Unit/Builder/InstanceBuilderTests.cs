using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Builder;
using Shapeshifter.Core;
using Shapeshifter.Core.Deserialization;
using System;
using System.Collections.Generic;

namespace Shapeshifter.Tests.Unit.Builder
{
    [TestFixture]
    public class InstanceBuilderTests
    {
        [Test]
        public void PublicBaseValuesCanBeSet()
        {
            var builder = new InstanceBuilder<TestClass>();
            builder.SetMember("_basePublicField", "Hello");
            builder.SetMember("BasePublicProperty", 42);

            var instance = builder.GetInstance();

            instance._basePublicField.Should().Be("Hello");
            instance.BasePublicProperty.Should().Be(42);
        }

        [Test]
        public void PrivateBaseValuesCanBeSet()
        {
            var builder = new InstanceBuilder<TestClass>();
            builder.SetMember("_basePrivateField", "Hello");
            builder.SetMember("BasePrivateProperty", 42);

            var instance = builder.GetInstance();

            instance.GetBasePrivateField().Should().Be("Hello");
            instance.GetBasePrivateProperty().Should().Be(42);
        }

        [Test]
        public void PrivateChildValuesCanBeSet()
        {
            var builder = new InstanceBuilder<TestClass>();
            builder.SetMember("_childPrivateField", "Hello");
            builder.SetMember("ChildPrivateProperty", 42);

            var instance = builder.GetInstance();

            instance.GetChildPrivateField().Should().Be("Hello");
            instance.GetChildPrivateProperty().Should().Be(42);
        }

        [Test]
        public void MultipleGetInstance_ShouldThrowException()
        {
            var builder = new InstanceBuilder<TestClass>();

            builder.GetInstance();

            Action action = () => builder.GetInstance();

            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.InstanceAlreadyGivenAwayId);
        }

        [Test]
        public void SetMemberAfterGetInstance_ShouldThrowException()
        {
            var builder = new InstanceBuilder<TestClass>();

            builder.GetInstance();

            Action action = () => builder.SetMember("_childPrivateField", "Hello");

            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.InstanceAlreadyGivenAwayId);
        }

        [Test]
        public void SetNonExistingMember_ShouldThrowException()
        {
            var builder = new InstanceBuilder<TestClass>();

            Action action = () => builder.SetMember("noSuchField", "Hello");

            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.CannotFindFieldOrPropertyId);
        }

        [Test]
        public void SetWrongValue_ShouldThrowException()
        {
            var builder = new InstanceBuilder<TestClass>();

            Action action = () => builder.SetMember("_childPrivateField", 42);

            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.FailedToSetValueId);
        }

        [Test]
        public void ValuesCanBeRead()
        {
            var builder = new InstanceBuilder<TestClass>();
            builder.SetMember("_basePublicField", "Hello");
            builder.SetMember("BasePublicProperty", 42);

            builder.GetMember<string>("_basePublicField").Should().Be("Hello");
            builder.GetMember<int>("BasePublicProperty").Should().Be(42);
        }

        [Test]
        public void InstancePopulatedFromReader()
        {
            var objectProperties = new ObjectProperties(new Dictionary<string, object>()
            {
                {Constants.TypeNameKey, "TestClass"},
                {Constants.VersionKey, (long)1},
                {"_basePrivateField", "Hello"},
                {"BasePrivateProperty", 42},
                {"_childPrivateField", "Seeya"},
                {"ChildPrivateProperty", 43},
            });
            var reader = new ShapeshifterReader(objectProperties);

            var builder = new InstanceBuilder<TestClass>(reader);
            var instance = builder.GetInstance();

            instance.GetBasePrivateField().Should().Be("Hello");
            instance.GetBasePrivateProperty().Should().Be(42);
            instance.GetChildPrivateField().Should().Be("Seeya");
            instance.GetChildPrivateProperty().Should().Be(43);
        }

        private class TestClass : TestClassBase
        {
            private string _childPrivateField = null;
            private int ChildPrivateProperty { get; set; }

            public string GetChildPrivateField()
            {
                return _childPrivateField;
            }

            public int GetChildPrivateProperty()
            {
                return ChildPrivateProperty;
            }
        }


        private class TestClassBase
        {
            public string _basePublicField = null;
            public int BasePublicProperty { get; set; }

            private string _basePrivateField = null;
            private int BasePrivateProperty { get; set; }

            public string GetBasePrivateField()
            {
                return _basePrivateField;
            }

            public int GetBasePrivateProperty()
            {
                return BasePrivateProperty;
            }
        }
    }
}
