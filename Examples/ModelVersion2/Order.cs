using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ModelVersion2
{
    [DataContract]
    public class Order
    {
        [DataMember]
        public Address DeliveryAddress { get; set; }

        [DataMember]
        public int NumberOfOrderedWidgets { get; set; }

        [DataMember]
        public int NumberOfOrderedGadgets { get; set; }
    }
}
