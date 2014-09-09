using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices.ComTypes;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.SchemaComparison;

namespace Shapeshifter.Tests.Unit.SchemaComparison
{
    [TestFixture]
    public class SnapshotHistoryTests
    {
        [Test]
        public void SaveHistory_SavesData()
        {
            var history = SnapshotHistory.Empty;
            using (var memoryStream = new MemoryStream())
            {
                var snapshot = Snapshot.Create("V1", new[] { typeof(Order) });
                history.AddSnapshot(snapshot);
                history.SaveTo(memoryStream);

                memoryStream.Length.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public void SaveHistory_SavedDataCanBeLoadedBack()
        {
            var history = SnapshotHistory.Empty;
            using (var memoryStream = new MemoryStream())
            {
                var snapshot = Snapshot.Create("V1", new[] { typeof(Order) });
                history.AddSnapshot(snapshot);
                history.SaveTo(memoryStream);

                memoryStream.Position = 0;

                var newHistory = SnapshotHistory.LoadFrom(memoryStream);
                newHistory.Count.Should().Be(1);
            }
        }


        private void PrintStream(Stream stream)
        {
            stream.Position = 0;
            var sr = new StreamReader(stream);
            var result = sr.ReadToEnd();
            Debug.WriteLine(result);
        }


    }
}
