﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Shapeshifter.Core.Deserialization
{
    /// <summary>
    ///     Data read from the serialized stream in key value pair form
    /// </summary>
    internal class ObjectProperties : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, object> _properties;

        public ObjectProperties(Dictionary<string, object> properties)
        {
            _properties = properties;
        }

        public object this[string key]
        {
            get { return _properties[key]; }
        }

        public uint Version
        {
            get { return Convert.ToUInt32((long) _properties[Constants.VersionKey]); }
        }

        public string TypeName
        {
            get { return (string) _properties[Constants.TypeNameKey]; }
        }

        public bool IsInPackformat
        {
            get
            {
                return _properties.ContainsKey(Constants.TypeNameKey) && _properties.ContainsKey((Constants.VersionKey));
            }
        }

        public bool IsBuiltInTypeInPackformat
        {
            get
            {
                return _properties.ContainsKey(Constants.TypeNameKey) && _properties.ContainsKey((Constants.ValueKey));
            }
        }

        private object Value
        {
            get { return _properties[Constants.ValueKey]; }
        }

        public object GetBuiltInTypeUnpacked()
        {
            //TODO check if BuiltInType
            var type = Type.GetType(TypeName);
            return ImplicitConversionHelper.ConvertValue(type, Value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }
    }
}