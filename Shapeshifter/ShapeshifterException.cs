using System;
using System.Runtime.Serialization;
using Shapeshifter.Annotations;

namespace Shapeshifter
{
    [Serializable]
    public class ShapeshifterException : ApplicationException
    {
        public ShapeshifterException()
        {
        }

        public ShapeshifterException(string message) : base(message)
        {
        }

        public ShapeshifterException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ShapeshifterException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}