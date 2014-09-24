using System;
using System.Runtime.Serialization;

namespace Shapeshifter.SchemaComparison.Impl
{
    [DataContract]
    [Serializable]
    [KnownType(typeof(CustomSerializerInfo))]
    [KnownType(typeof(DefaultSerializerInfo))]
    internal abstract class SerializerInfo : InfoBase
    {
        protected SerializerInfo(string packformatName, uint version) : base(packformatName, version)
        {
        }
    }
}