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
        private readonly Lazy<List<SerializableTypeMemberInfo>> _itemCandidates;
        private readonly Lazy<List<KnownTypeAttribute>> _knownTypeAttributes;
        private readonly Lazy<SerializerAttribute> _serializerAttribute;
        private readonly Lazy<ShapeshifterRootAttribute> _shapeshifterRootAttribute;
        private readonly Type _type;

        private uint _version;

        public TypeInspector(Type type)
        {
            _type = type;
            _hasDataContractAttribute = new Lazy<bool>(() => _type.GetCustomAttributes(typeof (DataContractAttribute), false).Any());
            
            _itemCandidates = new Lazy<List<SerializableTypeMemberInfo>>(() => GetSerializableItemCandidatesForType(_type));

            _knownTypeAttributes = new Lazy<List<KnownTypeAttribute>>(GetKnownTypeAttributes);
        
            _serializerAttribute = new Lazy<SerializerAttribute>(
                () => _type.GetCustomAttributes(typeof (SerializerAttribute), false).FirstOrDefault() as SerializerAttribute);

            _shapeshifterRootAttribute = new Lazy<ShapeshifterRootAttribute>(
                () => _type.GetCustomAttributes(typeof(ShapeshifterRootAttribute), false).FirstOrDefault() as ShapeshifterRootAttribute);
        }

        public IEnumerable<SerializableTypeMemberInfo> SerializableItemCandidates
        {
            get { return _itemCandidates.Value; }
        }

        public bool HasDataContractAttribute
        {
            get { return _hasDataContractAttribute.Value; }
        }

        public bool HasSerializerAttribute
        {
            get { return SerializerAttribute != null; }
        }

        public bool HasShapeshifterRootAttribute
        {
            get { return ShapeshifterRootAttribute != null; }
        }

        public bool IsSerializable
        {
            get { return HasDataContractAttribute || HasSerializerAttribute || IsNativeType; }
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

        private SerializerAttribute SerializerAttribute
        {
            get { return _serializerAttribute.Value; }
        }

        private ShapeshifterRootAttribute ShapeshifterRootAttribute
        {
            get { return _shapeshifterRootAttribute.Value; }
        }

        public uint Version
        {
            get
            {
                if (_version == default(uint))
                {
                    _version = GetVersionFromSerializerAttribute() ?? GetHashVersion();
                }
                return _version;
            }
        }

        public string PackformatName
        {
            get { return GetPackformatNameFromSerializerAttribute() ?? GetTypePrettyShortName(_type); }
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

                foreach (SerializableTypeMemberInfo candidate in SerializableItemCandidates.OrderBy(item => item.Name))
                {
                    writer.Write(candidate.Name);
                    writer.Write(GetTypePrettyShortName(candidate.Type));
                }
                writer.Flush();
                stream.Position = 0;
                return MurMurHash3.Hash(stream);
            }
        }

        private string GetPackformatNameFromSerializerAttribute()
        {
            return SerializerAttribute != null ? SerializerAttribute.PackformatName : null;
        }

        private uint? GetVersionFromSerializerAttribute()
        {
            return SerializerAttribute != null && SerializerAttribute.HasVersion
                ? (uint?) SerializerAttribute.Version
                : null;
        }

        /// <summary>
        ///     Detects all deserializer candidates defined on a type. Deserializer candidates are static methods with an
        ///     <see cref="DeserializerAttribute" /> attribute.
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public void AcceptOnStaticMethods(ISerializableTypeVisitor visitor)
        {
            var staticMethods = _type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var staticMethod in staticMethods)
            {
                var serializerAttributes = staticMethod.GetCustomAttributes(typeof (SerializerAttribute), false).EmptyIfNull().OfType<SerializerAttribute>();

                foreach (var serializerAttribute in serializerAttributes)
                {
                    if (!serializerAttribute.ValidOnMethod)
                        throw Exceptions.InvalidUseOfSerializerAttributeOnMethod(staticMethod);
                    visitor.VisitSerializerMethod(serializerAttribute, staticMethod);
                }

                var deserializerAttributes = staticMethod.GetCustomAttributes(typeof (DeserializerAttribute), false).EmptyIfNull().OfType<DeserializerAttribute>();
                foreach (var deserializerAttribute in deserializerAttributes)
                {
                    if (!deserializerAttribute.ValidOnMethod)
                        throw Exceptions.InvalidUseOfDeserializerAttributeOnMethod(staticMethod);
                    visitor.VisitDeserializerMethod(deserializerAttribute, staticMethod);
                }
            }
        }

        public void AcceptOnType(ISerializableTypeVisitor visitor)
        {
            Type type = Type;
            var typeInfo = new TypeInfo(Type, PackformatName, Version, SerializableItemCandidates);

            if (HasDataContractAttribute)
            {
                visitor.VisitSerializerOnClass(typeInfo);
                //add default deserializer with current version
                visitor.VisitDeserializerOnClass(new DeserializerAttribute(PackformatName, Version), typeInfo);
            }

            var deserializerAttributes = type.GetCustomAttributes(typeof (DeserializerAttribute), false).EmptyIfNull().OfType<DeserializerAttribute>();

            foreach (var deserializerAttribute in deserializerAttributes)
            {
                if (!deserializerAttribute.ValidOnClass)
                    throw Exceptions.InvalidUseOfDeserializerAttributeOnClass(type);
                visitor.VisitDeserializerOnClass(deserializerAttribute, typeInfo);
            }
        }

        private static List<SerializableTypeMemberInfo> GetSerializableItemCandidatesForType(Type type)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            var candidates = new List<SerializableTypeMemberInfo>();

            var allFields = type.GetAllFieldsRecursive(flags);

            candidates.AddRange(allFields
                .Where(fieldInfo => ContainsAttributeSpecifyingCandidates(fieldInfo.GetCustomAttributes(true)))
                .Select(fieldInfo => new SerializableTypeMemberInfo(fieldInfo)));

            var allProperties = type.GetAllPropertiesRecursive(flags);

            candidates.AddRange(allProperties
                .Where(propertyInfo => ContainsAttributeSpecifyingCandidates(propertyInfo.GetCustomAttributes(true)))
                .Select(propertyInfo => new SerializableTypeMemberInfo(propertyInfo)));

            return candidates;
        }

        private static bool ContainsAttributeSpecifyingCandidates(object[] attributes)
        {
            if (attributes == null || attributes.Length == 0) return false;
            return attributes.OfType<DataMemberAttribute>().Any();
        }
    }
}