using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Shapeshifter.Core.Detection
{
    /// <summary>
    ///     This class walks the given type(s) and looks for types which needed for serialization. It looks for DataMembers and
    ///     KnownTypes to have the complete
    ///     class structure (plus hierarchy) for serialization. It also looks for types with members having
    ///     <see cref="SerializerAttribute" /> or <see cref="DeserializerAttribute" />.
    ///     It uses the passed visitor <see cref="ISerializableTypeVisitor" /> to notify the caller on types found.
    /// </summary>
    internal class SerializationStructureWalker
    {
        private readonly HashSet<Type> _typesVisited = new HashSet<Type>();
        private readonly ISerializableTypeVisitor _visitor;

        public SerializationStructureWalker(ISerializableTypeVisitor visitor)
        {
            _visitor = visitor;
        }

        public void WalkRootType(Type type)
        {
            InternalWalkType(type);
        }

        public void WalkKnownType(Type type)
        {
            if (type.IsGenericTypeDefinition)
                throw Exceptions.IllegalUsageOfOpenGenericAsKnownType(type);

            InternalWalkType(type);
        }

        public void WalkRootTypes(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                InternalWalkType(type);
            }
        }

        private void InternalWalkType(Type type)
        {
            //TODO cache typeInspectors statically?
            var typeInspector = new TypeInspector(type);

            //skip natives and already visited types
            if (typeInspector.IsNativeType || _typesVisited.Contains(type)) return;

            _typesVisited.Add(type);

            typeInspector.AcceptOnNonStaticMethods(_visitor);
            typeInspector.AcceptOnStaticMethods(_visitor);

            if (!typeInspector.IsSerializable) return;

            typeInspector.AcceptOnType(_visitor);

            foreach (var candidate in typeInspector.SerializableItemCandidates)
            {
                Type candidateType = candidate.Type;

                //recognize arrays and all IEnumerable<T> types we support (like List<T> and Collection<T>) and add the T to the list of known types
                if (candidateType.IsArray)
                {
                    InternalWalkType(candidateType.GetElementType());
                }
                else if (candidateType.IsGenericType)
                {
                    Type genericDefType = candidateType.GetGenericTypeDefinition();
                    if (genericDefType == typeof(List<>) || genericDefType == typeof(Collection<>) || genericDefType == typeof(IEnumerable<>))
                    {
                        InternalWalkType(candidateType.GetGenericArguments()[0]);
                    }
                    else if (genericDefType == typeof (Dictionary<,>))
                    {
                        InternalWalkType(candidateType.GetGenericArguments()[0]);
                        InternalWalkType(candidateType.GetGenericArguments()[1]);
                    }
                }
                InternalWalkType(candidateType);
            }

            if (typeInspector.HasKnownTypeAttribute)
            {
                foreach (var knownType in typeInspector.GetKnownTypes())
                {
                    WalkKnownType(knownType);
                }
            }
        }
    }
}