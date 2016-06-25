using Autofac;

using NUnit.Framework;

using RosMockLyn.Core.Interfaces;
using RosMockLyn.Core.IoC;

namespace RosMockLyn.Core.Tests.Integration
{
    [TestFixture]
    public class IntegrationTests
    {
        private IMockFileGenerator _mockFileGenerator;

        [SetUp]
        public void SetUp()
        {
            var container = BuildContainer();

            _mockFileGenerator = container.Resolve<IMockFileGenerator>();
        }

        private static IContainer BuildContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule<ModuleRegistry>();

            return builder.Build();
        }

        [Test]
        public void Test()
        {
            var generateMockFile = _mockFileGenerator.GenerateMockFile(
                @"D:\Development\Programming\Git Repos\RosMockLyn\GeneratedTestingAssembly.Tests\GeneratedTestingAssembly.Tests.csproj");
        }

    }
}