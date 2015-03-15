using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Shapeshifter.SchemaComparison
{
    /// <summary>
    ///     Class for managing the history of snapshots.
    ///     SnapshotHistory is a mutable class, but not the <see cref="Snapshot"/>s within.
    /// </summary>
    [DataContract]
    [ShapeshifterRoot]
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
        
        /// <summary>
        /// An Empty <see cref="SnapshotHistory"/>
        /// </summary>
        public static SnapshotHistory Empty
        {
            get { return new SnapshotHistory(); }
        }

        /// <summary>
        /// Loads the <see cref="SnapshotHistory"/> from the given file.
        /// </summary>
        /// <param name="filePath">Path to the serialized SnapshotHistory.</param>
        /// <returns>The SnapshotHistory loaded.</returns>
        public static SnapshotHistory LoadFrom(string filePath)
        {
            using (var file = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                return LoadFrom(file);
            }
        }

        /// <summary>
        /// Loads the <see cref="SnapshotHistory"/> from the given stream.
        /// </summary>
        /// <param name="stream">A stream containing the serialized SnapshotHistory.</param>
        /// <returns>The SnapshotHistory loaded.</returns>
        public static SnapshotHistory LoadFrom(Stream stream)
        {
            var serializer = new ShapeshifterSerializer<SnapshotHistory>();
            return serializer.Deserialize(stream);
        }

        /// <summary>
        /// Returns the number of <see cref="Snapshot"/>s in the history."/>
        /// </summary>
        public int Count
        {
            get { return _snapshots.Count; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator for the Snapshots in the history.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Snapshot> GetEnumerator()
        {
            return _snapshots.GetEnumerator();
        }

        /// <summary>
        /// Adds a snapshot to the end of the history.
        /// </summary>
        /// <param name="snapshot">Snapshot to add.</param>
        public void AddSnapshot(Snapshot snapshot)
        {
            CheckSnapshotName(snapshot.Name);
            _snapshots.Add(snapshot);
        }

        /// <summary>
        /// Replaces an existing snapshot with the same name. 
        /// </summary>
        /// <param name="snapshot">Snapshot to replace with.</param>
        public void ReplaceSnapshot(Snapshot snapshot)
        {
            if (String.IsNullOrWhiteSpace(snapshot.Name))
            {
                throw Exceptions.SnapshotNameIsMissing();
            }

            var indexToBeReplaced =
                _snapshots.FindIndex(snap => snap.Name.Equals(snapshot.Name, StringComparison.OrdinalIgnoreCase));

            if (indexToBeReplaced == -1)
            {
                throw Exceptions.SnapshotIsMssing(snapshot.Name);
            }

            _snapshots[indexToBeReplaced] = snapshot;
        }

        private void CheckSnapshotName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Exceptions.SnapshotNameIsMissing();
            }
            if (_snapshots.Any(snapshot => snapshot.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                throw Exceptions.DuplicateSnapshotName(name);
            }
        }

        /// <summary>
        /// Compares the given actual Snapshot to the snapshots in the SnapshotHistory.
        /// </summary>
        /// <param name="snapshot">The actual snapshot.</param>
        /// <returns>The differences between the snapshots.</returns>
        public SnapshotDifference CompareTo(Snapshot snapshot)
        {
            return snapshot.CompareToBase(_snapshots);
        }

        /// <summary>
        /// Saves the SnapshotHistory to the given path.
        /// </summary>
        /// <param name="filePath">Target path to save the serialized SnapshotHistory to.</param>
        public void SaveTo(string filePath)
        {
            using (var file = File.Create(filePath))
            {
                SaveTo(file);
            }
        }

        /// <summary>
        /// Saves the SnapshotHistory to the given stream.
        /// </summary>
        /// <param name="stream">A target stream to save the serialized SnapshotHistory to.</param>
        public void SaveTo(Stream stream)
        {
            var serializer = new ShapeshifterSerializer<SnapshotHistory>();
            serializer.Serialize(stream, this);
        }


    }
}