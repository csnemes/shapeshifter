using System.IO;
using Shapeshifter.Core;
using Shapeshifter.Core.Deserialization;

namespace Shapeshifter.SchemaComparison
{
    public class MissingDeserializerInfo
    {
        private readonly SerializerInfo _serializerInfo;
        private readonly Snapshot _snapshot;

        internal MissingDeserializerInfo(Snapshot snapshot, SerializerInfo serializerInfo)
        {
            _snapshot = snapshot;
            _serializerInfo = serializerInfo;
        }

        public string SnapshotName
        {
            get { return _snapshot.Name; }
        }

        public Snapshot Snapshot
        {
            get { return _snapshot; }
        }

        public string MissingPackformatName
        {
            get { return _serializerInfo.PackformatName; }
        }

        public uint MissingVersion
        {
            get { return _serializerInfo.Version; }
        }

        internal DeserializerKey Key
        {
            get { return new DeserializerKey(MissingPackformatName, MissingVersion); }
        }

        public void WriteHumanReadableExplanation(TextWriter writer)
        {
            writer.WriteLine("Compared to snapshot {0} the deserializer for type {1} with version {2} is missing.",
                SnapshotName, _serializerInfo.PackformatName, MissingVersion);
        }
    }
}