using System;
using System.Collections.Generic;

namespace Shapeshifter.Core
{
    /// <summary>
    ///     Data holder for type related basic info
    /// </summary>
    internal class TypeInfo
    {
        private readonly List<SerializableTypeMemberInfo> _items;
        private readonly string _packformatName;
        private readonly Type _type;
        private readonly uint _version;

        public TypeInfo(Type type, string packformatName, uint version, IEnumerable<SerializableTypeMemberInfo> items)
        {
            _type = type;
            _packformatName = packformatName;
            _version = version;
            _items = new List<SerializableTypeMemberInfo>(items);
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

        public IEnumerable<SerializableTypeMemberInfo> Items
        {
            get { return _items; }
        }
    }
}