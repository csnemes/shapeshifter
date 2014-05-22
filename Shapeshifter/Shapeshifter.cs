using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Shapeshifter.Core.Deserialization;
using Shapeshifter.Core.Detection;
using Shapeshifter.Core.Serialization;

namespace Shapeshifter
{
    /// <summary>
    ///     Serializer and deserializer for the given type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    ///     See UnitTests for usage.
    /// </remarks>
    public class Shapeshifter<T> : Shapeshifter, IShapeshifter<T>
    {
        public Shapeshifter() : base(typeof (T))
        {
        }

        public Shapeshifter(IEnumerable<Type> knownTypes) : base(typeof (T), knownTypes)
        {
        }

        public new T Deserialize(Stream sourceStream)
        {
            return (T) base.Deserialize(sourceStream);
        }

        public new T Deserialize(string source)
        {
            return (T) base.Deserialize(source);
        }
    }

    public class Shapeshifter : IShapeshifter
    {
        private readonly Lazy<MetadataExplorer> _metadata;
        private readonly Type _targetType;

        public Shapeshifter(Type type) : this(type, new Type[0])
        {
        }

        public Shapeshifter(Type type, IEnumerable<Type> knownTypes)
        {
            _targetType = type;

            var rootTypesToCheck = new List<Type>() { type };
            var knownTypesToCheck = new List<Type>((knownTypes ?? new Type[0]));

            _metadata = new Lazy<MetadataExplorer>(() => MetadataExplorer.CreateFor(rootTypesToCheck, knownTypesToCheck));
        }

        private SerializerCollection Serializers
        {
            get { return _metadata.Value.Serializers; }
        }

        private DeserializerCollection Deserializers
        {
            get { return _metadata.Value.Deserializers; }
        }

        public void Serialize(Stream targetStream, object objToPack)
        {
            var textWriter = new StreamWriter(targetStream);
            var packerEngine = new InternalPackformatWriter(textWriter, Serializers);
            packerEngine.Pack(objToPack);
        }

        public string Serialize(object objToPack)
        {
            var writer = new StringWriter(new StringBuilder());
            var packerEngine = new InternalPackformatWriter(writer, Serializers);
            packerEngine.Pack(objToPack);
            return writer.GetStringBuilder().ToString();
        }

        public object Deserialize(Stream sourceStream)
        {
            var reader = new StreamReader(sourceStream);
            var unpackerEngine = new InternalPackformatReader(reader, Deserializers);
            return unpackerEngine.Unpack(_targetType);
        }

        public object Deserialize(string source)
        {
            var reader = new StringReader(source);
            var unpackerEngine = new InternalPackformatReader(reader, Deserializers);
            return unpackerEngine.Unpack(_targetType);
        }
    }
}