using System;

using NUnit.Framework;

using RosMockLyn.Mocking.IoC;

using Assert = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert;

namespace RosMockLyn.Mocking.Tests
{
    [TestFixture]
    public class MockInjectorTests
    {
        private MockInjector _injector;

        [SetUp]
        public void Initialize()
        {
            _injector = new MockInjector();
        }

        [Test, Category("Unit Test")]
        public void RegisterSameBaseTypeMultipleTimes_ShouldThrowException()
        {
            // Arrange
            _injector.RegisterType<ISomeInterface, SomeInterfaceImpl>();
            
            // Act
            // Assert
            Assert.ThrowsException<InvalidOperationException>(() => _injector.RegisterType<ISomeInterface, SomeInterfaceOtherImpl>());
        }

        [Test, Category("Unit Test")]
        public void ResolvingRegisteredType_ShouldReturnInstance()
        {
            // Arrange
            _injector.RegisterType<ISomeInterface, SomeInterfaceImpl>();

            // Act
            var instance = _injector.Resolve<ISomeInterface>();
            
            // Assert
            Assert.IsNotNull(instance);
        }

        [Test, Category("Unit Test")]
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
