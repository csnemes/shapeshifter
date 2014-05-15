using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    public abstract class TestsBase
    {
        protected IShapeshifterSerializer<T> GetSerializer<T>()
        {
            return new ShapeshifterSerializer<T>();
        }
        
        protected IShapeshifterSerializer<T> GetSerializer<T>(params IPackformatSurrogateConverter[] surrogateConverters)
        {
            return new ShapeshifterSerializer<T>(null, surrogateConverters);
        }

        protected bool StringArrayEquals(string[] arr1, string[] arr2)
        {
            if (arr1.Length != arr2.Length) return false;
            for (int idx = 0; idx < arr1.Length; idx++)
            {
                if (!Equals(arr1[idx], arr2[idx])) return false;
            }
            return true;
        }

        [DataContract]
        protected class Person
        {
            [DataMember]
            public string Name { get; set; }
        }
    }
}
