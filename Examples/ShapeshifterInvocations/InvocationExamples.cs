using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapeshifter;
using NUnit.Framework;

namespace ShapeshifterInvocations
{
    [TestFixture]
    public class InvocationExamples
    {
        [Test]
        public void CustomerConversionFromV1ToV2()
        {
            var v1Customer = new ModelVersion1.Customer()
            {
                Name = "Joe the Lion",
                Orders = new List<ModelVersion1.Order>(),
                HomeAddress = "Z1111 CityPart Some Street 23."
            };

            var serializedFormat = new ShapeshifterSerializer<ModelVersion1.Customer>().Serialize(v1Customer);

            var v2Customer = new ShapeshifterSerializer<ModelVersion2.Customer>().Deserialize(serializedFormat);

            Debug.Print(v2Customer.GetType().ToString());
            Debug.Print(((ModelVersion2.IndividualCustomer)v2Customer).Name);
            Debug.Print(((ModelVersion2.IndividualCustomer)v2Customer).HomeAddress.Zip);

        }
    }
}
