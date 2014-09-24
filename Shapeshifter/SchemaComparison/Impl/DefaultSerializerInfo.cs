using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Shapeshifter.Core;
using Shapeshifter.Utils;

namespace Shapeshifter.SchemaComparison.Impl
{
    [DataContract]
    [Serializable]
    internal class DefaultSerializerInfo : SerializerInfo
    {
        [DataMember]
        private readonly string _typeFullName;

        [DataMember]
        private readonly List<SerializedMemberInfo> _serializedMembers;

        public DefaultSerializerInfo(string packformatName, uint version, string typeFullName, IEnumerable<FieldOrPropertyMemberInfo> serializedMemberInfos) : base(packformatName, version)
        {
            _typeFullName = typeFullName;
            _serializedMembers = new List<SerializedMemberInfo>(serializedMemberInfos.EmptyIfNull().Select(minfo => new SerializedMemberInfo(minfo.Name, minfo.Type.FullName)));
        }

        public string TypeFullName
        {
            get { return _typeFullName; }
        }
    }

    [DataContract]
    [Serializable]
    internal class SerializedMemberInfo
    {
        [DataMember]
        private string _name;
        [DataMember]
        private string _type;

        public SerializedMemberInfo(string name, string type)
        {
            _name = name;
            _type = type;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Type
        {
            get { return _type; }
        }
    }
}
