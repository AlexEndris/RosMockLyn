using System.Threading.Tasks;

namespace RosMockLyn.Mocking.IoC
{
    public interface IInjector
    {
        /// <summary>
        /// Registers an interface or base type to a concrete.
        /// </summary>
        /// <typeparam name="TInterface">Interface or base type.</typeparam>
        /// <typeparam name="TConcrete">Concrete type.</typeparam>
        void RegisterType<TInterface, TConcrete>() where TInterface : class 
            where TConcrete : TInterface, new();

        /// <summary>
        /// Resolves the given type if registered.
        /// </summary>
        /// <typeparam name="T">Type to resolve an instance of.</typeparam>
        /// <returns>Returns a requested instance if registered, otherwise <c>null</c></returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Scans all available assemblies for all<see cref="IInjectorRegistry"/>.
        /// </summary>
        /// <returns>Task to wait for.</returns>
        Task ScanAssemblies();
    }
}