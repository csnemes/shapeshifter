using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Core.Detection;

namespace Shapeshifter.Tests.Unit.Core.Detection
{
    [TestFixture]
    public class MetadataExplorerTests
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
        [Shapeshifter]
        private class ExternalClass
        {
            [DataMember]
            public string PrimitiveType { get; set; }

            [DataMember]
            public InnerClass Inner { get; set; }
        }

        [DataContract]
        [Shapeshifter]
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
        [Shapeshifter]
        [KnownType(typeof(InnerClass))]
        private class ClassWithKnownTypes
        {
            [DataMember]
            public string PrimitiveType { get; set; }
        }

        [DataContract]
        [Shapeshifter]
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
