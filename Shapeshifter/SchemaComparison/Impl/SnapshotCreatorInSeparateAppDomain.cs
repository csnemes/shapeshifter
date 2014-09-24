using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Shapeshifter.Core;

namespace Shapeshifter.SchemaComparison.Impl
{
    public static class SnapshotCreatorInSeparateAppDomain
    {
        public static Snapshot Create(string snapshotName, IEnumerable<string> assemblyPaths)
        {
            var domain = AppDomain.CreateDomain("snapshotMaker", null, AppDomain.CurrentDomain.SetupInformation);
            try
            {
                var remoteRef = domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, "Shapeshifter.SchemaComparison.Impl.SnapshotCreatorOtherSide") as SnapshotCreatorOtherSide;
                var result = remoteRef.CreateSnapshot(snapshotName, assemblyPaths.ToList(), null);
                return result;
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }
    }

    public class SnapshotCreatorOtherSide : MarshalByRefObject
    {
        private List<string> _basePaths;
 
        public Snapshot CreateSnapshot(string snapshotName, List<string> assemblyPaths, List<string> searchFolders)
        {
            try
            {
                _basePaths = (searchFolders ?? new List<string>()).Union(
                    assemblyPaths.Select(Path.GetDirectoryName).Distinct(StringComparer.InvariantCultureIgnoreCase)).ToList();
                
                //this is the remote domain
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;

                var assemblies = GetAssembliesFromPaths(assemblyPaths);

                return Snapshot.Create(snapshotName, assemblies);

            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
            }
        }

        private IEnumerable<Assembly> GetAssembliesFromPaths(IEnumerable<string> assemblyPaths)
        {
            return assemblyPaths.Select(Assembly.LoadFrom);
        }

        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            foreach (var basePath in _basePaths)
            {
                Assembly assembly;
                if (TryFindAssemblyByItsFullNameInAFolder(basePath, args.Name, out assembly))
                {
                    return assembly;
                }
            }

            return null;
        }

        private bool TryFindAssemblyByItsFullNameInAFolder(string folderPath, string assemblyFullName, out Assembly assembly)
        {
            assembly = null;
            var files = Directory.EnumerateFiles(folderPath, "*.dll").Concat(Directory.EnumerateFiles(folderPath, "*.exe"));
            foreach (var file in files)
            {
                try
                {
                    var fileAssemblyName = AssemblyName.GetAssemblyName(file);
                    if (fileAssemblyName.FullName.Equals(assemblyFullName))
                    {
                        assembly = Assembly.LoadFrom(file);
                        return true;
                    }
                }
                catch (System.BadImageFormatException)
                { }
            }
            return false;
        }
    }

}
