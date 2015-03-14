using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Shapeshifter.Tests.Unit.RoundtripTests.ComplexStructure
{
    [DataContract]
    public class Order
    {
        [DataMember]
        public List<OrderItem> Items { get; set; }

        [DataMember]
        public DateTime OrderDate { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public Address DeliveryAddress { get; set; }
    }
}
