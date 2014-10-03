using System.IO;
using System.Text;
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
        /// <param name="verbose">if true writes the original class structure as well</param>
        public void WriteHumanReadableExplanation(TextWriter writer, bool verbose)
        {
            writer.WriteLine("Compared to snapshot {0} the deserializer for type {1} with version {2} is missing.",
                SnapshotName, MissingPackformatName, MissingVersion);

            //original class details
            if (verbose)
            {
                var defaultOriginalSerializer = _serializerInfo as DefaultSerializerInfo;
                if (defaultOriginalSerializer != null)
                {
                    writer.WriteLine(GetOriginalClassStructure(defaultOriginalSerializer));
                }
            }
        }

        private string GetOriginalClassStructure(DefaultSerializerInfo defaultSerializerInfo)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("serialized class {0} \n", defaultSerializerInfo.TypeFullName);
            builder.AppendLine("{");
            foreach (var serializedMember in defaultSerializerInfo.SerializedMembers)
            {
                builder.AppendFormat("\t{0} {1}\n", serializedMember.Type, serializedMember.Name);
            }
            builder.AppendLine("}");
            return builder.ToString();
        }
    }
}