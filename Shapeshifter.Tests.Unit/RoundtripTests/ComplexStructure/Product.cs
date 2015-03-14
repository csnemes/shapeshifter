using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Shapeshifter.Tests.Unit.RoundtripTests.ComplexStructure
{
    [DataContract]
    public class Product
    {
        public Product(string name, string description, double price)
        {
            Name = name;
            Description = description;
            Price = price;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public double Price { get; set; }
    }
}
