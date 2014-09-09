using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace Snapshot
{
    public class Options
    {

        [VerbOption("take", HelpText = "Takes a snapshot.")]
        public TakeOptions TakeVerb { get; set; }

        
        [VerbOption("show", HelpText = "Displays the content of a snapshot.")]
        public ShowOptions ShowVerb { get; set; }


        [HelpVerbOption]
        public string GetVerbUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }

    }
}
