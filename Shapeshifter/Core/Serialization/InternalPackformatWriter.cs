using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Shapeshifter.Core.Serialization
{
    /// <summary>
    ///     Writes an object to the given <see cref="TextWriter" /> in its packed format. It uses the underlying JsonWriter to
    ///     emit Json compatible
    ///     structures
    /// </summary>
    internal class InternalPackformatWriter : IDisposable
    {
        private readonly SerializerCollection _serializers;
        private readonly JsonWriter _writer;

        public InternalPackformatWriter(TextWriter writer, SerializerCollection serializers)
        {
            _writer = new JsonTextWriter(writer);
            _serializers = serializers;
        }

        public void WriteProperty(string propertyKey, object propertyValue, Type declaredSourceType = null)
        {
            _writer.WritePropertyName(propertyKey);
            WriteValue(propertyValue, declaredSourceType);
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
                if (_writer != null) ((IDisposable) _writer).Dispose();
            }

            // Clean up unmanaged resources here. 

            _disposed = true;
        }

        ~InternalPackformatWriter()
        {
            Dispose(false);
        }

        #endregion

        public void Pack(object objToPack, Type declaredSourceType = null)
        {
            WriteValue(objToPack, declaredSourceType);
        }

        private void WriteValueWithTypeInformation(object obj)
        {
            if (obj == null)
            {
                _writer.WriteNull();
                return;
            }

            if (obj is string || 
                obj is int ||
                obj is long || 
                obj is short || 
                obj is uint || 
                obj is ulong || 
                obj is ushort || 
                obj is bool || 
                obj is float || 
                obj is double || 
                obj is decimal || 
                obj is char || 
                obj is byte || 
                obj is sbyte || 
                obj is DateTime || 
                obj is DateTimeOffset ||
                obj is Guid)
            {
                _writer.WriteStartObject();
                _writer.WritePropertyName(Constants.TypeNameKey);
                _writer.WriteValue(obj.GetType().FullName);
                _writer.WritePropertyName(Constants.ValueKey);
                WriteValue(obj);
                _writer.WriteEndObject();
                //Will look like:
                // myValue : { 
                //              __type : System.String
                //              __val : "dfsdfsdf"    
                //           }
            }
            else
            {
                WriteObject(obj);
            }
        }

        private void WriteValue(object obj, Type declaredSourceType = null)
        {
            if (obj == null)
            {
                _writer.WriteNull();
                return;
            }

            if (declaredSourceType == typeof (object))
            {
                WriteValueWithTypeInformation(obj);
                return;
            }

            if (obj is string)
            {
                _writer.WriteValue((string) obj);
            }
            else if (obj is int)
            {
                _writer.WriteValue((int) obj);
            }
            else if (obj is long)
            {
                _writer.WriteValue((long) obj);
            }
            else if (obj is short)
            {
                _writer.WriteValue((short) obj);
            }
            else if (obj is uint)
            {
                _writer.WriteValue((uint) obj);
            }
            else if (obj is ulong)
            {
                _writer.WriteValue((ulong) obj);
            }
            else if (obj is ushort)
            {
                _writer.WriteValue((ushort) obj);
            }
            else if (obj is bool)
            {
                _writer.WriteValue((bool) obj);
            }
            else if (obj is float)
            {
                _writer.WriteValue((float) obj);
            }
            else if (obj is double)
            {
                _writer.WriteValue((double) obj);
            }
            else if (obj is decimal)
            {
                _writer.WriteValue((decimal) obj);
            }
            else if (obj is char)
            {
                _writer.WriteValue((char) obj);
            }
            else if (obj is byte)
            {
                _writer.WriteValue((byte) obj);
            }
            else if (obj is sbyte)
            {
                _writer.WriteValue((sbyte) obj);
            }
            else if (obj is DateTime)
            {
                _writer.WriteValue((DateTime) obj);
            }
            else if (obj is DateTimeOffset)
            {
                _writer.WriteValue((DateTimeOffset) obj);
            }
            else if (obj is Guid)
            {
                _writer.WriteValue(((Guid) obj).ToString());
            }
            else if (obj.GetType().IsConstructedFromOpenGeneric(typeof (KeyValuePair<,>)))
            {
                Type declaredKeyType = null;
                Type declaredValueType = null;

                if (declaredSourceType != null)
                {
                    var genericArguments = declaredSourceType.GetGenericArguments();
                    declaredKeyType = genericArguments[0];
                    declaredValueType = genericArguments[1];
                }

                WriteKeyValuePair(obj, declaredKeyType, declaredValueType);
            }
            else if (obj is IEnumerable)
            {
                Type declaredElementType = null;

                if (declaredSourceType != null)
                {
                    var genericEnumerableInterface = declaredSourceType.GetInterfaces()
                        .FirstOrDefault(i => i.IsConstructedFromOpenGeneric(typeof (IEnumerable<>)));
                    if (genericEnumerableInterface != null)
                    {
                        declaredElementType = genericEnumerableInterface.GetGenericArguments().First();
                    }
                }
                WriteArray((IEnumerable) obj, declaredElementType);
            }
            else
            {
                WriteObject(obj);
            }
        }

        private void WriteKeyValuePair(object keyValuePair, Type declaredKeyType = null, Type declaredValueType = null)
        {
            var type = keyValuePair.GetType();

            var keyPropInfo = type.GetProperty("Key");
            var keyVal = keyPropInfo.GetValue(keyValuePair, null);

            var valuePropInfo = type.GetProperty("Value");
            var valueVal = valuePropInfo.GetValue(keyValuePair, null);

            _writer.WriteStartArray();
            WriteValue(keyVal, declaredKeyType);
            WriteValue(valueVal, declaredValueType);
            _writer.WriteEndArray();
        }

        private void WriteArray(IEnumerable arr, Type declaredSourceType = null)
        {
            _writer.WriteStartArray();

            foreach (object arrItem in arr)
            {
                WriteValue(arrItem, declaredSourceType);
            }

            _writer.WriteEndArray();
        }

        private void WriteObject(object obj)
        {
            _writer.WriteStartObject();

            var type = obj.GetType();
            var serializer = _serializers.ResolveSerializer(type);
            serializer.GetSerializerFunc()(this, obj);

            _writer.WriteEndObject();
        }
    }
}