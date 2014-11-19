using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Windows.ApplicationModel;

namespace RosMockLyn.Utilities.IoC
{
    public sealed class MockInjector : IInjector
    {
        private readonly Dictionary<Type, Type> _typeMapper = new Dictionary<Type, Type>();

        public void RegisterAssembly(Assembly assembly)
        {
            var registries = GetRegistriesFromAssembly(assembly);

            registries.Apply(x => x.Register(this));
        }

        public void RegisterType<TInterface, TConcrete>() where TInterface : class
                                                          where TConcrete : TInterface, new()
        {
            Type baseType = typeof (TInterface);
            Type mappedType = typeof (TConcrete);

            if (this._typeMapper.ContainsKey(baseType))
                throw new InvalidOperationException("You can't map two different types to the same base type.");

            this._typeMapper[baseType] = mappedType;
        }

        public T Resolve<T>() where T : class
        {
            Type mappedType;

            if (!this._typeMapper.TryGetValue(typeof (T), out mappedType))
                return null;

            return InstantiateType<T>(mappedType);
        }

        public async Task ScanAssemblies()
        {
            var folder = Package.Current.InstalledLocation;

            foreach (var file in await folder.GetFilesAsync())
            {
                if (file.FileType == ".dll" || file.FileType == ".exe")
                {
                    try
                    {
                        var assemblyName = new AssemblyName(file.DisplayName);
                        var assembly = Assembly.Load(assemblyName);

                        RegisterAssembly(assembly);
                    }
                    catch (Exception)
                    {
                        // TODO: Do something here!
                    }
                }
            }
        }

        private static T InstantiateType<T>(Type mappedType) where T : class
        {
            ConstructorInfo constructor = mappedType.GetTypeInfo()
                                                    .DeclaredConstructors
                                                    .SingleOrDefault(x => !x.GetParameters().Any());

            return constructor != null 
                    ? (T) constructor.Invoke(new object[] {})
                    : null;
        }

        private static IEnumerable<IInjectorRegistry> GetRegistriesFromAssembly(Assembly assembly)
        {
            return assembly.ExportedTypes
                .Where(x => x.IsAssignableTo<IInjectorRegistry>())
                .Select(Activator.CreateInstance)
                .OfType<IInjectorRegistry>();
        }

    }
}
