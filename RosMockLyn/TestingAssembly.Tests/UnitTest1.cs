using System;

using GeneratedTestingAssembly;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Returns_UsedTheWrongWay()
        {
            var someInterface = Mock.For<ISomeInterface>();
            someInterface.IntCall();
            someInterface.Returns(someInterface);
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

    public static class Extensions
    {
        public static T Received<T>(this T mock, int expectedCalls)
        {
            var realMock = mock as IMock;
            if (realMock == null)
                throw new InvalidOperationException("mock is no mock");

            realMock.Received(expectedCalls);

            return mock;
        }

        public static void Returns<T>(this T returnType, T value)
        {
            Call lastCall = Mock.recorder.GetLastCall();

            if (!CheckReturnType(lastCall.ReturnType, typeof(T)))
            {
                throw new InvalidOperationException();
            }

            lastCall.MockedObject.Returns(lastCall.CalledMember, value);
        }


        // Change Name!!!
        private static bool CheckReturnType(Type expectedReturnType, Type actualType)
        {
            return expectedReturnType == actualType;
        }

    }
}
