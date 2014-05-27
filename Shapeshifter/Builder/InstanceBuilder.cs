using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Shapeshifter.Core;
using Shapeshifter.Core.Deserialization;
using Shapeshifter.Core.Detection;

namespace Shapeshifter.Builder
{
    /// <summary>
    ///     Class helping in object creation
    /// </summary>
    public class InstanceBuilder
    {
        private readonly TypeInspector _typeInspector;
        private object _instance;

        public InstanceBuilder(Type typeToBuild)
        {
            _typeInspector = new TypeInspector(typeToBuild);
            _instance = FormatterServices.GetUninitializedObject(typeToBuild);
        }

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

    public class InstanceBuilder<T> : InstanceBuilder
    {
        public InstanceBuilder() : base(typeof(T))
        {}

        public new T GetInstance()
        {
            return (T)base.GetInstance();
        }
    }
}