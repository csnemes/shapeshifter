using System;
using System.Runtime.Serialization;

namespace Shapeshifter.Core.Serialization
{
    /// <summary>
    ///     Default serializer which writes all member values with <see cref="DataMemberAttribute" /> attribute to the
    ///     <see cref="InternalPackformatWriter" />
    /// </summary>
    internal class DefaultSerializer : Serializer
    {
        private readonly SerializableTypeInfo _serializableTypeInfo;

        public DefaultSerializer(SerializableTypeInfo serializableTypeInfo)
            : base(serializableTypeInfo.Type, serializableTypeInfo.PackformatName, serializableTypeInfo.Version)
        {
            _serializableTypeInfo = serializableTypeInfo;
        }

        public override Action<InternalPackformatWriter, object> GetSerializerFunc()
        {
            return SerializeWithReflection;
        }

        private void SerializeWithReflection(InternalPackformatWriter writer, object objToWrite)
        {
            if (objToWrite == null)
            {
                throw new ArgumentNullException("objToWrite");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (_serializableTypeInfo.Type != objToWrite.GetType())
            {
                throw Exceptions.InstanceTypeDoesNotMatchSerializerType(_serializableTypeInfo.Type, objToWrite.GetType());
            }

            //write type and version
            writer.WriteProperty(Constants.TypeNameKey, _serializableTypeInfo.PackformatName);
            writer.WriteProperty(Constants.VersionKey, Convert.ToInt64(_serializableTypeInfo.Version));

            //write fields one-by-one
            foreach (var packItemCandidate in _serializableTypeInfo.Items)
            {
                writer.WriteProperty(packItemCandidate.Name, packItemCandidate.GetValueFor(objToWrite), packItemCandidate.Type);
            }
        }
    }
}