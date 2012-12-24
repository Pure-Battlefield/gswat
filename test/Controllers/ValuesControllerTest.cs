using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using core;

namespace test.Controllers
{
    [TestClass]
    public class ValuesControllerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var mockCore = new Mock<ICore>();
        }
    }
}
