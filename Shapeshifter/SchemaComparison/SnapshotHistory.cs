using System.Collections.Generic;
using System.IO;

namespace Shapeshifter.SchemaComparison
{
    /// <summary>
    ///     Class for managing the history of snapshots.
    /// </summary>
    public class SnapshotHistory
    {
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
            return null;
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
        }
    }
}