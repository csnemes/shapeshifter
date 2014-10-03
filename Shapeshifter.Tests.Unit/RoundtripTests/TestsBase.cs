using System.Reflection;
using System.Runtime.Serialization;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    public abstract class TestsBase
    {
        protected static ShapeshifterSerializer<T> GetSerializer<T>()
        {
            return new ShapeshifterSerializer<T>(new[] {Assembly.GetExecutingAssembly()});
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
