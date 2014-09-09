using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Shapeshifter.Core.Converters;
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
        public Shapeshifter(IEnumerable<Assembly> descendantSearchScope = null) 
            : base(typeof (T), descendantSearchScope)
        {
        }

        public Shapeshifter(IEnumerable<Type> knownTypes, IEnumerable<Assembly> descendantSearchScope = null)
            : base(typeof(T), knownTypes, descendantSearchScope)
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
        private readonly IEnumerable<Type> _builtInKnownTypes = new List<Type>
        {
           typeof (EnumConverter)
        };

        private readonly Lazy<MetadataExplorer> _metadata;
        private readonly Type _targetType;

        public Shapeshifter(Type type, IEnumerable<Assembly> descendantSearchScope = null)
            : this(type, new Type[0], descendantSearchScope)
        {
        }

        public Shapeshifter(Type type, IEnumerable<Type> knownTypes, IEnumerable<Assembly> descendantSearchScope = null)
        {
            _targetType = type;

            var rootTypesToCheck = new List<Type> { type };
            var knownTypesToCheck = _builtInKnownTypes.Union(knownTypes ?? Enumerable.Empty<Type>());

            _metadata = new Lazy<MetadataExplorer>(() => MetadataExplorer.CreateFor(rootTypesToCheck, knownTypesToCheck, descendantSearchScope));
        }

        private SerializerCollection Serializers
        {
            get { return _metadata.Value.Serializers; }
        }

        private DeserializerCollection Deserializers
        {
            get { return _metadata.Value.Deserializers; }
        }

        public void Serialize(Stream targetStream, object objToPack, Type declaredSourceType = null)
        {
            var textWriter = new StreamWriter(targetStream);
            var packerEngine = new InternalPackformatWriter(textWriter, Serializers);
            packerEngine.Pack(objToPack, declaredSourceType);
            textWriter.Flush();
        }

        public string Serialize(object objToPack, Type declaredSourceType = null)
        {
            using (var writer = new StringWriter(new StringBuilder()))
            {
                var packerEngine = new InternalPackformatWriter(writer, Serializers);
                packerEngine.Pack(objToPack, declaredSourceType);
                return writer.GetStringBuilder().ToString();
            }
        }

        public object Deserialize(Stream sourceStream)
        {
            var reader = new StreamReader(sourceStream);
            var unpackerEngine = new InternalPackformatReader(reader, Deserializers);
            return unpackerEngine.Unpack(_targetType);
        }

        public object Deserialize(string source)
        {
            using (var reader = new StringReader(source))
            {
                var unpackerEngine = new InternalPackformatReader(reader, Deserializers);
                return unpackerEngine.Unpack(_targetType);
            }
        }
    }
}