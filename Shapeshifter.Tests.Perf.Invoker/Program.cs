using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapeshifter.Tests.Unit.RoundtripTests;

namespace Shapeshifter.Tests.Perf.Invoker
{
    class Program
    {
        static void Main(string[] args)
        {
            var tests = new ComplexStructureTests();
            tests.MesurePerformance();
        }
    }
}
