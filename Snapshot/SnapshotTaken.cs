using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Snapshot
{
    public class SnapshotTaken
    {
        private readonly IEnumerable<string> _includedPaths;
        private readonly IEnumerable<string> _excludedNames;
        private readonly string _name;
        private List<string> _assembliesToParse;
        private Shapeshifter.SchemaComparison.Snapshot _snapshot;
        private readonly bool _verbose;

        private SnapshotTaken(string name, IEnumerable<string> includedPaths, IEnumerable<string> excludedNames, bool verbose)
        {
            //Validate excluded filenames
            var errorItem = excludedNames.FirstOrDefault(item => !String.IsNullOrEmpty(Path.GetDirectoryName(item)));
            if (errorItem != null)
            {
                throw new ApplicationException(String.Format("Excluded file name {0} should not contain directory information.", errorItem));
            }

            _name = name;
            _includedPaths = includedPaths;
            _excludedNames = excludedNames;
        }

        public static SnapshotTaken TakeSnapshot(string name, IEnumerable<string> includedPaths, IEnumerable<string> excludedNames, bool verbose)
        {
            var recorder = new SnapshotTaken(name, includedPaths, excludedNames ?? new String[0], verbose);
            recorder.Take();
            return recorder;
        }

        public List<string> AssembliesParsed
        {
            get { return _assembliesToParse; }
        }

        public Shapeshifter.SchemaComparison.Snapshot Snapshot
        {
            get { return _snapshot; }
        }

        private void Take()
        {
            var result = new List<string>();
            foreach (var includedPath in _includedPaths)
            {
                result.AddRange(GetFilteredFilesFromPath(includedPath));
            }
            _assembliesToParse = result.Distinct().ToList();
            _snapshot = Shapeshifter.SchemaComparison.Snapshot.Create(_name, _assembliesToParse);
        }

        private IEnumerable<string> GetFilteredFilesFromPath(string aPath)
        {
            try
            {
                var directory = Path.GetDirectoryName(aPath);
                var fileName = Path.GetFileName(aPath);

                if (!Path.IsPathRooted(directory))
                {
                    directory = Path.Combine(Directory.GetCurrentDirectory(), directory);
                }

                var files = Directory.GetFiles(directory, fileName, SearchOption.TopDirectoryOnly);
                var filesPath = files.Where(file => !DoesMatchAnyExcludedFileName(file));

                foreach (var path in filesPath)
                {
                    Console.WriteLine(path);
                }

                return filesPath;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(String.Format("Failed to process input path {0} with error {1}", aPath, ex.Message));
            }
        }

        private bool DoesMatchAnyExcludedFileName(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            foreach (var excludedName in _excludedNames)
            {
                var excludedFiles = Directory.GetFiles(directory, excludedName, SearchOption.TopDirectoryOnly);
                if (excludedFiles.Contains(filePath)) return true;
            }
            return false;
        }
    }
}
