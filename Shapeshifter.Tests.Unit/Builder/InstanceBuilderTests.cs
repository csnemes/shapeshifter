using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Builder;
using Shapeshifter.Core;
using Shapeshifter.Tests.Unit.SchemaComparison.Version1;

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

        private class TestClass : TestClassBase
        {
            private string _childPrivateField;
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
            public string _basePublicField;
            public int BasePublicProperty { get; set; }

            private string _basePrivateField;
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
