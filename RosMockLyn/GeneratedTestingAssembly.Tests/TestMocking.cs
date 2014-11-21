using System;

using AssemblyWithInterfaces;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using RosMockLyn.Mocking;

namespace GeneratedTestingAssembly.Tests
{

    [TestClass]
    public class TestMocking
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

        [TestMethod]
        public void Returns_ShouldNotCountAsReceived()
        {
            var someInterface = Mock.For<ISomeInterface>();

            someInterface.IntCall().Returns(2);

            someInterface.Received(0).IntCall();
        }
    }
}
