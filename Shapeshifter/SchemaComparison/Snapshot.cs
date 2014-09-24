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
    ///     Class holding serializer data for a given point in time
    ///     This class is immutable incl. the data within
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

        public string Name
        {
            get { return _name; }
        }
        
        public DateTime TakenDate
        {
            get { return _takenDate; }
        }

        private IEnumerable<SerializerInfo> Serializers
        {
            get { return _serializers; }
        }

        public static Snapshot Create(string snapshotName, IEnumerable<string> assemblyPaths)
        {
            return SnapshotCreatorInSeparateAppDomain.Create(snapshotName, assemblyPaths);
        }

        public static Snapshot Create(string snapshotName, IEnumerable<Assembly> assembliesInScope)
        {
            SnapshotDetector builder = SnapshotDetector.CreateFor(assembliesInScope);
            return new Snapshot(snapshotName, builder.Serializers, builder.Deserializers);
        }

        public static Snapshot Create(string snapshotName, IEnumerable<Type> knownTypes)
        {
            SnapshotDetector builder = SnapshotDetector.CreateFor(null, knownTypes);
            return new Snapshot(snapshotName, builder.Serializers, builder.Deserializers);
        }

        public static Snapshot Create(string snapshotName, params Type[] knownTypes)
        {
            return Create(snapshotName, (IEnumerable<Type>) knownTypes);
        }

        public static Snapshot LoadFrom(string filePath)
        {
            var serializer = new DataContractSerializer(typeof (Snapshot));
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                return (Snapshot) serializer.ReadObject(stream);
            }
        }

        public void SaveTo(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Create))
            {
                SaveTo(stream);
            }
        }

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

        public SnapshotDifference CompareToActual(Snapshot snapshot)
        {
            return new SnapshotDifference(InternalCompareToActual(snapshot));
        }

        public SnapshotDifference CompareToBase(Snapshot snapshot)
        {
            return new SnapshotDifference(snapshot.InternalCompareToActual(this));
        }

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