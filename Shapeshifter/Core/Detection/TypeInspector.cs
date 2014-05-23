using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Shapeshifter.Utils;

namespace Shapeshifter.Core.Detection
{
    /// <summary>
    ///     Class used for getting all sort of type related information for a given Type
    /// </summary>
    /// <remarks>
    ///     Lazy properties used for do some late init to improve performance. For bigger performance gain TypeInspector
    ///     instances should be cached. TODO
    /// </remarks>
    internal class TypeInspector
    {
        private readonly Lazy<bool> _hasDataContractAttribute;
        private readonly Lazy<List<SerializableMemberInfo>> _itemCandidates;
        private readonly Lazy<List<KnownTypeAttribute>> _knownTypeAttributes;
        private readonly Lazy<ShapeshifterAttribute> _shapeshifterAttribute;
        private readonly Type _type;

        private uint _version;

        public TypeInspector(Type type)
        {
            _type = type;
            _hasDataContractAttribute = new Lazy<bool>(() => _type.GetCustomAttributes(typeof (DataContractAttribute), false).Any());
            
            _itemCandidates = new Lazy<List<SerializableMemberInfo>>(() => GetSerializableItemCandidatesForType(_type));

            _knownTypeAttributes = new Lazy<List<KnownTypeAttribute>>(GetKnownTypeAttributes);
        
            _shapeshifterAttribute = new Lazy<ShapeshifterAttribute>(
                () => _type.GetCustomAttributes(typeof(ShapeshifterAttribute), false).FirstOrDefault() as ShapeshifterAttribute);
        }

        public IEnumerable<SerializableMemberInfo> SerializableItemCandidates
        {
            get { return _itemCandidates.Value; }
        }

        public bool HasDataContractAttribute
        {
            get { return _hasDataContractAttribute.Value; }
        }

        public bool HasShapeshifterAttribute
        {
            get { return ShapeshifterAttribute != null; }
        }

        public bool IsSerializable
        {
            get { return HasDataContractAttribute || HasShapeshifterAttribute || IsNativeType; }
        }

        public bool IsNativeType
        {
            get { return this.Type.IsPrimitive || this.Type == typeof(String); }
        }

        public bool HasKnownTypeAttribute
        {
            get { return KnownTypeAttributes.Any(); }
        }

        public IEnumerable<KnownTypeAttribute> KnownTypeAttributes
        {
            get { return _knownTypeAttributes.Value; }
        }

        private ShapeshifterAttribute ShapeshifterAttribute
        {
            get { return _shapeshifterAttribute.Value; }
        }

        public uint Version
        {
            get
            {
                if (_version == default(uint))
                {
                    _version = GetVersionFromShapeshifterAttribute() ?? GetHashVersion();
                }
                return _version;
            }
        }

        public string PackformatName
        {
            get { return GetPackformatNameFromShapeshifterAttribute() ?? GetTypePrettyShortName(_type); }
        }

        internal Type Type
        {
            get { return _type; }
        }

        public IEnumerable<Type> GetKnownTypes()
        {
            foreach (KnownTypeAttribute knownTypeAttribute in KnownTypeAttributes)
            {
                if (knownTypeAttribute.Type != null)
                {
                    yield return knownTypeAttribute.Type;
                }
                else
                {
                    var methodToCall = this.Type.GetMethod(knownTypeAttribute.MethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (methodToCall == null)
                    {
                        throw Exceptions.KnownTypeMethodNotFound(knownTypeAttribute.MethodName, this.Type);
                    }

                    var knownTypesList = methodToCall.Invoke(null, new object[0]) as IEnumerable<Type>;
                    if (knownTypesList == null)
                    {
                        throw Exceptions.KnownTypeMethodReturnValueIsInvalid(knownTypeAttribute.MethodName, this.Type);
                    }

                    foreach (var knownType in knownTypesList)
                    {
                        yield return knownType;
                    }
                }
            }
        }

        private static string GetTypePrettyShortName(Type type)
        {
            if (!type.IsGenericType) return type.Name;

            Type[] genericArguments = type.GetGenericArguments();
            var sb = new StringBuilder(type.Name.Substring(0, type.Name.IndexOf('`')));
            sb.Append("<");
            for (int idx = 0; idx < genericArguments.Length; idx++)
            {
                Type argType = genericArguments[idx];
                sb.Append(GetTypePrettyShortName(argType));
                if (idx < genericArguments.Length - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append(">");
            return sb.ToString();
        }

        private List<KnownTypeAttribute> GetKnownTypeAttributes()
        {
            return _type.GetCustomAttributes(typeof (KnownTypeAttribute), false).OfType<KnownTypeAttribute>().ToList();
        }

        private uint GetHashVersion()
        {
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);

                foreach (SerializableMemberInfo candidate in SerializableItemCandidates.OrderBy(item => item.Name))
                {
                    writer.Write(candidate.Name);
                    writer.Write(GetTypePrettyShortName(candidate.Type));
                }
                writer.Flush();
                stream.Position = 0;
                return MurMurHash3.Hash(stream);
            }
        }

        private string GetPackformatNameFromShapeshifterAttribute()
        {
            return ShapeshifterAttribute != null ? ShapeshifterAttribute.PackformatName : null;
        }

        private uint? GetVersionFromShapeshifterAttribute()
        {
            return ShapeshifterAttribute != null && ShapeshifterAttribute.IsVersionSpecified
                ? (uint?)ShapeshifterAttribute.Version
                : null;
        }

        public void AcceptOnNonStaticMethods(ISerializableTypeVisitor visitor)
        {
            var nonStaticMethods = _type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var nonStaticMethod in nonStaticMethods)
            {
                ProcessAttributes<SerializerAttribute>(nonStaticMethod, ThrowInvalidAttributeUsage);
                ProcessAttributes<DeserializerAttribute>(nonStaticMethod, ThrowInvalidAttributeUsage);
            }
        }

        private static void ThrowInvalidAttributeUsage(Attribute attribute, MethodInfo methodInfo)
        {
            throw Exceptions.InvalidUsageOfAttributeOnInstanceMethod(attribute, methodInfo);
        }

        public void AcceptOnStaticMethods(ISerializableTypeVisitor visitor)
        {
            var staticMethods = _type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var staticMethod in staticMethods)
            {
                ProcessAttributes<SerializerAttribute>(staticMethod, visitor.VisitSerializerMethod);
                ProcessAttributes<DeserializerAttribute>(staticMethod, visitor.VisitDeserializerMethod);
            }
        }

        private static void ProcessAttributes<T>(MethodInfo staticMethod, Action<T, MethodInfo> processorMethod)
            where T: Attribute
        {
            var attributes = staticMethod.GetCustomAttributes(typeof(T), false).EmptyIfNull().OfType<T>();
            foreach (var attribute in attributes)
            {
                processorMethod.Invoke(attribute, staticMethod);
            }
        }

        public void AcceptOnType(ISerializableTypeVisitor visitor)
        {
            var typeInfo = new SerializableTypeInfo(Type, PackformatName, Version, SerializableItemCandidates);

            if (HasDataContractAttribute || HasShapeshifterAttribute)
            {
                visitor.VisitSerializableClass(typeInfo);
            }
        }

        private static List<SerializableMemberInfo> GetSerializableItemCandidatesForType(Type type)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            var candidates = new List<SerializableMemberInfo>();

            var allFields = type.GetAllFieldsRecursive(flags);

            candidates.AddRange(allFields
                .Where(fieldInfo => ContainsAttributeSpecifyingCandidates(fieldInfo.GetCustomAttributes(true)))
                .Select(fieldInfo => new SerializableMemberInfo(fieldInfo)));

            var allProperties = type.GetAllPropertiesRecursive(flags);

            candidates.AddRange(allProperties
                .Where(propertyInfo => ContainsAttributeSpecifyingCandidates(propertyInfo.GetCustomAttributes(true)))
                .Select(propertyInfo => new SerializableMemberInfo(propertyInfo)));

            return candidates;
        }

        private static bool ContainsAttributeSpecifyingCandidates(object[] attributes)
        {
            if (attributes == null || attributes.Length == 0) return false;
            return attributes.OfType<DataMemberAttribute>().Any();
        }
    }
}