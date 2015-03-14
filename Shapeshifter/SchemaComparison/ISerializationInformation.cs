using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shapeshifter.SchemaComparison
{
    /// <summary>
    /// Interface provides information on default serializers such as version, packformat name and the underlying type.
    /// This interface is used to provide this information for snapshot.exe if someone needs the current version of a type.
    /// </summary>
    public interface ISerializationInformation
    {
        /// <summary>
        /// The fully qualified name of the type
        /// </summary>
        string TypeFullName { get; }

        /// <summary>
        /// Version used in the serialized form
        /// </summary>
        uint Version { get; }

        /// <summary>
        /// Name used in the serialized form
        /// </summary>
        string PackformatName { get; }
    }
}
