using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using Shapeshifter.Tests.Unit.RoundtripTests.ComplexStructure;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class ComplexStructureTests
    {
        private readonly Randomizer _randomizer;

        public ComplexStructureTests()
        {
            _randomizer = Randomizer.GetRandomizer(typeof (ComplexStructureTests));
        }


        [Test, Explicit, Category("Performance"), Category("Manual")]
        public void MesurePerformance()
        {
            var customer = CreateCustomer(10, 5);
            Debug.WriteLine("Cust10_Json:{0}", MesureJsonPerformance(customer));
            Debug.WriteLine("Cust10_Shapeshifter:{0}", MesureShapeshifterPerformance(customer));
            customer = CreateCustomer(50, 10);
            Debug.WriteLine("Cust50_Json:{0}", MesureJsonPerformance(customer));
            Debug.WriteLine("Cust50_Shapeshifter:{0}", MesureShapeshifterPerformance(customer));
        }

        private TimeSpan MesureShapeshifterPerformance(Customer customer)
        {
            var watch = Stopwatch.StartNew();
            var shapeshifterSerializer = new ShapeshifterSerializer<Customer>();

            for (int cnt = 1; cnt < 1000; cnt++)
            {
                using (var stream = new MemoryStream())
                {
                    shapeshifterSerializer.Serialize(stream, customer);
                    stream.Seek(0, SeekOrigin.Begin);
                    var result = shapeshifterSerializer.Deserialize(stream);
                }
            }
            watch.Stop();
            return watch.Elapsed;
        }

        private TimeSpan MesureJsonPerformance(Customer customer)
        {
            var watch = Stopwatch.StartNew();
            var jsonSerializer = new JsonSerializer();

            for (int cnt = 1; cnt < 1000; cnt++)
            {
                using (var stream = new MemoryStream())
                {
                    var writer = new StreamWriter(stream);
                    jsonSerializer.Serialize(writer, customer);
                    writer.Flush();

                    stream.Seek(0, SeekOrigin.Begin);

                    var reader = new StreamReader(stream);
                    var result = jsonSerializer.Deserialize(reader, typeof(Customer));
                }
            }
            watch.Stop();
            return watch.Elapsed;
        }

       private Customer CreateCustomer(int numberOfOrders, int numberOfOrderItemsInASingleOrder)
        {
            var orders = new List<Order>();
            for (int idx = 1; idx < numberOfOrders; idx++)
            {
                orders.Add(CreateOrder(numberOfOrderItemsInASingleOrder));
            }

            return new Customer()
            {
                Address = CreateRandomAddress(),
                Id = Guid.NewGuid(),
                Name = "First Customer Ltd.",
                Orders = orders
            };
        }

        private Order CreateOrder(int numberOfOrderItems)
        {
            return new Order()
            {
                DeliveryAddress = CreateRandomAddress(),
                Description = "Order for the customer",
                OrderDate = new DateTime(2015,3,1),
                Items = CreateOrderItems(numberOfOrderItems)
            };
        }

        private List<OrderItem> CreateOrderItems(int numberOfOrderItems)
        {
            var items = new List<OrderItem>();

            for (int idx = 1; idx < numberOfOrderItems; idx++)
            {
                var delay = _randomizer.Next(1, 2) == 1 ? new TimeSpan(1, 0, 0, 0) : (TimeSpan?) null;
                var product = _randomizer.Next(1, 2) == 1
                    ? new Product("Prod1", "Product 1", 100)
                    : new Product("Prod2", "Product 2 desc", 200);

                items.Add(new OrderItem()
                {
                    Amount = idx,
                    Delay = delay,
                    Type = delay != null ? OrderItemType.Delayed : OrderItemType.Simple,
                    Product = product
                });
            }

            return items;
        }

        private Address CreateRandomAddress()
        {
            switch (_randomizer.Next(1, 3))
            {
                case 1:
                    return new Address()
                    {
                        City = "Budapest",
                        Street = "Akármi",
                        Zip = "H-1112"
                    };
                case 2:
                    return new Address()
                    {
                        City = "London",
                        Street = "Whatever",
                        Zip = "E-DR12W"
                    };
                default:
                    return null; 
            }
        }
    }
}
