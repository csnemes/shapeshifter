using System.Linq;
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
        public void CompareToBase_MultipleBases_GroupBySnapshot_FromOldestToNewer()
        {
            var baseSnapshot = Snapshot.Create("Base", typeof(Order));
            var newSnapshot = Snapshot.Create("New", typeof(NewOrder));
            var newerSnapshot = Snapshot.Create("Newer", typeof(NewerVersion.NewOrder));

            var diff = newerSnapshot.CompareToBase(new[] { baseSnapshot, newSnapshot });

            var result = diff.GroupBySnapshotName();
            result.ToArray()[0].Key.Should().Be("Base");
            result.ToArray()[1].Key.Should().Be("New");
        }

        [Test]
        public void CompareToBase_MultipleBases_SameMissingItem_DetectedOnlyInTheOldest()
        {
            //Having the same serializable class in multiple base snapshots (without deserializer) might be detected for each
            //base snapshot, but we only need one error message, for the oldest snapshot 

            var baseSnapshot = Snapshot.Create("Base", typeof(Order));
            var newSnapshot = Snapshot.Create("New", typeof(NewOrder));
            var newerSnapshot = Snapshot.Create("Newer", typeof(NewerVersion.NewOrder));

            var diff = newerSnapshot.CompareToBase(new[] { baseSnapshot, newSnapshot });

            var result = diff.GroupByPackformatName();
            result["OrderItem"].Count().Should().Be(1);
        }

        [Test]
        public void CompareToBase_MultipleBases_MultipleVersionChangesOnTheSameClass_AreDetected()
        {
            var snapshot1 = Snapshot.Create("Base", typeof(TestClass));
            var snapshot2 = Snapshot.Create("New", typeof(Version2.TestClass));
            var snapshot3 = Snapshot.Create("Newer", typeof(Version3.TestClass));

            var diff = snapshot3.CompareToBase(new[] { snapshot1, snapshot2 });

            var result = diff.GroupByPackformatName();
            result["TestClass"].Count().Should().Be(2);
            result["TestClass"].ToArray()[0].SnapshotName.Should().Be("Base");
            result["TestClass"].ToArray()[1].SnapshotName.Should().Be("New");
        }
    }
}
