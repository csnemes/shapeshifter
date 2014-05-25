using System;

namespace Shapeshifter.Core.Deserialization
{
    /// <summary>
    ///     A key is the combination of packformatName and version
    /// </summary>
    internal struct DeserializerKey
    {
        private readonly string _packedName;
        private readonly uint _version; //zero means unspecified

        public DeserializerKey(string packedName, uint version = 0)
        {
            if (String.IsNullOrWhiteSpace(packedName))
            {
                throw new ArgumentException("packedName");
            }
            _packedName = packedName;
            _version = version;
        }

        public string PackedName
        {
            get { return _packedName; }
        }

        public uint Version
        {
            get { return _version; }
        }

        public bool Equals(DeserializerKey other)
        {
            return string.Equals(_packedName, other._packedName) && _version == other._version;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DeserializerKey && Equals((DeserializerKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_packedName.GetHashCode()*397) ^ (int) _version;
            }
        }

        public static bool operator ==(DeserializerKey left, DeserializerKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DeserializerKey left, DeserializerKey right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", _packedName, _version);
        }
    }
}