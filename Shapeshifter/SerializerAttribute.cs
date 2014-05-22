using System;

namespace Shapeshifter
{
    /// <summary>
    /// </summary>
    /// <example>
    ///     [Serializer(typeof(MyClass), 1]
    ///     public static void SerializeMyClass(IShapeshifterWriter writer, MyClass itemToSerialize)
    ///     { ... }
    ///     Can also applied with version number only on classes to specify the used version number explicitly
    ///     [Serializer(1)]
    ///     public class MyClass
    ///     { ... }
    ///     Or use to specify explicitly the packformat name used in serialized form
    ///     [Serializer("MonClass")]
    ///     public class MyClass
    ///     { ... }
    /// </example>
    public class SerializerAttribute : Attribute
    {
        private readonly string _packformatName;
        private readonly Type _targetType;
        private uint? _version;

        public SerializerAttribute()
        {
        }

        public SerializerAttribute(Type targetType) : this(targetType, null, null)
        {
        }

        public SerializerAttribute(Type targetType, uint version) : this(targetType, targetType.Name, version)
        {
        }

        public SerializerAttribute(Type targetType, string packformatName, uint version)
            : this(targetType, packformatName, (uint?) version)
        {
        }

        public SerializerAttribute(uint version) : this(null, null, version)
        {
        }

        public SerializerAttribute(string packformatName) : this(null, packformatName, null)
        {
        }

        private SerializerAttribute(Type targetType, string packformatName, uint? version)
        {
            _targetType = targetType;
            _packformatName = packformatName;
            _version = version;
        }

        public Type TargetType
        {
            get { return _targetType; }
        }

        public string PackformatName
        {
            get { return _packformatName; }
        }

        public bool HasVersion
        {
            get { return _version.HasValue; }
        }

        public uint Version
        {
            get { return _version.HasValue ? _version.Value : default(uint); }
            set { _version = value; }
        }

        internal bool ValidOnClass
        {
            get { return true; }
        }

        internal bool ValidOnMethod
        {
            get { return _targetType != null && _version != null; }
        }
    }
}