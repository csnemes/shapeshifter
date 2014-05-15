using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Shapeshifter.Core
{
    /// <summary>
    ///     Class that reads from the underlying <see cref="TextReader" /> containing object in PackFormat. The class useses
    ///     JsonReader internaly to
    ///     read from the TextReader.
    /// </summary>
    internal class PackformatReader : IDisposable
    {
        private readonly ConversionHelpers _conversionHelpers;
        private readonly JsonReader _reader;
        private readonly DeserializationCandidatesCollection _typeContext;

        public PackformatReader(TextReader reader, DeserializationCandidatesCollection typeContext) :
            this(reader, typeContext, new List<IPackformatSurrogateConverter>())
        {
        }

        public PackformatReader(TextReader reader, DeserializationCandidatesCollection typeContext,
            IEnumerable<IPackformatSurrogateConverter> customConverters)
        {
            _reader = new JsonTextReader(reader);
            _typeContext = typeContext;
            _conversionHelpers = new ConversionHelpers(new ConvertersCollection(customConverters));
        }

        public PackformatReader(string source, DeserializationCandidatesCollection typeContext)
            : this(new StringReader(source), typeContext, new List<IPackformatSurrogateConverter>())
        {
        }

        public PackformatReader(string source, DeserializationCandidatesCollection typeContext,
            IEnumerable<IPackformatSurrogateConverter> customConverters)
            : this(new StringReader(source), typeContext, customConverters)
        {
        }

        public object Unpack<T>()
        {
            object result = MatchValue();
            return _conversionHelpers.ConvertValueToTargetType<T>(result);
        }

        public object Unpack(Type targetType)
        {
            object result = MatchValue();
            return _conversionHelpers.ConvertValueToTargetType(targetType, result);
        }

        private object MatchValue(bool skipRead = false)
        {
            if (!skipRead)
            {
                if (!_reader.Read()) throw new UnexpectedEndOfTokenStreamException();
            }

            //TODO skip comments

            switch (_reader.TokenType)
            {
                case (JsonToken.Null):
                {
                    return null;
                }
                case (JsonToken.String):
                case (JsonToken.Integer):
                case (JsonToken.Boolean):
                case (JsonToken.Float):
                case (JsonToken.Date):
                {
                    return _reader.Value;
                }
                case (JsonToken.Bytes):
                {
                    throw new NotImplementedException();
                }
                case (JsonToken.StartObject):
                {
                    return MatchObject();
                }
                case (JsonToken.StartArray):
                {
                    return MatchArray();
                }
                default:
                {
                    throw Exceptions.UnexpectedTokenEncountered(_reader.TokenType);
                }
            }
        }

        private object MatchArray()
        {
            var result = new List<object>();
            while (_reader.Read() && _reader.TokenType != JsonToken.EndArray)
            {
                result.Add(MatchValue(true));
            }
            return result;
        }

        private object MatchObject()
        {
            var elements = new Dictionary<string, object>();
            while (_reader.Read() && _reader.TokenType != JsonToken.EndObject)
            {
                if (_reader.TokenType == JsonToken.Comment) continue;
                if (_reader.TokenType != JsonToken.PropertyName)
                {
                    throw Exceptions.UnexpectedTokenEncountered(_reader.TokenType);
                }

                var key = (string) _reader.Value;
                object value = MatchValue();
                elements.Add(key, value);
            }

            var objectProperties = new ObjectProperties(elements);

            //build up object 
            if (objectProperties.IsInPackformat)
            {
                DeserializerCandidate typeUnpacker = _typeContext.ResolveDeserializer(objectProperties.TypeName,
                    objectProperties.Version);

                //if we don't have a real builder we'll throw an exception when someone tries to get the data
                Func<ObjectProperties, ConversionHelpers, object> builderFunc = (typeUnpacker == null
                    ? null
                    : typeUnpacker.GetDeserializerFunc()) ?? ((inp, conv) =>{
                                                                                throw Exceptions.CannotFindDeserializer(objectProperties);
                                                                            });

                return new ObjectInPackedForm(objectProperties, builderFunc, _conversionHelpers);
            }
            //TODO handle the case when no name and/or version is present  - some JSON not serialized with shapeshifter
            throw Exceptions.InvalidInput();
        }

        #region IDisposable implementation

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Dispose managed resources.
                if (_reader != null) ((IDisposable) _reader).Dispose();
            }

            // Clean up unmanaged resources here. 

            _disposed = true;
        }

        ~PackformatReader()
        {
            Dispose(false);
        }

        #endregion
    }
}