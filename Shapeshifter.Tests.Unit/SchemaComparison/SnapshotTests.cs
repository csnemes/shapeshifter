using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.SchemaComparison;
using Shapeshifter.Tests.Unit.SchemaComparison.Version1;

namespace Shapeshifter.Tests.Unit.SchemaComparison
{
    [TestFixture]
    public class SnapshotTests
    {
        [Test]
        public void CreateSnapshot()
        {
            var snapshot = Snapshot.Create("First", typeof (Order));
            snapshot.Name.Should().Be("First");
        }

        [Test]
        public void CompareToBase_SameVersions_HasNoMissingItems()
        {
            var baseSnapshot = Snapshot.Create("Base", typeof(Order));
            var newSnapshot = Snapshot.Create("New", typeof(Order));

            var diff = newSnapshot.CompareToBase(baseSnapshot);

            diff.Should().NotBeNull();
            diff.HasMissingItem.Should().BeFalse();
        }

        [Test]
        public void CompareToBase_DifferentVersions_HasMissingItems()
        {
            var baseSnapshot = Snapshot.Create("Base", typeof (Order));
            var newSnapshot = Snapshot.Create("New", typeof(NewOrder));

            var diff = newSnapshot.CompareToBase(baseSnapshot);

            diff.Should().NotBeNull();
            diff.HasMissingItem.Should().BeTrue();
            diff.GroupBySnapshotName().Should().ContainKey("Base");
        }

        [Test]
        public void CompareToBase_NameChange_Detected()
        {
            var baseSnapshot = Snapshot.Create("Base", typeof(Order));
            var newSnapshot = Snapshot.Create("New", typeof(NewOrder));

            var diff = newSnapshot.CompareToBase(baseSnapshot);
            var result = diff.GroupBySnapshotName();
            result["Base"].Select(item => item.MissingPackformatName).Should().Contain("Order");
        }

        [Test]
        public void CompareToBase_MissingClass_Detected()
        {
            var baseSnapshot = Snapshot.Create("Base", typeof(Order));
            var newSnapshot = Snapshot.Create("New", typeof(NewOrder));

            var diff = newSnapshot.CompareToBase(baseSnapshot);
            var result = diff.GroupBySnapshotName();
            result["Base"].Select(item => item.MissingPackformatName).Should().Contain("Address");
        }

        [Test]
        public void CompareToBase_MultipleBases_GroupBySnapshot_FromNewerToOldest()
        {
            var baseSnapshot = Snapshot.Create("Base", typeof(Order));
            Thread.Sleep(100);
            var newSnapshot = Snapshot.Create("New", typeof(NewOrder));
            Thread.Sleep(100);
            var newerSnapshot = Snapshot.Create("Newer", typeof(NewerVersion.NewOrder));

            var diff = newerSnapshot.CompareToBase(new[] { baseSnapshot, newSnapshot });

            var result = diff.GroupBySnapshotName();
            result.ToArray()[0].Key.Should().Be("New");
            result.ToArray()[1].Key.Should().Be("Base");
        }

        [Test]
        public void CompareToBase_MultipleBases_SameMissingItem_DetectedOnlyInTheOldest()
        {
            //Having the same serializable class in multiple base snapshots (without deserializer) might be detected for each
            //base snapshot, but we only need one error message, for the oldest snapshot 

            var baseSnapshot = Snapshot.Create("Base", typeof(Order));
            Thread.Sleep(100);
            var newSnapshot = Snapshot.Create("New", typeof(NewOrder));
            Thread.Sleep(100);
            var newerSnapshot = Snapshot.Create("Newer", typeof(NewerVersion.NewOrder));

            var diff = newerSnapshot.CompareToBase(new[] { baseSnapshot, newSnapshot });

            var result = diff.GroupByPackformatName();
            result["OrderItem"].Count().Should().Be(1);
        }

        [Test]
        public void CompareToBase_MultipleBases_MultipleVersionChangesOnTheSameClass_AreDetected()
        {
            var snapshot1 = Snapshot.Create("Base", typeof(TestClass));
            Thread.Sleep(100);
            var snapshot2 = Snapshot.Create("New", typeof(Version2.TestClass));
            Thread.Sleep(100);
            var snapshot3 = Snapshot.Create("Newer", typeof(Version3.TestClass));

            var diff = snapshot3.CompareToBase(new[] { snapshot1, snapshot2 });

            var result = diff.GroupByPackformatName();
            result["TestClass"].Count().Should().Be(2);
            result["TestClass"].ToArray()[0].SnapshotName.Should().Be("New");
            result["TestClass"].ToArray()[1].SnapshotName.Should().Be("Base");
        }

        [Test]
        public void SaveAndLoad_Success()
        {
            string filePath = Path.Combine(Path.GetTempPath(), @"shapeshifter_test.xml");

            var snapshot = Snapshot.Create("First", typeof(Order));
            using (var stream = File.Open(filePath, FileMode.Create))
            {
                snapshot.SaveTo(stream);
            }

            var loadedSnapshot = Snapshot.LoadFrom(filePath);
            loadedSnapshot.Name.Should().Be("First");

            loadedSnapshot.CompareToBase(snapshot).HasMissingItem.Should().BeFalse();
        }

        [Test]
        public void Create_FromFile()
        {
            var dllPath = Path.Combine(Path.GetTempPath(), "ModelVersion1.dll");
            var targetPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            SaveEmbeddedResourceToFile("Shapeshifter.Tests.Unit.SchemaComparison.Resources.ModelVersion1.dll", dllPath);
        
            var snapshot = Snapshot.Create("Test", new [] { dllPath });
            snapshot.SaveTo(targetPath);

            File.Exists(targetPath).Should().BeTrue();
        }

        private void SaveEmbeddedResourceToFile(string resourceName, string targetPath)
        {
            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (Stream output = File.Create(targetPath))
            {
                input.CopyTo(output);
            }
        }
    }
}
