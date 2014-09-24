using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Shapeshifter;

namespace ModelVersion1
{
    [Shapeshifter]
    [DataContract]

    public class Customer
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Address HomeAddress { get; set; }
        
        [DataMember]
        List<Order> Orders { get; set; }
    }
}
