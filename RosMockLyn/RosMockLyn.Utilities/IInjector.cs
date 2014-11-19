namespace RosMockLyn.Utilities
{
    public interface IInjector
    {
        void RegisterType<TInterface, TConcrete>() where TInterface: class 
            where TConcrete : TInterface, new();

        T Resolve<T>() where T : class;
    }
}