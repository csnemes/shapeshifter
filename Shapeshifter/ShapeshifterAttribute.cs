using System;

namespace Shapeshifter
{
    /// <summary>
    /// </summary>
    /// <example>
    ///     Marker attribute meaning that this class will be serialized and deserialized using Shapeshifter's default serialization method.
    ///     [Shapeshifter]
    ///     public class MyClass
    ///     { ... }
    /// 
    ///     Also the default serialization method can be influenced with this attribute specifying the classname and/or version used in the serialized format.
    ///     [Shapeshifter("MonClass", 1)]
    ///     public class MyClass
    ///     { ... }
    /// 
    ///     [Shapeshifter("MonClass")]
    ///     public class MyClass
    ///     { ... }
    ///
    ///     [Shapeshifter(1)]
    ///     public class MyClass
    ///     { ... }
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ShapeshifterAttribute : Attribute
    {
        private readonly string _packformatName;
        private uint? _version;

        public ShapeshifterAttribute(string packformatName, uint version)
            :this (packformatName, (uint?) version)
        {
        }

        public ShapeshifterAttribute(string packformatName = null)
            :this(packformatName, null)
        {
        }

        public ShapeshifterAttribute(uint version)
            : this(null, version)
        {
        }

        private ShapeshifterAttribute(string packformatName, uint? version)
        {
            _packformatName = packformatName;
            _version = version;
        }

        public bool IsPackformatNameSpecified
        {
            get { return _packformatName != null; }
        }

        public string PackformatName
        {
            get { return _packformatName; }
        }

        public bool IsVersionSpecified
        {
            get { return _version.HasValue; }
        }

        public uint Version
        {
            get { return _version.HasValue ? _version.Value : default(uint); }
            set { _version = value; }
        }

    }
}