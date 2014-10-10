using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Shapeshifter.Core;
using Shapeshifter.Core.Detection;
using Shapeshifter.Core.Serialization;

namespace Shapeshifter.Tests.Unit.Core.Serialization
{
    [TestFixture]
    public class InternalPackformatWriterTests
    {
        [Test]
        public void VersionNumber_ShouldBePresent()
        {
            var input = new TestClass() {Value = "Jenco"};

            var result = Serialize(input);

            var jobj = JObject.Parse(result);
            var version = jobj[Constants.VersionKey];
            version.Value<uint>().Should().NotBe(0);
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

        [Test]
        public void Pack_DeclaredTypeIsObject_TypeInfoIsWritten()
        {
            var result = Serialize(42, typeof(object));

            var jobj = JObject.Parse(result);
            var typeName = jobj[Constants.TypeNameKey];
            typeName.Value<string>().Should().Be("System.Int32");
        }

        [Test]
        public void Pack_DeclaredTypeIsDictionaryWithObjects_TypeInfoIsWritten()
        {
            var input = new Dictionary<object, object>{{"key", 42}};
            var result = Serialize(input, typeof(Dictionary<object,object>));

            var dictionary = JArray.Parse(result);
            var keyValuePair = dictionary.First;
            var key = keyValuePair.First;
            key[Constants.TypeNameKey].Value<string>().Should().Be("System.String");;
            var value = keyValuePair.Last;
            value[Constants.TypeNameKey].Value<string>().Should().Be("System.Int32"); ;
        }

        private static string Serialize<T>(T toPack, Type declaredType = null)
        {
            var typeContext = MetadataExplorer.CreateFor(typeof(T)).Serializers;

            var sb = new StringBuilder();
            var engine = new InternalPackformatWriter(new StringWriter(sb), typeContext);
            engine.Pack(toPack, declaredType);
            return sb.ToString();
        }

        [DataContract]
        [ShapeshifterRoot]
        private class TestClass
        {
            [DataMember]
            public string Value { get; set; }
        }
 
    }
}
