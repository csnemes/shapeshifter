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
        private const BindingFlags BindingFlagsForInstanceMembers = 
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private const BindingFlags BindingFlagsForStaticMembers = 
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        private readonly Lazy<bool> _hasDataContractAttribute;
        private readonly Lazy<List<FieldOrPropertyMemberInfo>> _serializableMemberCandidates;
        private readonly Lazy<Dictionary<string, FieldOrPropertyMemberInfo>> _dataHolderMembers;
        private readonly Lazy<List<KnownTypeAttribute>> _knownTypeAttributes;
        private readonly Lazy<ShapeshifterAttribute> _shapeshifterAttribute;
        private readonly Type _type;

        private uint _version;

        public TypeInspector(Type type)
        {
            _type = type;
            _hasDataContractAttribute = new Lazy<bool>(() => _type.GetCustomAttributes(typeof (DataContractAttribute), false).Any());
            
            _serializableMemberCandidates = new Lazy<List<FieldOrPropertyMemberInfo>>(() => GetSerializableItemCandidatesForType(_type));

            _dataHolderMembers = new Lazy<Dictionary<string, FieldOrPropertyMemberInfo>>(() => 
                GetAllFieldAndPropertyMembersForType(_type).ToDictionary(fld => fld.Name));

            _knownTypeAttributes = new Lazy<List<KnownTypeAttribute>>(GetKnownTypeAttributes);
        
            _shapeshifterAttribute = new Lazy<ShapeshifterAttribute>(
                () => _type.GetCustomAttributes(typeof(ShapeshifterAttribute), false).FirstOrDefault() as ShapeshifterAttribute);
        }

        public IEnumerable<FieldOrPropertyMemberInfo> SerializableMemberCandidates
        {
            get { return _serializableMemberCandidates.Value; }
        }

        public IEnumerable<FieldOrPropertyMemberInfo> DataHolderMembers
        {
            get { return _dataHolderMembers.Value.Values; }
        }

        public FieldOrPropertyMemberInfo GetFieldOrPropertyMemberInfo(string name)
        {
            FieldOrPropertyMemberInfo result = null;
            if (!_dataHolderMembers.Value.TryGetValue(name, out result))
            {
                throw Exceptions.CannotFindFieldOrProperty(name, this.Type);
            }
            return result;
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
            get { return HasDataContractAttribute || IsNativeType; }
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
            get { return GetPackformatNameFromShapeshifterAttribute() ?? _type.GetPrettyName(); }
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
                    var methodToCall = this.Type.GetMethod(knownTypeAttribute.MethodName, BindingFlagsForStaticMembers);
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

        private List<KnownTypeAttribute> GetKnownTypeAttributes()
        {
            return _type.GetCustomAttributes(typeof (KnownTypeAttribute), false).OfType<KnownTypeAttribute>().ToList();
        }

        private uint GetHashVersion()
        {
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);

                foreach (FieldOrPropertyMemberInfo candidate in SerializableMemberCandidates.OrderBy(item => item.Name))
                {
                    writer.Write(candidate.Name);
                    writer.Write(candidate.Type.GetPrettyName());
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
            var nonStaticMethods = _type.GetMethods(BindingFlagsForInstanceMembers);

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
            var staticMethods = _type.GetMethods(BindingFlagsForStaticMembers);

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
            var typeInfo = new SerializableTypeInfo(Type, PackformatName, Version, SerializableMemberCandidates);

            if (HasDataContractAttribute)
            {
                visitor.VisitSerializableClass(typeInfo);
            }
        }

        private List<FieldOrPropertyMemberInfo> GetSerializableItemCandidatesForType(Type type)
        {
            return DataHolderMembers.Where(field => field.IsSerializable).ToList();
        }

        private static List<FieldOrPropertyMemberInfo> GetAllFieldAndPropertyMembersForType(Type type)
        {
            var allFields = type.GetAllFieldsRecursive(BindingFlagsForInstanceMembers);
            var allProperties = type.GetAllPropertiesRecursive(BindingFlagsForInstanceMembers);

            return allFields.Select(fieldInfo => new FieldOrPropertyMemberInfo(fieldInfo))
                .Concat(allProperties.Select(propertyInfo => new FieldOrPropertyMemberInfo(propertyInfo))).ToList();
        }

    }
}