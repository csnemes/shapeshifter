using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Core;

namespace Shapeshifter.Tests.Unit
{
    [TestFixture]
    public class SerializationCandidatesDetectorTests
    {
        [Test]
        public void ShouldDetectMemberTypesWithDataMemberAttribute()
        {
            var result = PackformatCandidatesDetector.CreateFor(typeof (ExternalClass)).SerializationCandidates;
            result.ResolveSerializer(typeof (InnerClass)).Should().NotBeNull();
        }

        [Test]
        public void ShouldDetectItemTypeOfListMembersWithDataMemberAttribute()
        {
            var result = PackformatCandidatesDetector.CreateFor(typeof(ClassWithList)).SerializationCandidates;
            result.ResolveSerializer(typeof(InnerClass)).Should().NotBeNull();
        }
        
        [Test]
        public void ShouldDetectKnownTypesAddedDirectly()
        {
            var result = PackformatCandidatesDetector.CreateFor(typeof(ClassWithKnownTypes)).SerializationCandidates;
            result.ResolveSerializer(typeof(InnerClass)).Should().NotBeNull();
        }

        [Test]
        public void ShouldDetectKnownTypesAddedByMethod()
        {
            var result = PackformatCandidatesDetector.CreateFor(typeof(ClassWithKnownTypesMethod)).SerializationCandidates;
            result.ResolveSerializer(typeof(InnerClass)).Should().NotBeNull();
        }


        [DataContract]
        [Serializer]
        private class ExternalClass
        {
            [DataMember]
            public string PrimitiveType { get; set; }

            [DataMember]
            public InnerClass Inner { get; set; }
        }

        [DataContract]
        [Serializer]
        private class ClassWithList
        {
            [DataMember]
            public List<InnerClass> Inner { get; set; }
        }

        [DataContract]
        private class InnerClass
        {
            
        }

        [DataContract]
        [Serializer]
        [KnownType(typeof(InnerClass))]
        private class ClassWithKnownTypes
        {
            [DataMember]
            public string PrimitiveType { get; set; }
        }

        [DataContract]
        [Serializer]
        [KnownType("GetKnownTypes")]
        private class ClassWithKnownTypesMethod
        {
            [DataMember]
            public string PrimitiveType { get; set; }

            private static Type[] GetKnownTypes()
            {
                return new[] {typeof (InnerClass)};
            }
        }
    }
}
