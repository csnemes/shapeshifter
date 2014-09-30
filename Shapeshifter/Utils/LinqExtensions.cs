using System.Collections.Generic;
using System.Linq;

namespace Shapeshifter.Utils
{
    internal static class LinqExtensions
    {
        /// <summary>
        ///     Indicates whether the specified source is null or an empty collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
                return true;

            return (!source.Any());
        }

        /// <summary>
        ///     In case of a null source returns an empty <see cref="IEnumerable{T}" /> that has the specified type argument.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            if (source == null)
                return Enumerable.Empty<T>();

            return source;
        }
    }
}