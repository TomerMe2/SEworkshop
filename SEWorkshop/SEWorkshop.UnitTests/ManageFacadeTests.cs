using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SEWorkshop.Facades;
using SEWorkshop.Models;
using SEWorkshop.Adapters;
using System.Linq;
using SEWorkshop.Exceptions;

namespace SEWorkshop.UnitTests
{
    [TestFixture]
    class ManageFacadeTests
    {
        private IManageFacade Facade { get; set; }

        [OneTimeSetUp]
        public void Init()
        {
            Facade = ManageFacade.GetInstance();
        }

        [Test]
        public void CreateStoreTest()
        {
        }
    }
}
