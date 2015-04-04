using Autofac;

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
        }

        private static IContainer BuildContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule<ModuleRegistry>();
            return builder.Build();
        }
    }
}
