using System.Reflection;
using System.Threading.Tasks;

namespace RosMockLyn.Utilities.IoC
{
    public interface IInjector
    {
        void RegisterAssembly(Assembly assembly);

        void RegisterType<TInterface, TConcrete>() where TInterface: class 
            where TConcrete : TInterface, new();

        T Resolve<T>() where T : class;

        Task ScanAssemblies();
    }
}