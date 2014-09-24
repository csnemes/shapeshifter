using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Shapeshifter.Core;
using Shapeshifter.Core.Serialization;

namespace Shapeshifter.Tests.Unit.RoundtripTests
{
    [TestFixture]
    public class ListBasedClassTests : TestsBase
    {

        [Test]
        public void ClassWithMyList_EmptyListTest_ShouldThrow()
        {
            Action action = () => GetSerializer<ClassWithMyList>();
            action.ShouldThrow<ShapeshifterException>().Where(i => i.Id == Exceptions.DataContractAttributeMissingFromHierarchyId);
        }

        [DataContract]
        [Shapeshifter]
        private class ClassWithMyList
        {
            [DataMember]
            public List<MyList> MyListItems { get; set; }
        }


        [DataContract]
        private class MyList : List<string>
        {
            [DataMember]
            private string OtherItem { get; set; }
        }
    }


}
