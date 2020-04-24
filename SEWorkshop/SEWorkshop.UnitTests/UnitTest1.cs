using System;
using NUnit.Framework;

namespace SEWorkshop.UnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            Console.WriteLine("Dummy test :)");
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}