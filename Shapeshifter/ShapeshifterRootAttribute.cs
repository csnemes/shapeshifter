using System;

namespace Shapeshifter
{
    /// <summary>
    ///     Marks the root of a serialization object graph. 
    ///     It can be used also to specify the serialized name and/or version of a class for the default serializer/deserializer.
    /// </summary>
    /// <example>
    ///      Marker attribute meaning that this class will be serialized and deserialized using Shapeshifter's default serialization method.
    /// 
    /// <code>
    ///     [ShapeshifterRoot]
    ///     public class MyClass
    ///     { ... }
    /// </code>
    /// 
    ///     Also the default serialization method can be influenced with this attribute specifying the class name and/or version used in the serialized format.
    /// 
    /// <code>
    ///     [ShapeshifterRoot("MonClass", 1)]
    ///     public class MyClass
    ///     { ... }
    /// 
    ///     [ShapeshifterRoot("MonClass")]
    ///     public class MyClass
    ///     { ... }
    ///
    ///     [ShapeshifterRoot(1)]
    ///     public class MyClass
    ///     { ... }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class ShapeshifterRootAttribute : Attribute
    {
        private readonly string _packformatName;
        private uint? _version;

        /// <summary>
        /// Initializes the attribute with a serialized name and version.
        /// </summary>
        /// <param name="packformatName">Name used in the serialized form for the attributed type</param>
        /// <param name="version">Version used in the serialized form</param>
        public ShapeshifterRootAttribute(string packformatName, uint version)
            :this (packformatName, (uint?) version)
        {
        }

        /// <summary>
        /// Initializes the attribute with a serialized name if given.
        /// </summary>
        /// <param name="packformatName">Name used in the serialized form for the attributed typye</param>
        public ShapeshifterRootAttribute(string packformatName = null)
            :this(packformatName, null)
        {
        }

        /// <summary>
        /// Initializes the attribute with a version and the default name.
        /// </summary>
        /// <param name="version">Version used in the serialized form</param>
        public ShapeshifterRootAttribute(uint version)
            : this(null, version)
        {
        }

        private ShapeshifterRootAttribute(string packformatName, uint? version)
        {
            _packformatName = packformatName;
            _version = version;
        }

        /// <summary>
        /// Returns the serialized name
        /// </summary>
        public string PackformatName
        {
            get { return _packformatName; }
        }


        /// <summary>
        /// Returns true if version is specified
        /// </summary>
        public bool IsVersionSpecified
        {
            get { return _version.HasValue; }
        }

        /// <summary>
        /// Returns the version
        /// </summary>
        public uint Version
        {
            get { return _version.HasValue ? _version.Value : default(uint); }
            set { _version = value; }
        }

    }
}