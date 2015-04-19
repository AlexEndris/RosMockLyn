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

            var assemblyManipulator = buildContainer.Resolve<IAssemblyManipulator>();

            GenerateAssembly(buildContainer);
            AddReference(assemblyManipulator);
        }

        private static void AddReference(IAssemblyManipulator assemblyManipulator)
        {
            assemblyManipulator.AddReferenceToAssembly(
                @"E:\important\eigene dateien\visual studio 2013\Projects\RosMockLyn\GeneratedTestingAssembly.Tests\bin\Debug\GeneratedTestingAssembly.Tests.dll",
                @"E:\important\eigene dateien\visual studio 2013\Projects\RosMockLyn\GeneratedTestingAssembly.Tests\bin\Debug\MockAssembly.dll");
        }

        private static void GenerateAssembly(IContainer buildContainer)
        {
            var assemblyGenerator = buildContainer.Resolve<IAssemblyGenerator>();

            GenerationOptions options = new GenerationOptions();

            options.ProjectPath =
                @"E:\important\eigene dateien\visual studio 2013\Projects\RosMockLyn\GeneratedTestingAssembly.Tests\GeneratedTestingAssembly.Tests.csproj";
            options.SolutionRoot = @"E:\important\eigene dateien\visual studio 2013\Projects\RosMockLyn\";

            options.OutputFilePath =
                @"E:\important\eigene dateien\visual studio 2013\Projects\RosMockLyn\GeneratedTestingAssembly.Tests\bin\Debug\MockAssembly.dll";

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
