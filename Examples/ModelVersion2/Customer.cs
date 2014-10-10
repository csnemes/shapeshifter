using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Shapeshifter;

namespace ModelVersion2
{
    [ShapeshifterRoot]
    [DataContract]
    [KnownType(typeof(IndividualCustomer))]
    public abstract class Customer
    {
        [DataMember]
        List<Order> Orders { get; set; }
    }
}
