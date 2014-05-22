using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Core.Detection;

namespace Shapeshifter.Tests.Unit.Core
{
    [TestFixture]
    public class SerializationCandidatesDetectorTests
    {
        [Test]
        public void ShouldDetectMemberTypesWithDataMemberAttribute()
        {
            var result = MetadataExplorer.CreateFor(typeof (ExternalClass)).Serializers;
            result.ResolveSerializer(typeof (InnerClass)).Should().NotBeNull();
        }

        [Test]
        public void ShouldDetectItemTypeOfListMembersWithDataMemberAttribute()
        {
            var result = MetadataExplorer.CreateFor(typeof(ClassWithList)).Serializers;
            result.ResolveSerializer(typeof(InnerClass)).Should().NotBeNull();
        }
        
        [Test]
        public void ShouldDetectKnownTypesAddedDirectly()
        {
            var result = MetadataExplorer.CreateFor(typeof(ClassWithKnownTypes)).Serializers;
            result.ResolveSerializer(typeof(InnerClass)).Should().NotBeNull();
        }

        [Test]
        public void ShouldDetectKnownTypesAddedByMethod()
        {
            var result = MetadataExplorer.CreateFor(typeof(ClassWithKnownTypesMethod)).Serializers;
            result.ResolveSerializer(typeof(InnerClass)).Should().NotBeNull();
        }


        [DataContract]
        [ShapeshifterRoot]
        private class ExternalClass
        {
            [DataMember]
            public string PrimitiveType { get; set; }

            [DataMember]
            public InnerClass Inner { get; set; }
        }

        [DataContract]
        [ShapeshifterRoot]
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
        [ShapeshifterRoot]
        [KnownType(typeof(InnerClass))]
        private class ClassWithKnownTypes
        {
            [DataMember]
            public string PrimitiveType { get; set; }
        }

        [DataContract]
        [ShapeshifterRoot]
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
