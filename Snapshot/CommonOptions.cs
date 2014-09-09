using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;

namespace Snapshot
{
    public abstract class CommonOptions
    {
        [Option('q', "quiet",
   HelpText = "Suppress summary message.")]
        public bool Quiet { get; set; }
    }
}
