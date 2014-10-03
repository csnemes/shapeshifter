using System;
using Shapeshifter.Core;

namespace Shapeshifter
{
    /// <summary>
    ///     Attribute for marking custom deserializers of a given type and version. A custom deserializer can be any static method with the required signature 
    ///     static object AnyName(IShapeshifterReader reader). 
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
    /// <code>
    ///     [Deserializer(typeof(MyClass), 56789)]
    ///     public static object DeserializerForMyClass(IShapeshifterReader reader)
    ///     {}
    ///
    ///     [Deserializer("MyOldClass", 12345)]
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
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class DeserializerAttribute : Attribute
    {
        private readonly Type _targeType;
        private readonly string _packformatName;
        private readonly uint? _version;
        private bool _forAllDescendants = false;

        /// <summary>
        /// Creates an instance of the attribute with the specified targetType
        /// </summary>
        public DeserializerAttribute(Type targetType)
            : this(targetType, targetType.GetPrettyName(), null)
        {
        }

        /// <summary>
        /// Creates an instance of the attribute with the specified targetType and version
        /// </summary>
        public DeserializerAttribute(Type targetType, uint version)
            : this(targetType, targetType.GetPrettyName(), version)
        {
        }

        /// <summary>
        /// Creates an instance of the attribute with the specified serialized type name and version
        /// </summary>
        public DeserializerAttribute(string packformatName, uint version)
            : this(null, packformatName, version)
        {
        }

        private DeserializerAttribute(Type targetType, string packformatName, uint? version)
        {
            _targeType = targetType;
            _packformatName = packformatName;
            _version = version;            
        }

        /// <summary>
        /// Returns the type provided by the custom deserializer marked by this attribute
        /// </summary>
        public Type TargeType
        {
            get { return _targeType; }
        }

        /// <summary>
        /// Returns the serialized type name targeted by the custom deserializer marked by this attribute
        /// </summary>
        public string PackformatName
        {
            get { return _packformatName; }
        }

        /// <summary>
        /// Returns the version targeted by the custom deserializer marked by this attribute
        /// </summary>
        public uint? Version
        {
            get { return _version; }
        }

        /// <summary>
        /// Specifies if the marked custom deserializer method should be used for all descendants of the given target type.
        /// Descendant detection uses the descendantSearchScope specified when creating <see cref="ShapeshifterSerializer"/>.
        /// </summary>
        public bool ForAllDescendants
        {
            get { return _forAllDescendants; }
            set { _forAllDescendants = value; }
        }
    }
}