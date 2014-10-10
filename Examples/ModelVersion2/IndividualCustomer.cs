using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Shapeshifter;
using Shapeshifter.Builder;

namespace ModelVersion2
{
    [ShapeshifterRoot]
    [DataContract]
    public class IndividualCustomer : Customer
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Address HomeAddress { get; set; }


        [Deserializer("Customer", 1050986160)]
        public static object DeserializerForOldVersion(IShapeshifterReader reader)
        {
            var builder = new InstanceBuilder<IndividualCustomer>();
            builder.SetMember("Name", reader.Read<string>("Name"));

            var oldAddress = reader.Read<string>("HomeAddress");

            builder.SetMember("HomeAddress", ConvertStringToAddress(oldAddress));

            builder.SetMember("Orders", reader.Read<List<Order>>("Orders"));
            return builder.GetInstance();
        }

        private static Address ConvertStringToAddress(string addressInStringFormat)
        {
            var split = addressInStringFormat.Split(' ');
            return new Address()
            {
                Zip = split[0],
                City = split[1],
                Street = String.Join(" ", split.Skip(2))
            };
        }
    }
}
