using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shapeshifter.Core.Deserialization
{
    /// <summary>
    /// Used for storing a single instance of a class containing custom deserializer method. The instance is single per serialization/deserialization run. 
    /// So within a single Shapeshifter.Serialize the same instance of this class is used. This can be used to exchange information between deserializer methods during the
    /// deserialization.
    /// </summary>
    internal class SerializerInstanceStore
    {
        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();

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
    }
}
