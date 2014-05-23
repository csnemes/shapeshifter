using System.IO;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Shapeshifter.Core;
using Shapeshifter.Core.Detection;
using Shapeshifter.Core.Serialization;

namespace Shapeshifter.Tests.Unit.Core
{
    [TestFixture]
    public class PackformatWriterTests
    {
        [Test]
        public void VersionNumber_ShouldBePresent()
        {
            var input = new TestClass() {Value = "Jenco"};

            var result = Serialize(input);

            var jobj = JObject.Parse(result);
            var version = jobj[Constants.VersionKey];
            version.Value<uint>().Should().Be(2612302157);
        }

        [Test]
        public void TypeName_ShouldBePresent()
        {
            var input = new TestClass() { Value = "Jenco" };

            var result = Serialize(input);

            var jobj = JObject.Parse(result);
            var version = jobj[Constants.TypeNameKey];
            version.Value<string>().Should().Be("TestClass");
        }

        [Test]
        public void Properties_ShouldBePresent()
        {
            var input = new TestClass() { Value = "Jenco" };

            var result = Serialize(input);

            var jobj = JObject.Parse(result);
            var version = jobj["Value"];
            version.Value<string>().Should().Be("Jenco");
        }


        private string Serialize(object toPack)
        {
            var typeContext = MetadataExplorer.CreateFor(typeof(TestClass)).Serializers;

            var sb = new StringBuilder();
            var engine = new InternalPackformatWriter(new StringWriter(sb), typeContext);
            engine.Pack(toPack);
            return sb.ToString();
        }

        [DataContract]
        [Shapeshifter]
        private class TestClass
        {
            [DataMember]
            public string Value { get; set; }
        }
 
    }
}
