using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Shapeshifter.Core;

namespace Shapeshifter.Tests.Unit
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
            var typeContext = PackformatCandidatesDetector.CreateFor(typeof(TestClass)).SerializationCandidates;

            var sb = new StringBuilder();
            var engine = new PackformatWriter(new StringWriter(sb), typeContext);
            engine.Pack(toPack);
            return sb.ToString();
        }

        [DataContract]
        [Serializer]
        private class TestClass
        {
            [DataMember]
            public string Value { get; set; }
        }
 
    }
}
