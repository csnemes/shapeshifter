using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace Snapshot
{
    /// <summary>
    /// snapshot command line tool can be used to create and manage shapeshifter snapshots
    /// usage:
    /// snapshot command [command parameters]
    /// 
    /// snapshot take [-recurse] [-output:filename]
    /// 
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            string invokedVerb;
            object invokedVerbInstance;

            var options = new Options();
            if (!Parser.Default.ParseArguments(args, options,
              (verb, subOptions) =>
              {
                  // if parsing succeeds the verb name and correct instance
                  // will be passed to onVerbCommand delegate (string,object)
                  invokedVerb = verb;
                  invokedVerbInstance = subOptions;
              }))
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

        }


    }
}
