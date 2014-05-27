using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Core;
using System.Linq;
using System.Reflection;

namespace Shapeshifter.Tests.Unit.Core
{
    [TestFixture]
    public class TypeExtensionsTests
    {
        [Test]
        public void IsMostDerivedIn_Success()
        {
            var properties = typeof (ClassWithPropertyOverrride).GetAllPropertiesRecursive(BindingFlags.Instance | BindingFlags.NonPublic).ToList();
            properties.Should().HaveCount(2);

            var abstractProperty = properties.Single(i => i.DeclaringType == typeof (ClassWithAbstractProperty));
            abstractProperty.IsMostDerivedIn(properties).Should().BeFalse();

            var overriddenProperty = properties.Single(i => i.DeclaringType == typeof(ClassWithPropertyOverrride));
            overriddenProperty.IsMostDerivedIn(properties).Should().BeTrue();
        }

        private abstract class ClassWithAbstractProperty
        {
            protected abstract int MyProperty { get; }
        }

        private class ClassWithPropertyOverrride : ClassWithAbstractProperty
        {
            protected override int MyProperty
            {
                get { return 0; }
            }
        }
    }
}
