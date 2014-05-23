using System;

namespace Shapeshifter
{
    /// <summary>
    ///     Attribute for marking static helper methods as deserializers of a given type and version.
    /// </summary>
    /// <remarks>
    ///     If it is applied to a static method it means that for the defined name and version this method will be called during
    ///     deserialization.
    ///     The method must be static and the signature must conform to object MyMethod(IShapeshifterReader reader).
    ///     Multiple DeserializerAttributes can be applied to a single method.
    ///     For the attribute a name and a version can be specified. The name is the name of the class in the serialized data
    ///     and the version is its serialized version
    ///     Versions are calculated automatically by Shapeshifter or defined by the <see cref="SerializerAttribute" /> or <see cref="ShapeshifterAttribute" />.
    ///     If no version is specified on a method that deserializer becomes the default, which means that if no version
    ///     specific deserializer is found it will be used. Only one default deserializer can be defined for a given name.
    /// </remarks>
    /// <example>
    ///     [Deserializer(typeof(MyClass))]
    ///     public static object DeserializerForMyClass(IShapeshifterReader reader)
    ///     {}
    ///
    ///     [Deserializer("MyOldClass")]
    ///     public static object DeserializerForAllOldVersions(IShapeshifterReader reader)
    ///     {}
    ///
    ///     [Deserializer("MyOldClass", 32456)]
    ///     [Deserializer("MyOldClass", 67890)]
    ///     public static object DeserializerForSpecifiedOldVersions(IShapeshifterReader reader)
    ///     {}
    /// 
    ///     [Deserializer(typeof(MyBase), ForAllDescendants = true)]
    ///     public static object DeserializerForAllDescendants(IShapeshifterReader reader, Type targetType)
    ///     {}
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class DeserializerAttribute : Attribute
    {
        private readonly Type _targeType;
        private readonly string _packformatName;
        private uint _version;
        private bool _forAllDescendants = false;

        public DeserializerAttribute(Type targetType, uint version = 0) 
            : this(targetType.Name, version)
        {
            _targeType = targetType;
        }

        public DeserializerAttribute(string packformatName, uint version = 0)
        {
            _packformatName = packformatName;
            _version = version;
        }

        public Type TargeType
        {
            get { return _targeType; }
        }

        public string PackformatName
        {
            get { return _packformatName; }
        }

        public bool IsVersionSpecified
        {
            get { return _version != 0; }
        }

        public uint Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public bool ForAllDescendants
        {
            get { return _forAllDescendants; }
            set { _forAllDescendants = value; }
        }
    }
}