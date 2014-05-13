using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Core;

namespace Shapeshifter.Tests.Unit
{
    [TestFixture]
    public class PackformatReaderTests
    {
        [Test]
        public void ClassDeserializationTest()
        {
            string json = @"{
                    '__typeName' : 'PersonInfo',
                    '__version' : 1,
                    '_firstName' : 'Jeno',
                    '_lastName' : 'Nagy'
             }";

            var result = Deserialize<PersonInfo>(json);

            result.LastName.Should().Be("Nagy");
            result.FirstName.Should().Be("Jeno");
        }

        [Test]
        public void NestedClassDeserializationTest()
        {
            string json = @"{
            __typeName : 'OwnedItem',
            __version  : 2,
            Item : 'Intel PC',
            Owner : {
                    '__typeName' : 'PersonInfo',
                    '__version' : 1,
                    '_firstName' : 'Jeno',
                    '_lastName' : 'Nagy'
                }
             }";

            var result = Deserialize<OwnedItem>(json);

            result.Owner.LastName.Should().Be("Nagy");
            result.Owner.FirstName.Should().Be("Jeno");
            result.Item.Should().Be("Intel PC");
        }

        [Test]
        public void NestedClassDeserializationWithVersionMismatchTest()
        {
            string json = @"{
            __typeName : 'OwnedItem',
            __version  : 1,
            OldItem : 'Intel PC',
            Owner : {
                    '__typeName' : 'PersonInfo',
                    '__version' : 1,
                    '_firstName' : 'Jeno',
                    '_lastName' : 'Nagy'
                }
             }";

            var result = Deserialize<OwnedItem>(json);

            result.Owner.LastName.Should().Be("Nagy");
            result.Owner.FirstName.Should().Be("Jeno");
            result.Item.Should().Be("Intel PC");
        }

        private T Deserialize<T>(string input)
        {
            var typeContext = PackformatCandidatesDetector.CreateFor(typeof (T)).DeserializationCandidates;
            var engine = new PackformatReader(input, typeContext);
            return (T)engine.Unpack<T>();
        }

        [DataContract]
        [Serializer(Version=2)]
        private class OwnedItem
        {
            [DataMember]
            public PersonInfo Owner { get;  set; }
            [DataMember]
            public string Item { get; set; }

            [Deserializer("OwnedItem", 1)]
            private static object TransformVersion1To2(IPackformatValueReader reader)
            {
                return new OwnedItem()
                {
                    Item = reader.GetValue<string>("OldItem"),
                    Owner = reader.GetValue<PersonInfo>("Owner")
                };
            }
        }
        
        [DataContract]
        [Serializer(1)]
        private class PersonInfo
        {
            public PersonInfo(string firstName, string lastName)
            {
                _firstName = firstName;
                _lastName = lastName;
            }

            [DataMember]
            private readonly string _firstName;

            [DataMember]
            private string _lastName;

            public string FirstName
            {
                get { return _firstName; }
            }

            public string LastName
            {
                get { return _lastName; }
            }
        }
    }
}
