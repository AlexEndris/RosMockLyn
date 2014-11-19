using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RosMockLyn.Utilities
{
    public class MockInjector : IInjector
    {
        private readonly Dictionary<Type, Type> _typeMapper = new Dictionary<Type, Type>(); 

        public void RegisterType<TInterface, TConcrete>() where TInterface : class
                                                          where TConcrete : TInterface, new()
        {
            Type baseType = typeof (TInterface);
            Type mappedType = typeof (TConcrete);

            if (_typeMapper.ContainsKey(baseType))
                throw new Exception(); // TODO: Use more fitting exception type!

            _typeMapper[baseType] = mappedType;
        }

        public T Resolve<T>() where T : class
        {
            Type mappedType;

            if (!_typeMapper.TryGetValue(typeof (T), out mappedType))
            {
                return null;
            }

            return InstantiateType<T>(mappedType);
        }

        private static T InstantiateType<T>(Type mappedType) where T : class
        {
            ConstructorInfo constructor =
                mappedType.GetTypeInfo().DeclaredConstructors.SingleOrDefault(x => !x.GetParameters().Any());

            return constructor != null 
                    ? (T) constructor.Invoke(new object[] {})
                    : null;
        }
    }
}
