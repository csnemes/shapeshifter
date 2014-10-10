using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Shapeshifter.Core.Deserialization;
using Shapeshifter.SchemaComparison.Impl;

namespace Shapeshifter.SchemaComparison
{
    /// <summary>
    ///     Class holding serializer data for a given point in time, used for comparing different versions of the same serializable object graph in different points in time.
    ///     This class is immutable.
    /// </summary>
    [DataContract]
    [Serializable]
    public class Snapshot
    {
        [DataMember] private readonly Dictionary<DeserializerKey, DeserializerInfo> _deserializers;
        [DataMember] private readonly string _name;
        [DataMember] private readonly DateTime _takenDate;
        [DataMember] private readonly List<SerializerInfo> _serializers;

        private Snapshot(string name, IEnumerable<SerializerInfo> serializers,
            IEnumerable<DeserializerInfo> deserializers)
        {
            _name = name;
            _takenDate = DateTime.Now;
            _serializers = serializers.ToList();
            _deserializers = deserializers.ToDictionary(item => item.Key);
        }

        /// <summary>
        /// Name of the snapshot
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        
        /// <summary>
        /// Date when the snapshot was taken
        /// </summary>
        public DateTime TakenDate
        {
            get { return _takenDate; }
        }

        private IEnumerable<SerializerInfo> Serializers
        {
            get { return _serializers; }
        }

        /// <summary>
        /// Creates a snapshot from the given assemblies by searching for all serializable object graph roots marked by the <see cref="ShapeshifterRootAttribute"/>. 
        /// </summary>
        /// <param name="snapshotName">Name of the snapshot.</param>
        /// <param name="assemblyPaths">List of paths of the input assemblies.</param>
        /// <returns>The snapshot taken.</returns>
        public static Snapshot Create(string snapshotName, IEnumerable<string> assemblyPaths)
        {
            return SnapshotCreatorInSeparateAppDomain.Create(snapshotName, assemblyPaths);
        }

        /// <summary>
        /// Creates a snapshot from the given assemblies by searching for all serializable object graph roots marked by the <see cref="ShapeshifterRootAttribute"/>. 
        /// </summary>
        /// <param name="snapshotName">Name of the snapshot.</param>
        /// <param name="assembliesInScope">List of input assemblies.</param>
        /// <returns>The snapshot taken.</returns>
        public static Snapshot Create(string snapshotName, IEnumerable<Assembly> assembliesInScope)
        {
            SnapshotDetector builder = SnapshotDetector.CreateFor(assembliesInScope);
            return new Snapshot(snapshotName, builder.Serializers, builder.Deserializers);
        }

        /// <summary>
        /// Creates a snapshot from the given types. 
        /// </summary>
        /// <param name="snapshotName">Name of the snapshot.</param>
        /// <param name="knownTypes">List of types to examine (all roots of serializable object hierarchies).</param>
        /// <returns>The snapshot taken.</returns>
        public static Snapshot Create(string snapshotName, IEnumerable<Type> knownTypes)
        {
            SnapshotDetector builder = SnapshotDetector.CreateFor(null, knownTypes);
            return new Snapshot(snapshotName, builder.Serializers, builder.Deserializers);
        }

        /// <summary>
        /// Creates a snapshot from the given types. 
        /// </summary>
        /// <param name="snapshotName">Name of the snapshot.</param>
        /// <param name="knownTypes">List of types to examine (all roots of serializable object hierarchies).</param>
        /// <returns>The snapshot taken.</returns>
        public static Snapshot Create(string snapshotName, params Type[] knownTypes)
        {
            return Create(snapshotName, (IEnumerable<Type>) knownTypes);
        }

        /// <summary>
        /// Load a snapshot from a file.
        /// </summary>
        /// <param name="filePath">The path for the serialized snapshot.</param>
        /// <returns>The snapshot loaded.</returns>
        public static Snapshot LoadFrom(string filePath)
        {
            var serializer = new DataContractSerializer(typeof (Snapshot));
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                return (Snapshot) serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Saves the snapshot to a file.
        /// </summary>
        /// <param name="filePath">The path for the serialized snapshot.</param>
        public void SaveTo(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Create))
            {
                SaveTo(stream);
            }
        }

        /// <summary>
        /// Saves the snapshot to a stream.
        /// </summary>
        /// <param name="stream">The target stream to save the snapshot to.</param>
        public void SaveTo(Stream stream)
        {
            var serializer = new DataContractSerializer(typeof(Snapshot));
            serializer.WriteObject(stream, this);
        }

        private IEnumerable<MissingDeserializerInfo> InternalCompareToActual(Snapshot snapshot)
        {
            return Serializers.Where(item => !snapshot.ExistsDeserializerFor(item))
                .Select(serializer => new MissingDeserializerInfo(this, serializer));
        }

        /// <summary>
        /// Compares the snapshot (considered as base) to the given actual snapshot and returns the differences as a <see cref="SnapshotDifference"/>.
        /// </summary>
        /// <param name="snapshot">The snapshot of the actual state.</param>
        /// <returns>The difference between the snapshots.</returns>
        public SnapshotDifference CompareToActual(Snapshot snapshot)
        {
            return new SnapshotDifference(InternalCompareToActual(snapshot));
        }

        /// <summary>
        /// Compares the snapshot (considered as actual) to the given base snapshot and returns the differences as a <see cref="SnapshotDifference"/>.
        /// </summary>
        /// <param name="snapshot">The snapshot of the base state.</param>
        /// <returns>The difference between the snapshots.</returns>
        public SnapshotDifference CompareToBase(Snapshot snapshot)
        {
            return new SnapshotDifference(snapshot.InternalCompareToActual(this));
        }

        /// <summary>
        /// Compares the snapshot (considered as actual) to the given base snapshots and returns the differences as a <see cref="SnapshotDifference"/>.
        /// </summary>
        /// <param name="snapshots">The list of base snapshots.</param>
        /// <returns>The difference between the snapshots.</returns>
        public SnapshotDifference CompareToBase(IEnumerable<Snapshot> snapshots)
        {
            SnapshotDifference result = SnapshotDifference.Empty;
            foreach (Snapshot snapshot in snapshots)
            {
                result = result.CombineWith(CompareToBase(snapshot));
            }

            return result;
        }

        private bool ExistsDeserializerFor(SerializerInfo serializerInfo)
        {
            return _deserializers.ContainsKey(serializerInfo.Key);
        }
    }
}