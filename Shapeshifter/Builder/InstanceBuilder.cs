using System;
using System.Linq;
using System.Runtime.Serialization;
using Shapeshifter.Core;
using Shapeshifter.Core.Deserialization;
using Shapeshifter.Core.Detection;

namespace Shapeshifter.Builder
{
    /// <summary>
    ///     Class helping in object creation in custom deserializers.
    /// </summary>
    public class InstanceBuilder
    {
        private readonly TypeInspector _typeInspector;
        private object _instance;

        /// <summary>
        /// Creates a builder for the type given. If a reader is specified it tries to fill the new instance with the data in the reader.
        /// </summary>
        /// <param name="typeToBuild">Type to build.</param>
        /// <param name="reader">Reader with member data.</param>
        public InstanceBuilder(Type typeToBuild, IShapeshifterReader reader = null)
        {
            _typeInspector = new TypeInspector(typeToBuild);
            _instance = FormatterServices.GetUninitializedObject(typeToBuild);

            if (reader != null)
            {
                SetMembersByReflection(reader);
            }
        }

        private void SetMembersByReflection(IShapeshifterReader reader)
        {
            var packItemCandidates = _typeInspector.DataHolderMembers.ToDictionary(item => item.Name);

            foreach (var packItem in reader)
            {
                FieldOrPropertyMemberInfo target;
                if (packItemCandidates.TryGetValue(packItem.Key, out target))
                {
                    target.SetValueFor(_instance, ValueConverter.ConvertValueToTargetType(target.Type, packItem.Value));
                }
            }
        }

        /// <summary>
        /// Sets the given member of the instance to the value.
        /// </summary>
        /// <param name="name">The name of the target field or property.</param>
        /// <param name="value">The value to set.</param>
        public void SetMember(string name, object value)
        {
            if (_instance == null)
            {
                throw Exceptions.InstanceAlreadyGivenAway();
            }

            var memberInfo = _typeInspector.GetFieldOrPropertyMemberInfo(name);

            try
            {
                var convertedValue = ValueConverter.ConvertValueToTargetType(memberInfo.Type, value);
                memberInfo.SetValueFor(_instance, convertedValue);
            }
            catch (Exception ex)
            {
                throw Exceptions.FailedToSetValue(name, _typeInspector.Type, ex);
            }
        }

        /// <summary>
        /// Gets the value of a member (field or property) specified by name as T.
        /// </summary>
        /// <typeparam name="T">The type of the member.</typeparam>
        /// <param name="name">The name of the member sought.</param>
        /// <returns>The value of the given member.</returns>
        public T GetMember<T>(string name)
        {
            T result;

            if (_instance == null)
            {
                throw Exceptions.InstanceAlreadyGivenAway();
            }

            var memberInfo = _typeInspector.GetFieldOrPropertyMemberInfo(name);

            try
            {
                var value = memberInfo.GetValueFor(_instance);
                result = (T) ValueConverter.ConvertValueToTargetType(typeof(T), value);
            }
            catch (Exception ex)
            {
                throw Exceptions.FailedToGetValue(name, _typeInspector.Type, ex);
            }

            return result;
        }

        /// <summary>
        /// Returns the instance created and set-up by the builder. Once an instance is given away the builder cannot be used further.
        /// </summary>
        /// <returns>The instance built.</returns>
        public object GetInstance()
        {
            if (_instance == null)
            {
                throw Exceptions.InstanceAlreadyGivenAway();
            }

            var result = _instance;
            _instance = null;
            return result;
        }
    }

    /// <summary>
    ///     Class helping in object creation in custom deserializers for type T.
    /// </summary>
    public class InstanceBuilder<T> : InstanceBuilder
    {
        /// <summary>
        /// Creates a builder for the type T. If a reader is specified it tries to fill the new instance with the data in the reader.
        /// </summary>
        /// <param name="reader">Reader with member data.</param>
        public InstanceBuilder(IShapeshifterReader reader = null)
            : base(typeof(T), reader)
        {}

        /// <summary>
        /// Returns the instance created and set-up by the builder. Once an instance is given away the builder cannot be used further.
        /// </summary>
        /// <returns>The instance built.</returns>
        public new T GetInstance()
        {
            return (T)base.GetInstance();
        }
    }
}