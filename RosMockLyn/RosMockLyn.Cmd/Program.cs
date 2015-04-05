using Autofac;

using RosMockLyn.Core;
using RosMockLyn.Core.Interfaces;
using RosMockLyn.Core.IoC;

namespace RosMockLyn.Cmd
{
    public class Program
    {
        public static void Main()
        {
            var buildContainer = BuildContainer();

            var assemblyGenerator = buildContainer.Resolve<IAssemblyGenerator>();

            GenerationOptions options = new GenerationOptions();

            options.ProjectPath =
                @"E:\important\eigene dateien\visual studio 2013\Projects\RosMockLyn\GeneratedTestingAssembly.Tests\GeneratedTestingAssembly.Tests.csproj";
            options.SolutionRoot = @"E:\important\eigene dateien\visual studio 2013\Projects\RosMockLyn\";

            assemblyGenerator.GenerateMockAssembly(options);
        }

        private static IContainer BuildContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule<ModuleRegistry>();
            return builder.Build();
        }
    }
}
