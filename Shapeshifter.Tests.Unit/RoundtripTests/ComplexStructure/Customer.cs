using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Shapeshifter.Tests.Unit.RoundtripTests.ComplexStructure
{
    [DataContract]
    [ShapeshifterRoot]
    public class Customer
    {
        [DataMember]
        public List<Order> Orders { get; set; }

        [DataMember]
        public Address Address { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Guid Id { get; set; }
    }
}
