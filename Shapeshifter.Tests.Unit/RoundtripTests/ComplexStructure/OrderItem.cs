using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Shapeshifter.Tests.Unit.RoundtripTests.ComplexStructure
{
    [DataContract]
    public class OrderItem
    {
        [DataMember]
        public Product Product { get; set; }

        [DataMember]
        public OrderItemType Type { get; set; }

        [DataMember]
        public int Amount { get; set; }

        [DataMember]
        public TimeSpan? Delay { get; set; }

        [Serializer(typeof(TimeSpan))]
        public static void TimeSpanSerialize(IShapeshifterWriter writer, TimeSpan timeSpan)
        {
            writer.Write("value", timeSpan.Ticks);
        }

        [Deserializer(typeof(TimeSpan))]
        public static object TimeSpanDeserialize(IShapeshifterReader reader)
        {
            var valueAsTicks = reader.Read<long>("value");
            return new TimeSpan(valueAsTicks);
        }
    }

    
    public enum OrderItemType
    {
        Simple = 1,
        Delayed = 2
    }
}
