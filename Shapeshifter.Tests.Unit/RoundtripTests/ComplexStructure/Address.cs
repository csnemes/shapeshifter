using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Shapeshifter.Tests.Unit.RoundtripTests.ComplexStructure
{
    [DataContract]
    public class Address
    {
        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string Zip { get; set; }

        [DataMember]
        public string Street { get; set; }
    }
}
