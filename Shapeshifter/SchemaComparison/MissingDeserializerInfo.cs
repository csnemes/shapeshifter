using System.IO;
using Shapeshifter.Core.Deserialization;
using Shapeshifter.SchemaComparison.Impl;

namespace Shapeshifter.SchemaComparison
{
    /// <summary>
    /// Class containing information about a missing deserializer. Instances of this class are created during snapshot comparison of serializable object graphs.
    /// </summary>
    public class MissingDeserializerInfo
    {
        private readonly SerializerInfo _serializerInfo;
        private readonly Snapshot _snapshot;

        internal MissingDeserializerInfo(Snapshot snapshot, SerializerInfo serializerInfo)
        {
            _snapshot = snapshot;
            _serializerInfo = serializerInfo;
        }

        /// <summary>
        /// Name of the snapshot which contains the original type
        /// </summary>
        public string SnapshotName
        {
            get { return _snapshot.Name; }
        }

        /// <summary>
        /// The <see cref="SchemaComparison.Snapshot"/> which contains the original type
        /// </summary>
        public Snapshot Snapshot
        {
            get { return _snapshot; }
        }

        /// <summary>
        /// The serialized name for which the deserializer is missing.
        /// </summary>
        public string MissingPackformatName
        {
            get { return _serializerInfo.PackformatName; }
        }

        /// <summary>
        /// The version for which the deserializer is missing.
        /// </summary>
        public uint MissingVersion
        {
            get { return _serializerInfo.Version; }
        }

        internal DeserializerKey Key
        {
            get { return new DeserializerKey(MissingPackformatName, MissingVersion); }
        }

        /// <summary>
        /// Writes human readable explanation of the difference onto the given <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer"><see cref="TextWriter"/> to write to.</param>
        public void WriteHumanReadableExplanation(TextWriter writer)
        {
            writer.WriteLine("Compared to snapshot {0} the deserializer for type {1} with version {2} is missing.",
                SnapshotName, MissingPackformatName, MissingVersion);
        }
    }
}