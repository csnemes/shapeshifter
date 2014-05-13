using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.History;

namespace Shapeshifter.Tests.Unit.History
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
        [Serializer]
        private class InnerClass
        {
            [DataMember]
            public string Value { get; set; } 
        }

        [DataContract]
        [Serializer]
        private class ExternalClass
        {
            [DataMember]
            public List<InnerClass> Values { get; set; }
        }

        private class ConverterClass
        {
            [Deserializer(typeof(InnerClass), 100)]
            public static object Deserializer(IPackformatValueReader reader)
            {
                return null;
            }

            [Serializer(typeof(InnerClass), 100)]
            public static void Serializer(IPackformatValueWriter writer, InnerClass item)
            {
            }
        }
    }
}
