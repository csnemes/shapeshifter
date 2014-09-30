using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shapeshifter.Core.Deserialization;

namespace Shapeshifter.SchemaComparison
{
    /// <summary>
    ///     Contains the result of the comparison of base snapshot(s) againts the current snapshot. Can be used to figure out
    ///     what version changes are
    ///     not handled yet in your current code.
    /// </summary>
    public class SnapshotDifference
    {
        private readonly List<MissingDeserializerInfo> _missingItems;

        internal SnapshotDifference(IEnumerable<MissingDeserializerInfo> missingItems)
        {
            _missingItems = new List<MissingDeserializerInfo>(missingItems);
        }

        private SnapshotDifference()
        {
            _missingItems = new List<MissingDeserializerInfo>();
        }

        /// <summary>
        /// An empty <see cref="SnapshotDifference"/>
        /// </summary>
        public static SnapshotDifference Empty
        {
            get { return new SnapshotDifference(); }
        }
        
        /// <summary>
        /// Returns true if the difference contains missing deserializers.
        /// </summary>
        public bool HasMissingItem
        {
            get { return _missingItems.Any(); }
        }

        /// <summary>
        /// Groups the missing deserializer information contained within the SnapshotDifference by snapshot name
        /// </summary>
        /// <returns>The grouped missing deserializers.</returns>
        public IDictionary<string, List<MissingDeserializerInfo>> GroupBySnapshotName()
        {
            return _missingItems.GroupBy(item => item.SnapshotName).ToDictionary(grp => grp.Key, grp => grp.ToList());
        }

        /// <summary>
        /// Groups the missing deserializer information contained within the SnapshotDifference by serialized class name
        /// </summary>
        /// <returns>The grouped missing deserializers.</returns>
        public IDictionary<string, List<MissingDeserializerInfo>> GroupByPackformatName()
        {
            return _missingItems.GroupBy(item => item.MissingPackformatName)
                .ToDictionary(grp => grp.Key, grp => grp.ToList());
        }

        /// <summary>
        /// Returns the differences corresponding to a specific base snapshot (specified by its name)
        /// </summary>
        /// <param name="snapshotName"></param>
        /// <returns>List of differences.</returns>
        public IEnumerable<MissingDeserializerInfo> GetMissingItemsComparedToBaseSnapshotCalled(string snapshotName)
        {
            return _missingItems.Where(item => Equals(item.SnapshotName, snapshotName));
        }

        /// <summary>
        /// Combines the content of the current SnapshotDifference with the one given.
        /// </summary>
        /// <param name="difference">SnapshotDifference to combine with</param>
        /// <returns>SnapshotDifference with data combined.</returns>
        public SnapshotDifference CombineWith(SnapshotDifference difference)
        {
            var existingKeys = new HashSet<DeserializerKey>(_missingItems.Select(item => item.Key));
            var result = new List<MissingDeserializerInfo>(_missingItems);
            result.AddRange(difference._missingItems.Where(item => !existingKeys.Contains(item.Key)));
            return new SnapshotDifference(result);
        }

        /// <summary>
        /// Returns differences in human readable form.
        /// </summary>
        /// <returns>Differences described.</returns>
        public string GetHumanReadableResult()
        {
            using (var writer = new StringWriter())
            {
                foreach (var missingDeserializerInfo in _missingItems)
                {
                    missingDeserializerInfo.WriteHumanReadableExplanation(writer);
                }
                return writer.GetStringBuilder().ToString();
            }
        }
    }
}