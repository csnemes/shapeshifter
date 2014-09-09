using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Shapeshifter.SchemaComparison
{
    /// <summary>
    ///     Class for managing the history of snapshots.
    ///     SnapshotHistory is a mutable class.
    /// </summary>
    [DataContract]
    [Shapeshifter]
    public class SnapshotHistory : IEnumerable<Snapshot>
    {
        [DataMember]
        private readonly List<Snapshot> _snapshots;

        private SnapshotHistory()
        {
            _snapshots = new List<Snapshot>();
        }

        private SnapshotHistory(List<Snapshot> snapshots)
        {
            _snapshots = snapshots;
        }
        
        public static SnapshotHistory Empty
        {
            get { return new SnapshotHistory(); }
        }

        public static SnapshotHistory LoadFrom(string filePath)
        {
            return null;
        }

        public static SnapshotHistory LoadFrom(Stream stream)
        {
            var serializer = new Shapeshifter<SnapshotHistory>();
            return serializer.Deserialize(stream);
        }

        public int Count
        {
            get { return _snapshots.Count; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Snapshot> GetEnumerator()
        {
            return _snapshots.GetEnumerator();
        }

        public void AddSnapshot(Snapshot snapshot)
        {
            _snapshots.Add(snapshot);
        }

        public SnapshotDifference CompareTo(Snapshot snapshot)
        {
            return snapshot.CompareToBase(_snapshots);
        }

        public void SaveTo(string filePath)
        {
        }

        public void SaveTo(Stream stream)
        {
            var serializer = new Shapeshifter<SnapshotHistory>();
            serializer.Serialize(stream, this);
        }
    }
}