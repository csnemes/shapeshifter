using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Shapeshifter.Core;

namespace Shapeshifter
{
    /// <summary>
    ///     Serializer and deserializer for the given type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    ///     See UnitTests for usage.
    /// </remarks>
    public class ShapeshifterSerializer<T> : ShapeshifterSerializer, IShapeshifterSerializer<T>
    {
        public ShapeshifterSerializer() : base(typeof (T))
        {
        }

        public ShapeshifterSerializer(IEnumerable<Type> knownTypes) : base(typeof (T), knownTypes)
        {
        }

        public ShapeshifterSerializer(IEnumerable<Type> knownTypes,
            IEnumerable<IPackformatSurrogateConverter> customConverters)
            : base(typeof (T), knownTypes, customConverters)
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

    public class ShapeshifterSerializer : IShapeshifterSerializer
    {
        private readonly List<IPackformatSurrogateConverter> _customConverters;
        private readonly Lazy<PackformatCandidatesDetector> _serializationCandidates;
        private readonly Type _targetType;

        public ShapeshifterSerializer(Type type) : this(type, new Type[0])
        {
        }

        public ShapeshifterSerializer(Type type, IEnumerable<Type> knownTypes)
            : this(type, knownTypes, new IPackformatSurrogateConverter[0])
        {
        }

        public ShapeshifterSerializer(Type type, IEnumerable<Type> knownTypes,
            IEnumerable<IPackformatSurrogateConverter> customConverters)
        {
            _targetType = type;
            _customConverters = new List<IPackformatSurrogateConverter>(customConverters);

            var rootTypesToCheck = new List<Type>() { type };
            var knownTypesToCheck = new List<Type>((knownTypes ?? new Type[0]));
            knownTypesToCheck.AddRange(GetSurrogateTypesForConverters(_customConverters));

            _serializationCandidates = new Lazy<PackformatCandidatesDetector>(
                () => PackformatCandidatesDetector.CreateFor(rootTypesToCheck, knownTypesToCheck));
        }

        private IEnumerable<Type> GetSurrogateTypesForConverters(IEnumerable<IPackformatSurrogateConverter> customConverters)
        {
            foreach (var typeInspector in customConverters.Select(t => new TypeInspector(t.GetType())))
            {
                if (!typeInspector.HasKnownTypeAttribute)
                {
                    throw Exceptions.KnownTypeAttributeMissingFromSurrogateConverter(typeInspector.Type);
                }

                foreach (var knownType in typeInspector.GetKnownTypes())
                {
                    yield return knownType;
                }
            }
        }


        private SerializationCandidatesCollection SerializationCandidatesCollection
        {
            get { return _serializationCandidates.Value.SerializationCandidates; }
        }

        private DeserializationCandidatesCollection DeserializationCandidatesCollection
        {
            get { return _serializationCandidates.Value.DeserializationCandidates; }
        }

        public void Serialize(Stream targetStream, object objToPack)
        {
            var textWriter = new StreamWriter(targetStream);
            var packerEngine = new PackformatWriter(textWriter, SerializationCandidatesCollection, _customConverters);
            packerEngine.Pack(objToPack);
        }

        public string Serialize(object objToPack)
        {
            var writer = new StringWriter(new StringBuilder());
            var packerEngine = new PackformatWriter(writer, SerializationCandidatesCollection, _customConverters);
            packerEngine.Pack(objToPack);
            return writer.GetStringBuilder().ToString();
        }

        public object Deserialize(Stream sourceStream)
        {
            var reader = new StreamReader(sourceStream);
            var unpackerEngine = new PackformatReader(reader, DeserializationCandidatesCollection, _customConverters);
            return unpackerEngine.Unpack(_targetType);
        }

        public object Deserialize(string source)
        {
            var reader = new StringReader(source);
            var unpackerEngine = new PackformatReader(reader, DeserializationCandidatesCollection, _customConverters);
            return unpackerEngine.Unpack(_targetType);
        }
    }
}