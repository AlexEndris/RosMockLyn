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
        public void Sandbox()
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

        public static class Arg
        {
            public static T Is<T>()
            {
                return default(T);
            }
        }

        //[TestMethod]
        //public void Returns_UsedTheWrongWay()
        //{
        //    var someInterface = Mock.For<ISomeInterface>();
        //    someInterface.IntCall();
        //    Assert.ThrowsException<InvalidOperationException>(() => someInterface.Returns(someInterface));
        //}

        //[TestMethod]
        //public void Returns_CorrectlyUsed()
        //{
        //    var someInterface = Mock.For<ISomeInterface>();

        //    someInterface.IntCall().Returns(5);
        
        //    int result = someInterface.IntCall();

        //    Assert.AreEqual(5, result);
        //}

        [TestMethod]
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
