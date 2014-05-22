using System;
using System.Collections;
using System.IO;
using Newtonsoft.Json;

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

        public void WriteProperty(string propertyKey, object propertyValue)
        {
            _writer.WritePropertyName(propertyKey);
            WriteValue(propertyValue);
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

        public void Pack(object objToPack)
        {
            WriteValue(objToPack);
        }

        private void WriteValue(object obj)
        {
            if (obj == null)
            {
                _writer.WriteNull();
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
            else if (obj is IEnumerable)
            {
                WriteArray((IEnumerable) obj);
            }
            else
            {
                WriteObject(obj);
            }
        }

        private void WriteArray(IEnumerable arr)
        {
            _writer.WriteStartArray();

            foreach (object arrItem in arr)
            {
                WriteValue(arrItem);
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