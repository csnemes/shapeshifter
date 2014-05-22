using System;

namespace Shapeshifter
{
    /// <summary>
    ///     Attribute primarly used for marking static helper methods as deserializers of a given type and version.
    ///     It can also be used on classes to register the class default deserializer for a given type and/or version.
    /// </summary>
    /// <remarks>
    ///     If this attribute applied to a class it means that for the defined name and version the default deserializer of the
    ///     class will
    ///     be used (good for name changes).
    ///     If it applied to a static method it means that for the defined name and version this method will be called during
    ///     deserialization.
    ///     The method must be static and the signature must conform to object MyMethod(IShapeshifterReader reader).
    ///     Multiple DeserializerAttributes can be applied to a single class or method
    ///     For the attribute a name and a version can be specified. The name is the name of the class in the serialized data
    ///     and the version is its serialized version
    ///     Versions are calculated automatically by Shapeshifter or defined by the <see cref="SerializerAttribute" />.
    ///     If no version is specified on a method that deserializer becomes the default, which means that if no version
    ///     specific deserializer
    ///     is found it will be used. Only one default deserializer can be defined for a given name.
    ///     If no name is specified (valid only for attribute on a class) the class name will become the name.
    ///     The system will automatically add a deserializer for the class name and the current version (no attribute is
    ///     necessary).
    /// </remarks>
    /// <example>
    ///     [Deserializer("MyOldClass")]
    ///     public class MyClass
    ///     would tell that a serialized MyOldClass with the same signature should be deserialized with the MyClass default
    ///     deserializer.
    ///     Usefull for name changes.
    ///     [Deserializer("MyOldClass", 456734)]
    ///     public class MyClass
    ///     {
    ///     [Deserializer("MyOldClass", 32456)]
    ///     [Deserializer("MyOldClass", 67890)]
    ///     public object DeserializerForOldVersions(IShapeshifterReader reader)
    ///     {}
    ///     [Deserializer("MyVeryOldClass")]
    ///     public object DeserializerForVeryOldVersions(IShapeshifterReader reader)
    ///     {}
    ///     }
    /// </example>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class DeserializerAttribute : Attribute
    {
        private readonly string _packformatName;
        private uint _version;

        public DeserializerAttribute(Type targetType, uint version = 0) : this(targetType.Name, version)
        {
        }

        public DeserializerAttribute(string packformatName, uint version = 0)
        {
            _packformatName = packformatName;
            _version = version;
        }

        public string PackformatName
        {
            get { return _packformatName; }
        }

        public uint Version
        {
            get { return _version; }
            set { _version = value; }
        }

        internal bool ValidOnClass
        {
            get { return _packformatName != null && _version != 0; }
        }

        internal bool ValidOnMethod
        {
            get { return _packformatName != null; }
        }
    }
}