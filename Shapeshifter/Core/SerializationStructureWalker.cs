using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace Shapeshifter.Core
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

        public void WalkType(Type type)
        {
            InternalWalkType(type, true);
        }

        public void WalkTypes(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                InternalWalkType(type, true);
            }
        }

        private void InternalWalkType(Type type, bool isRoot = false)
        {
            //skip natives and already visited types
            if (type.IsPrimitive || type == typeof (string)) return;
            if (_typesVisited.Contains(type)) return;

            _typesVisited.Add(type);

            var typeInspector = new TypeInspector(type);

            typeInspector.AcceptOnStaticMethods(_visitor);

            if (!typeInspector.IsSerializable) return;

            if (isRoot && !typeInspector.HasSerializerAttribute) return;

            typeInspector.AcceptOnType(_visitor);

            foreach (SerializableTypeMemberInfo candidate in typeInspector.SerializableItemCandidates)
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
                    if (genericDefType == typeof (List<>) || genericDefType == typeof (Collection<>))
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
                foreach (KnownTypeAttribute knownTypeAttribute in typeInspector.KnownTypeAttributes)
                {
                    if (knownTypeAttribute.Type != null)
                    {
                        InternalWalkType(knownTypeAttribute.Type);
                    }
                    else
                    {
                        MethodInfo methodToCall = type.GetMethod(knownTypeAttribute.MethodName,
                            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                        if (methodToCall == null)
                        {
                            throw Exceptions.KnownTypeMethodNotFound(knownTypeAttribute.MethodName, type);
                        }

                        var knownTypesList = methodToCall.Invoke(null, new object[0]) as IEnumerable<Type>;
                        if (knownTypesList == null)
                        {
                            throw Exceptions.KnownTypeMethodReturnValueIsInvalid(knownTypeAttribute.MethodName, type);
                        }

                        foreach (Type knownType in knownTypesList)
                        {
                            InternalWalkType(knownType);
                        }
                    }
                }
            }
        }
    }
}