using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.SchemaComparison.Impl;

namespace Shapeshifter.Tests.Unit.SchemaComparison
{
    [TestFixture]
    public class SnapshotDetectorTests
    {
        [Test]
        public void ShouldRecognizeDefaultSerializerAndDeserializer()
        {
            var result = SnapshotDetector.CreateFor(typeof (InnerClass));
            result.Serializers.Count().Should().Be(1);
            result.Deserializers.Count().Should().Be(1);
        }

        [Test]
        public void ShouldRecognizeDefaultSerializerAndDeserializerInHierarchy()
        {
            var result = SnapshotDetector.CreateFor(typeof(ExternalClass));
            result.Serializers.Count().Should().Be(2);
            result.Deserializers.Count().Should().Be(2);
        }

        [Test]
        public void ShouldRecognizeSerializerAndDeserializerMethods()
        {
            var result = SnapshotDetector.CreateFor(typeof(ConverterClass));
            result.Serializers.Count().Should().Be(1);
            result.Deserializers.Count().Should().Be(1);
        }


        [DataContract]
        [Shapeshifter]
        private class InnerClass
        {
            [DataMember]
            public string Value { get; set; } 
        }

        [DataContract]
        [Shapeshifter]
        private class ExternalClass
        {
            [DataMember]
            public List<InnerClass> Values { get; set; }
        }

        private class ConverterClass
        {
            [Deserializer(typeof(InnerClass), 100)]
            public static object Deserializer(IShapeshifterReader reader)
            {
                return null;
            }

            [Serializer(typeof(InnerClass), 100)]
            public static void Serializer(IShapeshifterWriter writer, InnerClass item)
            {
            }
        }
    }
}
