using System;
using System.Collections.Generic;

namespace Shapeshifter.Core
{
    /// <summary>
    ///     Data holder for type related basic info
    /// </summary>
    internal class SerializableTypeInfo
    {
        private readonly List<SerializableMemberInfo> _items;
        private readonly string _packformatName;
        private readonly Type _type;
        private readonly uint _version;

        public SerializableTypeInfo(Type type, string packformatName, uint version, IEnumerable<SerializableMemberInfo> items)
        {
            _type = type;
            _packformatName = packformatName;
            _version = version;
            _items = new List<SerializableMemberInfo>(items);
        }

        public Type Type
        {
            get { return _type; }
        }

        public string PackformatName
        {
            get { return _packformatName; }
        }

        public uint Version
        {
            get { return _version; }
        }

        public IEnumerable<SerializableMemberInfo> Items
        {
            get { return _items; }
        }
    }
}