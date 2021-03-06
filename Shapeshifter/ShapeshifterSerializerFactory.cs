﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using Shapeshifter.Core.Detection;

namespace Shapeshifter
{
    /// <summary>
    /// Class used for creating <see cref="ShapeshifterSerializer"/> instances. This class caches the serializers and parses all assemblies passed 
    /// to find types containing custom deserializers and shapeshifter roots. If no assemblies passed to the factory the current AppDomain assemblies are parsed.
    /// </summary>
    public class ShapeshifterSerializerFactory
    {
        private static readonly Lazy<ShapeshifterSerializerFactory> DefaultFactory = new Lazy<ShapeshifterSerializerFactory>(LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Returns the default shapeshifter serializer factory which uses the current appdomain for initialization of serializers
        /// </summary>
        public static ShapeshifterSerializerFactory Default
        {
            get { return DefaultFactory.Value; }
        }

        private readonly List<Assembly> _assembliesToParse;
        private readonly ConcurrentDictionary<Type, ShapeshifterSerializer> _cache = new ConcurrentDictionary<Type, ShapeshifterSerializer>();

        /// <summary>
        /// Creates a factory which parses all passed assemblies for deserializers and roots when creating a serializer
        /// </summary>
        /// <param name="assembliesToParse">List of assemblies we would like to parse for deserializers and shapeshifterroots</param>
        public ShapeshifterSerializerFactory(IEnumerable<Assembly> assembliesToParse)
        {
            _assembliesToParse = new List<Assembly>(assembliesToParse);
        }

        /// <summary>
        /// Creates a factory which parses all AppDomain.CurrentDomain assemblies for deserializers and roots when creating a serializer
        /// </summary>
        public ShapeshifterSerializerFactory()
        {
            //skipping GAC assemblies and shapeshifter assembly, aim is to parse only user assemblies
            var filteredAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.GlobalAssemblyCache)
                .Where(assembly => assembly != typeof (ShapeshifterSerializer).Assembly);
                
            _assembliesToParse = new List<Assembly>(filteredAssemblies);
        }

        /// <summary>
        /// Returns a <see cref="ShapeshifterSerializer{T}"/> for the specified type.
        /// </summary>
        /// <typeparam name="T">Type to be serialized/deserialized</typeparam>
        /// <returns>A serializer for the given type.</returns>
        public ShapeshifterSerializer<T> GetSerializer<T>()
        {
            return (ShapeshifterSerializer<T>) _cache.GetOrAdd(typeof (T), type =>
            {
                var metadata = MetadataExplorer.CreateFor(_assembliesToParse, ShapeshifterSerializer.BuiltInKnownTypes);
                return new ShapeshifterSerializer<T>(metadata);
            });
        }

    }
}
