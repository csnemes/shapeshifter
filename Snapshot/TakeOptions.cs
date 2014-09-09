using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;

namespace Snapshot
{
    public class TakeOptions : CommonOptions
    {
        [Option('o', "output", Required = true, HelpText = "Output file to write to.")]
        public string OutputFile { get; set; }
    }
}
