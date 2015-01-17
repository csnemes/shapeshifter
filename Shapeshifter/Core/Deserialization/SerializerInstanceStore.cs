using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shapeshifter.Core.Deserialization
{
    /// <summary>
    /// Used for storing a single instance of a class containing custom deserializer method(s). The instance is single per serialization/deserialization run. 
    /// So within a single Shapeshifter.Serialize the same instance of this class is used. During the deserialization this can be used to exchange information either 
    /// between different deserializer methods, or between a single deserializer in different points in time.
    /// </summary>
    internal class SerializerInstanceStore
    {
        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        /// <summary>
        /// Returns the instance for the given type. That instance has at least one custom deserializer method.
        /// </summary>
        public object GetInstance(Type type)
        {
            if (!_instances.ContainsKey(type))
            {
                //create instance using default public constructor
                var constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor == null)
                {
                    throw Exceptions.TypeHasNoPublicDefaultConstructor(type);
                }
                try
                {
                    var instance = constructor.Invoke(null);
                    _instances[type] = instance;
                }
                catch (Exception ex)
                {
                    throw Exceptions.FailureWhenInvokingConstructor(type, ex);
                }
            }

            return _instances[type];
        }

        /// <summary>
        /// Notifies instances that the deserialization is about to end. The hierarchy is already built, if some instance values need
        /// fixup it is the right time to do so.
        /// </summary>
        /// <remarks>
        /// The method will look for a public instance method called DeserializationEnding without parameter and return value.
        /// <code>
        /// public void DeserializationEnding()
        /// {
        /// }
        /// </code>
        /// </remarks>
        public void NotifyInstancesOnDeserializationEnding()
        {
            foreach (var instanceAndType in _instances)
            {
                var method = instanceAndType.Key.GetMethod("DeserializationEnding", Type.EmptyTypes);
                if (method != null)
                {
                    try
                    {
                        method.Invoke(instanceAndType.Value, new object[0]);
                    }
                    catch (Exception ex)
                    {
                        throw Exceptions.FailedToRunDeserializationEnding(instanceAndType.Key, ex);
                    }
                }
            }
        }
    }
}
