using System;

namespace Shapeshifter
{
    /// <summary>
    /// </summary>
    /// <example>
    ///     Specifies that this method serializes MyClass.
    ///     [Serializer(typeof(MyClass))]
    ///     public static void SerializeMyClass(IShapeshifterWriter writer, MyClass itemToSerialize)
    ///     { ... }
    ///
    ///     You can also specify the typename to be used in the serialized format and/or the schema version number.
    ///     [Serializer(typeof(MyClass), "MyTypename", 1)]
    ///     public static void SerializeMyClass(IShapeshifterWriter writer, MyClass itemToSerialize)
    ///     { ... }
    /// 
    ///     [Serializer(typeof(MyClass), "MyTypename")]
    ///     public static void SerializeMyClass(IShapeshifterWriter writer, MyClass itemToSerialize)
    ///     { ... }
    ///
    ///     [Serializer(typeof(MyClass), 1)]
    ///     public static void SerializeMyClass(IShapeshifterWriter writer, MyClass itemToSerialize)
    ///     { ... }
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class SerializerAttribute : Attribute
    {
        private readonly string _packformatName;
        private readonly Type _targetType;
        private readonly uint? _version;
        private bool _forAllDescendants = false;

        public SerializerAttribute(Type targetType, string packformatName, uint version)
            :this(targetType, packformatName, (uint?) version)
        {
        }

        public SerializerAttribute(Type targetType, string packformatName = null)
            : this(targetType, packformatName, null)
        {
        }

        public SerializerAttribute(Type targetType, uint version) 
            : this(targetType, null, version)
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

        public uint? Version
        {
            get { return _version; }
        }

        public bool ForAllDescendants
        {
            get { return _forAllDescendants; }
            set { _forAllDescendants = value; }
        }
    }
}