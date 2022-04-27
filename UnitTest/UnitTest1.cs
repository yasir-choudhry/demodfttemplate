using Microsoft.VisualStudio.TestTools.UnitTesting;
using DemoDftTemplate.Models;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            bool trueorfalse = true;
            bool swapexpected = false;

            bool swapresult;

            swapresult = TestClass.TestFunction(trueorfalse);

            Assert.AreEqual(swapexpected, swapresult);

            trueorfalse = false;
            swapexpected = true;


            swapresult = TestClass.TestFunction(trueorfalse);

            Assert.AreEqual(swapexpected, swapresult);
        }
    }
}
