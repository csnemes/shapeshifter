using System;

namespace Shapeshifter.Core
{
    /// <summary>
    ///     This class uses reflection to serialize members for a given type
    /// </summary>
    internal class ReflectionInstanceSerializer
    {
        private readonly TypeInfo _typeInfo;

        public ReflectionInstanceSerializer(TypeInfo typeInfo)
        {
            _typeInfo = typeInfo;
        }

        public void Serialize(IPackformatWriter writer, object objToWrite)
        {
            if (objToWrite == null)
            {
                throw new ArgumentNullException("objToWrite");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (_typeInfo.Type != objToWrite.GetType())
            {
                throw Exceptions.InstanceTypeDoesNotMatchSerializerType(_typeInfo.Type, objToWrite.GetType());
            }

            //write type and version
            writer.WriteProperty(Constants.TypeNameKey, _typeInfo.PackformatName);
            writer.WriteProperty(Constants.VersionKey, Convert.ToInt64(_typeInfo.Version));

            //write fields one-by-one
            foreach (SerializableTypeMemberInfo packItemCandidate in _typeInfo.Items)
            {
                writer.WriteProperty(packItemCandidate.Name, packItemCandidate.GetValueFor(objToWrite));
            }
        }
    }
}