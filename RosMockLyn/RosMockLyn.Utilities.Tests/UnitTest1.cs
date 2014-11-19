using System;

using GeneratedTestingAssembly;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace RosMockLyn.Utilities.Tests
{

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Returns_UsedTheWrongWay()
        {
            var someInterface = Mock.For<ISomeInterface>();
            someInterface.IntCall();
            Assert.ThrowsException<InvalidOperationException>(() => someInterface.Returns(someInterface));
        }

        [TestMethod]
        public void Returns_CorrectlyUsed()
        {
            var someInterface = Mock.For<ISomeInterface>();

            someInterface.IntCall().Returns(5);
        
            int result = someInterface.IntCall();

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void Received_CorrectlyUsed()
        {
            var someInterface = Mock.For<ISomeInterface>();
            someInterface.VoidCall();

            someInterface.Received(1).VoidCall();
        }
    }
}
