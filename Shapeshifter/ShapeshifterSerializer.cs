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
    ///     Shapeshifter serializer and deserializer for the given type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    ///     See UnitTests for usage.
    /// </remarks>
    public class ShapeshifterSerializer<T> : ShapeshifterSerializer
    {
        /// <summary>
        /// Creates a Shapeshifter serializer for the given type T. Additional assemblies can be specified as descendant search scopes. They will be used if 
        /// ForAllDescendants property is set to true on <see cref="SerializerAttribute"/> or on <see cref="DeserializerAttribute"/>.
        /// </summary>
        /// <param name="descendantSearchScope">List of assemblies to search for descendant classes</param>
        public ShapeshifterSerializer(IEnumerable<Assembly> descendantSearchScope = null) 
            : base(typeof (T), descendantSearchScope)
        {
        }

        /// <summary>
        /// Creates a Shapeshifter serializer for the given type T. Additional known types descendant search scope assemblies can be specified. 
        /// Specified known types will used during serialization and deserialization.
        /// Search scopes will be used if ForAllDescendants property is set to true on <see cref="SerializerAttribute"/> or on <see cref="DeserializerAttribute"/>.
        /// </summary>
        /// <param name="knownTypes">List of known types</param>
        /// <param name="descendantSearchScope">List of assemblies to search for descendant classes</param>
        public ShapeshifterSerializer(IEnumerable<Type> knownTypes, IEnumerable<Assembly> descendantSearchScope = null)
            : base(typeof(T), knownTypes, descendantSearchScope)
        {
        }

        /// <summary>
        /// Deserializes an instance of T from the given stream.
        /// </summary>
        /// <param name="sourceStream">Input stream with the serialized data</param>
        /// <returns>Deserialized instance</returns>
        public new T Deserialize(Stream sourceStream)
        {
            return (T) base.Deserialize(sourceStream);
        }

        /// <summary>
        /// Deserializes an instance of T from the given string.
        /// </summary>
        /// <param name="source">Input string with the serialized data</param>
        /// <returns>Deserialized instance</returns>
        public new T Deserialize(string source)
        {
            return (T) base.Deserialize(source);
        }
    }

    /// <summary>
    ///     Shapeshifter serializer and deserializer
    /// </summary>
    public class ShapeshifterSerializer
    {
        private readonly IEnumerable<Type> _builtInKnownTypes = new List<Type>
        {
           typeof (EnumConverter)
        };

        private readonly Type _targetType;
        private readonly MetadataExplorer _metadata;

        /// <summary>
        /// Creates a Shapeshifter serializer for the given type. Additional assemblies can be specified as descendant search scopes. They will be used if 
        /// ForAllDescendants property is set to true on <see cref="SerializerAttribute"/> or on <see cref="DeserializerAttribute"/>.
        /// </summary>
        /// <param name="type">Type to serialize or deserialize.</param>
        /// <param name="descendantSearchScope">List of assemblies to search for descendant classes</param>
        public ShapeshifterSerializer(Type type, IEnumerable<Assembly> descendantSearchScope = null)
            : this(type, new Type[0], descendantSearchScope)
        {
        }

        /// <summary>
        /// Creates a Shapeshifter serializer for the given type. Additional known types descendant search scope assemblies can be specified. 
        /// Specified known types will used during serialization and deserialization.
        /// Search scopes will be used if ForAllDescendants property is set to true on <see cref="SerializerAttribute"/> or on <see cref="DeserializerAttribute"/>.
        /// </summary>
        /// <param name="type">Type to serialize or deserialize.</param>
        /// <param name="knownTypes">List of known types</param>
        /// <param name="descendantSearchScope">List of assemblies to search for descendant classes</param>
        public ShapeshifterSerializer(Type type, IEnumerable<Type> knownTypes, IEnumerable<Assembly> descendantSearchScope = null)
        {
            _targetType = type;

            var rootTypesToCheck = new List<Type> { type };
            var knownTypesToCheck = _builtInKnownTypes.Union(knownTypes ?? Enumerable.Empty<Type>());

            _metadata = MetadataExplorer.CreateFor(rootTypesToCheck, knownTypesToCheck, descendantSearchScope);
        }

        private SerializerCollection Serializers
        {
            get { return _metadata.Serializers; }
        }

        private DeserializerCollection Deserializers
        {
            get { return _metadata.Deserializers; }
        }

        /// <summary>
        /// Serializes the given object to the target stream.
        /// </summary>
        /// <param name="targetStream">Stream to write the serialized form.</param>
        /// <param name="objToPack">Object to be serialized.</param>
        public void Serialize(Stream targetStream, object objToPack)
        {
            var textWriter = new StreamWriter(targetStream);
            var packerEngine = new InternalPackformatWriter(textWriter, Serializers);
            packerEngine.Pack(objToPack);
            textWriter.Flush();
        }

        /// <summary>
        /// Serializes the given object to a string.
        /// </summary>
        /// <param name="objToPack">Object to be serialized.</param>
        /// <returns>Serialized object as string</returns>
        public string Serialize(object objToPack)
        {
            using (var writer = new StringWriter(new StringBuilder()))
            {
                var packerEngine = new InternalPackformatWriter(writer, Serializers);
                packerEngine.Pack(objToPack);
                return writer.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Deserializes an instance of type specified at object construction from the given stream.
        /// </summary>
        /// <param name="sourceStream">Input stream with the serialized data</param>
        /// <returns>Deserialized instance</returns>
        public object Deserialize(Stream sourceStream)
        {
            var reader = new StreamReader(sourceStream);
            var unpackerEngine = new InternalPackformatReader(reader, Deserializers);
            return unpackerEngine.Unpack(_targetType);
        }

        /// <summary>
        /// Deserializes an instance of type specified at object construction from the given string.
        /// </summary>
        /// <param name="source">Input string with the serialized data</param>
        /// <returns>Deserialized instance</returns>
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