using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Shapeshifter.Core
{
    /// <summary>
    ///     Helper data holder for storing member (field or property) information.
    /// </summary>
    /// <remarks>
    ///     It can be used for both fields and properties, internally it will use the correct Info
    /// </remarks>
    internal class FieldOrPropertyMemberInfo
    {
        private readonly FieldInfo _fieldInfo;
        private readonly PropertyInfo _propertyInfo;

        public FieldOrPropertyMemberInfo(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        public FieldOrPropertyMemberInfo(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public string Name
        {
            get
            {
                if (_fieldInfo != null) return _fieldInfo.Name;
                return _propertyInfo.Name;
            }
        }

        public Type Type
        {
            get
            {
                if (_fieldInfo != null) return _fieldInfo.FieldType;
                return _propertyInfo.PropertyType;
            }
        }

        public bool IsSerializable
        {
            get
            {
                if (_fieldInfo != null)
                {
                    return ContainsAttributeSpecifyingCandidates(_fieldInfo.GetCustomAttributes(true));
                }
                return ContainsAttributeSpecifyingCandidates(_propertyInfo.GetCustomAttributes(true));
            }
        }

        public object GetValueFor(object instance)
        {
            if (_fieldInfo != null)
            {
                return _fieldInfo.GetValue(instance);
            }
            return _propertyInfo.GetValue(instance, null);
        }

        public void SetValueFor(object instance, object value)
        {
            if (_fieldInfo != null)
            {
                _fieldInfo.SetValue(instance, value);
            }
            else
            {
                _propertyInfo.SetValue(instance, value, null);
            }
        }

        private static bool ContainsAttributeSpecifyingCandidates(object[] attributes)
        {
            if (attributes == null || attributes.Length == 0) return false;
            return attributes.OfType<DataMemberAttribute>().Any();
        }

        protected bool Equals(FieldOrPropertyMemberInfo other)
        {
            return Equals(_fieldInfo, other._fieldInfo) && Equals(_propertyInfo, other._propertyInfo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FieldOrPropertyMemberInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_fieldInfo != null ? _fieldInfo.GetHashCode() : 0)*397) ^ (_propertyInfo != null ? _propertyInfo.GetHashCode() : 0);
            }
        }

        public static bool operator ==(FieldOrPropertyMemberInfo left, FieldOrPropertyMemberInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FieldOrPropertyMemberInfo left, FieldOrPropertyMemberInfo right)
        {
            return !Equals(left, right);
        }
    }
}