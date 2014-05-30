using System;
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
        public InstanceBuilder(IShapeshifterReader reader = null)
            : base(typeof(T), reader)
        {}

        public new T GetInstance()
        {
            return (T)base.GetInstance();
        }
    }
}