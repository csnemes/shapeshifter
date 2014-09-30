using System;

namespace Shapeshifter
{
    /// <summary>
    ///     Attribute for marking custom serializers for a given type. The serialized name and/or version can also be specified.
    ///     A custom serializer can be any static method with the required signature static void AnyName(IShapeshifterWriter writer, AnyClass itemToSerialize)
    /// </summary>
    /// <example>
    /// <code>
    ///     Specifies that this method serializes MyClass.
    ///     [Serializer(typeof(MyClass))]
    ///     public static void SerializeMyClass(IShapeshifterWriter writer, MyClass itemToSerialize)
    ///     { ... }
    ///
    ///     You can also specify the type name to be used in the serialized format and/or the schema version number.
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
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class SerializerAttribute : Attribute
    {
        private readonly string _packformatName;
        private readonly Type _targetType;
        private readonly uint? _version;
        private bool _forAllDescendants = false;

        /// <summary>
        /// Creates an instance of the attribute for the given targetType, serialized name and version
        /// </summary>
        public SerializerAttribute(Type targetType, string packformatName, uint version)
            :this(targetType, packformatName, (uint?) version)
        {
        }

        /// <summary>
        /// Creates an instance of the attribute for the given targetType and optional serialized name
        /// </summary>
        public SerializerAttribute(Type targetType, string packformatName = null)
            : this(targetType, packformatName, null)
        {
        }

        /// <summary>
        /// Creates an instance of the attribute for the given targetType and version
        /// </summary>
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

        /// <summary>
        /// Returns the type targeted by the custom serializer marked by this attribute
        /// </summary>
        public Type TargetType
        {
            get { return _targetType; }
        }

        /// <summary>
        /// Returns the serialized type name used by the custom serializer marked by this attribute
        /// </summary>
        public string PackformatName
        {
            get { return _packformatName; }
        }

        /// <summary>
        /// Returns the version used by the custom serializer marked by this attribute
        /// </summary>
        public uint? Version
        {
            get { return _version; }
        }

        /// <summary>
        /// Specifies if the marked custom serializer method should be used for all descendants of the given target type.
        /// Descendant detection uses the descendantSearchScope specified when creating <see cref="Shapeshifter"/>.
        /// </summary>
        public bool ForAllDescendants
        {
            get { return _forAllDescendants; }
            set { _forAllDescendants = value; }
        }
    }
}