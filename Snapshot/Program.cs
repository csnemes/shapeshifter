using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using CLAP;

namespace Snapshot
{
    /// <summary>
    /// snapshot command line tool can be used to create and manage shapeshifter snapshots
    /// usage:
    /// snapshot command [command parameters] [general parameters]
    /// 
    /// Possible commands are:
    /// add = adds a new snapshot to the list of existing snapshots
    /// list = lists all snapshots 
    /// compare = compares the current snapshot to the history, outputing the differences
    /// 
    /// General parameters are valid for all commands. They are:
    /// -f or --file points to the snapshot file. If not specified the tool will use the working directory and the shapeshifter.snapshot file.
    /// -v or --verbose switch details the run
    /// 
    /// Command specific parameters:
    /// command add: 
    ///     -n or --name = specifies the name of the snapshot (required)
    ///     semicolon separated list of folders and/or files = snapshot will search these files for serialized classes (if folder is specified it will look for *.dll and *.exe files) 
    ///     -ex or --exclude = excludes the specified files
    /// 
    /// command list:
    ///     no additional parameters
    /// 
    /// command compare:
    ///     -r or --recurse = recursive run for folders
    ///     semicolon separated list of folders and/or files = snapshot will search these files for serialized classes for current snapshot 
    ///     -ex or --exclude = excludes the specified files
    /// Compare does not add the current snapshot to the history, it only compares the current snapshot to the history
    /// 
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            var app = new SnapshotApp();
            var parser = new Parser(typeof (SnapshotApp));
            try
            {
                Parser.Run(args, app);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invocation failed.");
                Console.WriteLine(ex.Message);
                Console.WriteLine(parser.GetHelpString());
            }
        }


    }
}
