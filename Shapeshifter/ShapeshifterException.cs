using System;
using System.Runtime.Serialization;
using Shapeshifter.Annotations;

namespace Shapeshifter
{
    [Serializable]
    public class ShapeshifterException : ApplicationException
    {
        private readonly string _id;
        
        public ShapeshifterException(string id, string message, Exception innerException = null)
            : base(message, innerException)
        {
            _id = id;
        }

        protected ShapeshifterException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public string Id
        {
            get { return _id; }
        }
    }
}