using System;

using AssemblyWithInterfaces;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using RosMockLyn.Mocking;

namespace GeneratedTestingAssembly.Tests
{

    [TestClass]
    public class TestMocking
    {
        [TestMethod, TestCategory("Using mocking framework")]
        public void Test_Methods()
        {
            var someInterface = Mock.For<ISomeInterface>();

            someInterface.Setup(x => x.IntCall()).Returns(100);
            someInterface.Setup(
                x => x.ReturnParameters(1, 2, "3")).Returns(123);
            someInterface.Setup(x => x.Parameters(1,2,"1"))
                         .Throws<InvalidCastException>();

            someInterface.VoidCall();
            var intCall = someInterface.IntCall();
            intCall = someInterface.ReturnParameters(1, 2, "3");

            someInterface.Received(x => x.VoidCall()).AtLeastOne();
            someInterface.Received(x => x.IntCall()).One();
            Assert.ThrowsException<InvalidCastException>(() => someInterface.Parameters(1, 2, "1"));
        }

        [TestMethod, TestCategory("Using mocking framework")]
        public void Test_Properties()
        {
            var someInterface = Mock.For<ISomeInterface>();

            someInterface.IntNormalProperty = 100;
            someInterface.Setup(x => x.IntReadonlyProperty).Returns(200);

            Assert.AreEqual(100, someInterface.IntNormalProperty);
            Assert.AreEqual(200, someInterface.IntReadonlyProperty);
        }

        [TestMethod, TestCategory("Using mocking framework")]
        public void Test_Indexer()
        {
            var someInterface = Mock.For<ISomeInterface>();

            someInterface["1"] = 100;
            someInterface.Setup(x => x[1]).Returns(200);

            Assert.AreEqual(100, someInterface["1"]);
            Assert.AreEqual(200, someInterface[1]);
        }

        //[TestMethod]
        //public void Returns_CorrectlyUsed()
        //{
        //    var someInterface = Mock.For<ISomeInterface>();

        //    someInterface.IntCall().Returns(5);
        
        //    int result = someInterface.IntCall();

        //    Assert.AreEqual(5, result);
        //}

        [TestMethod, TestCategory("Using mocking framework")]
        public void Received_CorrectlyUsed()
        {
            var someInterface = Mock.For<ISomeInterface>();

            someInterface.VoidCall();

            someInterface.Received(x => x.VoidCall()).AtLeastOne();
        }

        //[TestMethod]
        //public void Returns_ShouldNotCountAsReceived()
        //{
        //    var someInterface = Mock.For<ISomeInterface>();

        //    someInterface.IntCall().Returns(2);

        //    someInterface.Received(0).IntCall();
        //}
    }
}
