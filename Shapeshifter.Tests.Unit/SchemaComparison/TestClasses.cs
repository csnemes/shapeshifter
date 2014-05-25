using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Shapeshifter.Tests.Unit.SchemaComparison
{

    [DataContract]
    [Shapeshifter]
    internal class Order
    {
        [DataMember]
        public Address Address { get; set; }
        [DataMember]
        public List<OrderItem> Items { get; set; }
    }

    [DataContract]
    internal class Address
    {
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string Street { get; set; }
    }

    [DataContract]
    internal class OrderItem
    {
        [DataMember]
        public int ItemId { get; set; }

        [DataMember]
        public string Description { get; set; }
    }

    //New Version 

    [DataContract]
    [Shapeshifter]
    internal class NewOrder
    {
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public List<OrderItem> Items { get; set; }
    }

    //Even newer version
    namespace NewerVersion  //namespace used to enable using the same class name 
    {
        [DataContract]
        [Shapeshifter]
        internal class NewOrder
        {
            [DataMember]
            public string Address { get; set; }
            [DataMember]
            public List<BetterOrderItem> Items { get; set; }
        }
    }

    [DataContract]
    internal class BetterOrderItem
    {
        [DataMember]
        public int ItemId { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int Quantity { get; set; }
    }

    namespace Version1
    {
        [DataContract]
        [Shapeshifter]
        internal class TestClass
        {
            [DataMember]
            public int Property1 { get; set; }
        }
    }

    namespace Version2
    {
        [DataContract]
        [Shapeshifter]
        internal class TestClass
        {
            [DataMember]
            public int Property1 { get; set; }

            [DataMember]
            public int Property2 { get; set; }
        }
    }

    namespace Version3
    {
        [DataContract]
        [Shapeshifter]
        internal class TestClass
        {
            [DataMember]
            public int Property1 { get; set; }

            [DataMember]
            public string Property2 { get; set; }
        }
    }

}
