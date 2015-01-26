using System;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using RosMockLyn.Mocking.IoC;

namespace RosMockLyn.Mocking.Tests
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

        [TestMethod, TestCategory("Unit Test")]
        public void RegisterSameBaseTypeMultipleTimes_ShouldThrowException()
        {
            // Arrange
            _injector.RegisterType<ISomeInterface, SomeInterfaceImpl>();
            
            // Act
            // Assert
            Assert.ThrowsException<InvalidOperationException>(() => _injector.RegisterType<ISomeInterface, SomeInterfaceOtherImpl>());
        }

        [TestMethod, TestCategory("Unit Test")]
        public void ResolvingRegisteredType_ShouldReturnInstance()
        {
            // Arrange
            _injector.RegisterType<ISomeInterface, SomeInterfaceImpl>();

            // Act
            var instance = _injector.Resolve<ISomeInterface>();
            
            // Assert
            Assert.IsNotNull(instance);
        }

        [TestMethod, TestCategory("Unit Test")]
        public void ResolvingNotRegisteredType_ShouldReturnNull()
        {
            // Arrange
            // Act
            var instance = this._injector.Resolve<ISomeInterface>();

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
