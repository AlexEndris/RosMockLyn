using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using RosMockLyn.Utilities.IoC;

namespace RosMockLyn.Utilities.Tests
{
    [TestClass]
    public class MockInjectorTests
    {
        private MockInjector _injector;

        [TestInitialize]
        public void Initialize()
        {
            _injector = new MockInjector();
        }

        [TestMethod]
        public void RegisterSameBaseTypeMultipleTimes_ShouldThrowException()
        {
            // Arrange
            _injector.RegisterType<ISomeInterface, SomeInterfaceImpl>();
            
            // Act
            // Assert
            Assert.ThrowsException<InvalidOperationException>(() => _injector.RegisterType<ISomeInterface, SomeInterfaceOtherImpl>());
        }

        [TestMethod]
        public void ResolvingRegisteredType_ShouldReturnInstance()
        {
            // Arrange
            _injector.RegisterType<ISomeInterface, SomeInterfaceImpl>();

            // Act
            var instance = _injector.Resolve<ISomeInterface>();
            
            // Assert
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void ResolvingNotRegisteredType_ShouldReturnNull()
        {
            // Arrange
            // Act
            var instance = _injector.Resolve<ISomeInterface>();

            // Assert
            Assert.IsNull(instance);
        }

        private interface ISomeInterface
        {
            
        }

        private class SomeInterfaceImpl : ISomeInterface
        {
            
        }

        private class SomeInterfaceOtherImpl : ISomeInterface
        {

        }
    }
}
