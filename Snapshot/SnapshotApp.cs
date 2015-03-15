using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CLAP;
using Shapeshifter.SchemaComparison;

namespace Snapshot
{
    public class SnapshotApp
    {
        private string _userSpecifiedSnapshotFilePath;
        private bool _verbose;

        [Verb(Description = "Takes a snapshot of the given assemblies and stores it in the snapshot file.")]
        public void Add(
            [Required]
            [Description("The name of the snapshot")]
            string name,
            [Required]
            [Description("Comma separated list of files to be parsed during snapshot creation. Wildcards are accepted.")]
            string[] include,
            [Description("Comma separated list of files to be excluded during snapshot creation. Only specify file names, not paths. Wildcards are accepted.")]
            [Aliases("x")]
            string[] exclude,
            [Description("If specified the snapshot won't be saved to the snapshot file.")]
            [DefaultValue(false)]
            bool whatif,
            [Description("If specified the snapshot will replace an existing one with the same name (Use only in special cases to fixup messed snapshots).")]
            [DefaultValue(false)]
            bool replace)
        {
            var path = GetSnapshotPath();
            var snapshotHistory = System.IO.File.Exists(path) ? SnapshotHistory.LoadFrom(path) : SnapshotHistory.Empty;

            var snapshot = SnapshotTaken.TakeSnapshot(name, include, exclude ?? new string[0], _verbose);

            Console.WriteLine("Assemblies parsed:");
            snapshot.AssembliesParsed.ForEach(Console.WriteLine);

            if (replace)
            {
                snapshotHistory.ReplaceSnapshot(snapshot.Snapshot);
            }
            else
            {
                snapshotHistory.AddSnapshot(snapshot.Snapshot);
            }

            if (!whatif)
            {
                snapshotHistory.SaveTo(path);
                Console.WriteLine("Snapshot taken.");
            }
        }

        [Verb(Description = "Lists the existing snapshots in the snapshot file.")]
        public void List()
        {
            var path = GetSnapshotPath();
            if (!System.IO.File.Exists(path))
            {
                throw new ApplicationException(String.Format("Snapshot file on path {0} not found.", path));
            }
            var history = SnapshotHistory.LoadFrom(path);

            Console.WriteLine("Snapshots:");

            foreach (var snapshot in history)
            {
                Console.WriteLine(String.Format("{0} taken at {1}", snapshot.Name,  snapshot.TakenDate));
            }

            Console.WriteLine("Listing done.");
        }

        [Verb(Description = "Compares the current snapshot againts the snapshots stored in a snapshot file.")]
        public void Compare(
            [Required]
            [Description("Comma separated list of files to be parsed during snapshot creation. Wildcards are accepted.")]
            string[] include,
            [Description("Comma separated list of files to be excluded during snapshot creation. Only specify file names, not paths. Wildcards are accepted.")]
            [Aliases("x")]
            string[] exclude,
            [Description("Defines the name of the oldest snapshot taken into account.")]
            string oldestSnapshot,
            [Description("Returns with exit code 1 if there is a difference between the snapshots.")]
            [Aliases("failOnDifference")]
            bool haltOnDifference)
        {
            var path = GetSnapshotPath();
            if (!System.IO.File.Exists(path))
            {
                throw new ApplicationException(String.Format("Snapshot file on path {0} not found.", path));
            }
            var history = SnapshotHistory.LoadFrom(path);
            var snapshot = SnapshotTaken.TakeSnapshot("ActualSnapshot", include, exclude ?? new string[0], _verbose);

            var snapshotsToCheckAgainst = oldestSnapshot != null
                ? history.SkipWhile(snp => !snp.Name.Equals(oldestSnapshot, StringComparison.InvariantCultureIgnoreCase))
                : history;

            var difference = snapshot.Snapshot.CompareToBase(snapshotsToCheckAgainst);

            Console.WriteLine("The following differences are detected:");
            Console.WriteLine(difference.GetHumanReadableResult(_verbose));

            if (haltOnDifference)
            {
                Environment.Exit(1);
            }
        }

        [Verb(Description = "View current snaphsot's types with versions.")]
        public void View(
            [Required]
            [Description("Comma separated list of files to be parsed during snapshot creation. Wildcards are accepted.")]
            string[] include,
            [Description("Comma separated list of files to be excluded during snapshot creation. Only specify file names, not paths. Wildcards are accepted.")]
            [Aliases("x")]
            string[] exclude
            )
        {
            var snapshot = SnapshotTaken.TakeSnapshot("ActualSnapshot", include, exclude ?? new string[0], _verbose);
            Console.WriteLine("Actual snapshot contains:");
            foreach (var serializerInfo in snapshot.Snapshot.DefaultSerializers.OrderBy(item => item.TypeFullName))
            {
                Console.WriteLine("{0} => {1}[{2}]", serializerInfo.TypeFullName, serializerInfo.PackformatName, serializerInfo.Version);
            }
        }

        [Verb(Description = "Deletes a snapshot from the snapshot file with the given name.")]
        public void Delete(
            [Required]
            [Description("The name of the snapshot")]
            string name,
            [Description("If specified the snapshot won't be saved to the snapshot file.")]
            [DefaultValue(false)]
            bool whatif)
        {
            var path = GetSnapshotPath();
            if (!System.IO.File.Exists(path))
            {
                throw new ApplicationException(String.Format("Snapshot file on path {0} not found.", path));
            }
            var history = SnapshotHistory.LoadFrom(path);

            history.RemoveSnapshot(name);

            if (!whatif)
            {
                history.SaveTo(path);
                Console.WriteLine("Snapshot deleted.");
            }
        }

        [Global(Aliases = "f",
            Description =
                "Specifies the snapshot file location to be used during command execution. If not specified the working directory and the shapeshifter.snapshot name will be used as default."
            )]
        public void File(string file)
        {
            _userSpecifiedSnapshotFilePath = file;
        }

        [Global(Aliases = "v",
            Description =
                "Prints detailed information on tool run."
            )]
        public void Verbose(bool verbose)
        {
            _verbose = verbose;
        }

        [Empty]
        private void Empty()
        {
            var assemblyInfo = Assembly.GetExecutingAssembly().GetName();
            Console.WriteLine("Shapeshifter's snapshot creating tool. Version {0}.", assemblyInfo.Version);
            Console.WriteLine("Try -? or help for more information.");
           
        }

        [Help(Aliases = "?,h")]
        private void Help(string help)
        {
            var assemblyInfo = Assembly.GetExecutingAssembly().GetName();
            Console.WriteLine("Shapeshifter's snapshot creating tool. Version {0}.", assemblyInfo.Version);
            Console.WriteLine("Usage (Use one of the verbs below):");
            Console.WriteLine(help);
            Examples();
        }

        private void Examples()
        {
            Console.WriteLine("Examples:");
            Console.WriteLine();
            Console.WriteLine(@"snapshot add -n:V1.0 -i:c:\mysource\*.dll");
            Console.WriteLine();
            Console.WriteLine("Adds a new snapshot to the shapeshifter.snapshot file in the current working folder. The snapshot's name will be V1.0 and it will contain " +
                              "the snapshot of all the dlls in the mysource folder.");
            Console.WriteLine();
            Console.WriteLine(@"snapshot add -n:V2.0 -i:c:\mysource\*.dll,c:\mysource\*.exe -x:Test* -file:c:\mysource\myapp.snapshot");
            Console.WriteLine();
            Console.WriteLine("Adds a new snapshot to the myapp.snapshot file. The snapshot's name will be V2.0 and it will contain " +
                              "the snapshot of all the dlls and exes in the mysource folder except those starting with Test.");
        }

        [Error]
        private void HandleError(ExceptionContext context)
        {
            Console.WriteLine("Invocation failed. Try -? or help for more information.");
            Console.WriteLine(context.Exception.Message);
            var loaderException = context.Exception as ReflectionTypeLoadException;
            if (loaderException != null)
            {
                foreach (var exception in loaderException.LoaderExceptions)
                {
                    Console.WriteLine(exception.Message);
                }
            }
            //
            if (_verbose)
            {
                Console.WriteLine(new string('-', 80));
                Console.WriteLine(context.Exception.ToString()); 
            }

            Environment.Exit(-1);
        }


        private string GetSnapshotPath()
        {
            var path = _userSpecifiedSnapshotFilePath ?? Path.Combine(Directory.GetCurrentDirectory(), "shapeshifter.snapshot");
            if (_verbose) Console.WriteLine(String.Format("SnapshotPath:{0}", path));
            return path;
        }

    }
}
