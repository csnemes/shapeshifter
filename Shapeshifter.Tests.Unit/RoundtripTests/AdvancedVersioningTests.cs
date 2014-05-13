using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class AdvancedVersioningTests
    {
        [Test]
        public void ReadOldVersionIntoNewVersion()
        {
            var input = new PersonVersionOne() { Age = 18, Name = new Name() { FirstName = "Jenő", LastName = "Kovács"} };
            var serializer = GetSerializer<PersonVersionOne>();
            var deserializer = GetSerializer<PersonVersionTwo>();
            var packed = serializer.Serialize(input);

            var result = deserializer.Deserialize(packed);

            result.Age.Should().Be(18);
            result.Name.Should().Be("Jenő Kovács");
        }

        private IShapeshifterSerializer<T> GetSerializer<T>()
        {
            return new ShapeshifterSerializer<T>();
        }

        [DataContract]
        [Serializer]
        private class PersonVersionOne
        {
            [DataMember]
            public Name Name { get; set; }
            [DataMember]
            public int Age { get; set; }
        }

        [DataContract]
        [Serializer]
        private class PersonVersionTwo //droping a class 
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public int Age { get; set; }

            [Deserializer("PersonVersionOne")]
            private static object TransformVersionOne(IPackformatValueReader reader)
            {
                var age = reader.GetValue<int>("Age");
                var namePack = reader.GetValueReader("Name");
                var firstName = namePack.GetValue<string>("FirstName");
                var lastName = namePack.GetValue<string>("LastName");
                return new PersonVersionTwo() { Age = age, Name = firstName + " " + lastName };
            }
        }

        [DataContract]
        private class Name
        {
            [DataMember]
            public string FirstName { get; set; }

            [DataMember]
            public string LastName { get; set; }
        }
    }
}
