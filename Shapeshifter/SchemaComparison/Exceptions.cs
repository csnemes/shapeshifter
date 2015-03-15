using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Shapeshifter.SchemaComparison
{
    internal static class Exceptions
    {

        public const string SnapshotNameIsMissingId = "SnapshotNameIsMissing";
        public static Exception SnapshotNameIsMissing()
        {
            return SafeCreateException(() => new ShapeshifterException(SnapshotNameIsMissingId, "Snapshot name is missing"));
        }

        public const string DuplicateSnapshotNameId = "DuplicateSnapshotName";
        public static Exception DuplicateSnapshotName(string name)
        {
            return SafeCreateException(() => new ShapeshifterException(DuplicateSnapshotNameId,
                String.Format("Snapshot name {0} already exists in snapshot file.", name)));
        }


        public const string SnapshotIsMssingId = "SnapshotIsMssing";
        public static Exception SnapshotIsMssing(string name)
        {
            return SafeCreateException(() => new ShapeshifterException(SnapshotIsMssingId,
                String.Format("Snapshot with name {0} cannot be found.", name)));
        }

        private static Exception SafeCreateException(Func<Exception> exceptionCreationFunc)
        {
            try
            {
                return exceptionCreationFunc();
            }
            catch (Exception ex)
            {
                return
                    new ApplicationException(
                        String.Format(
                            "Failed to create an exception. An exception occured while trying to create the real" +
                            " exception with {0}.", exceptionCreationFunc), ex);
            }
        }

    }
}
