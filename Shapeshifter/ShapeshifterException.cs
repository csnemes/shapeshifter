using System;
using System.Runtime.Serialization;

namespace Shapeshifter
{
    /// <summary>
    ///     Base exception type for all exceptions in the library.
    /// </summary>
    [Serializable]
    public class ShapeshifterException : ApplicationException
    {
        private readonly string _id;
        
        /// <summary>
        /// Constructor for ShapeshifterException
        /// </summary>
        /// <param name="id">Unique id of the exception type.</param>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public ShapeshifterException(string id, string message, Exception innerException = null)
            : base(message, innerException)
        {
            _id = id;
        }

        /// <summary>
        /// Serialization constructor of the exception
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ShapeshifterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Unique Id of the exception. Used in testing scenarios to check the outcome of an exception.
        /// </summary>
        public string Id
        {
            get { return _id; }
        }
    }
}