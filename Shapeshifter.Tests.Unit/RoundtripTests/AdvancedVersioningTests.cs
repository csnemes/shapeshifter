using FluentAssertions;
using NUnit.Framework;
using System.Runtime.Serialization;

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

        private Shapeshifter<T> GetSerializer<T>()
        {
            return new Shapeshifter<T>();
        }

        [DataContract]
        [Shapeshifter]
        private class PersonVersionOne
        {
            [DataMember]
            public Name Name { get; set; }
            [DataMember]
            public int Age { get; set; }
        }

        [DataContract]
        [Shapeshifter]
        private class PersonVersionTwo //droping a class 
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public int Age { get; set; }

            [Deserializer("PersonVersionOne", 2830146994)]
            private static object TransformVersionOne(IShapeshifterReader reader)
            {
                var age = reader.Read<int>("Age");
                var namePack = reader.GetReader("Name");
                var firstName = namePack.Read<string>("FirstName");
                var lastName = namePack.Read<string>("LastName");
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
