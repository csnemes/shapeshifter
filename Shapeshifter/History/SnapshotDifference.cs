﻿using System.Collections.Generic;
using System.Linq;
using Shapeshifter.Core;

namespace Shapeshifter.History
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

        public static SnapshotDifference Empty
        {
            get { return new SnapshotDifference(); }
        }


        public bool HasMissingItem
        {
            get { return _missingItems.Count > 0; }
        }


        public IDictionary<string, List<MissingDeserializerInfo>> GroupBySnapshotName()
        {
            return _missingItems.GroupBy(item => item.SnapshotName).ToDictionary(grp => grp.Key, grp => grp.ToList());
        }

        public IDictionary<string, List<MissingDeserializerInfo>> GroupByPackformatName()
        {
            return _missingItems.GroupBy(item => item.MissingPackformatName)
                .ToDictionary(grp => grp.Key, grp => grp.ToList());
        }

        public IEnumerable<MissingDeserializerInfo> GetMissingItemsComparedToBaseSnapshotCalled(string snapshotName)
        {
            return _missingItems.Where(item => Equals(item.SnapshotName, snapshotName));
        }

        public SnapshotDifference CombineWith(SnapshotDifference difference)
        {
            var existingKeys = new HashSet<DeserializerKey>(_missingItems.Select(item => item.Key));
            var result = new List<MissingDeserializerInfo>(_missingItems);
            result.AddRange(difference._missingItems.Where(item => !existingKeys.Contains(item.Key)));
            return new SnapshotDifference(result);
        }
    }
}