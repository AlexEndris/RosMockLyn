// Copyright (c) 2015, Alexander Endris
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// * Neither the name of RosMockLyn nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Windows.ApplicationModel;

namespace RosMockLyn.Mocking.IoC
{
    public sealed class MockInjector : IInjector
    {
        private readonly Dictionary<Type, Type> _typeMapper = new Dictionary<Type, Type>();

        public void RegisterType<TInterface, TConcrete>() where TInterface : class
                                                          where TConcrete : TInterface, new()
        {
            Type baseType = typeof(TInterface);
            Type mappedType = typeof(TConcrete);

            if (_typeMapper.ContainsKey(baseType))
                throw new InvalidOperationException("You can't map two different types to the same base type.");

            _typeMapper[baseType] = mappedType;
        }

        public T Resolve<T>() where T : class
        {
            Type mappedType;

            if (!_typeMapper.TryGetValue(typeof(T), out mappedType))
                return null;

            return InstantiateType<T>(mappedType);
        }

        public async Task ScanAssemblies()
        {
            var folder = Package.Current.InstalledLocation;

            foreach (var file in await folder.GetFilesAsync())
            {
                if (file.FileType != ".dll" && file.FileType != ".exe")
                    continue;

                try
                {
                    var assemblyName = new AssemblyName(file.DisplayName);
                    var assembly = Assembly.Load(assemblyName);

                    RegisterAssembly(assembly);
                }
                catch (Exception)
                {
                    // No need to do something as the assembly loaded
                    // could just be a native one (like Sqlite)
                }
            }
        }

        private void RegisterAssembly(Assembly assembly)
        {
            var registries = GetRegistriesFromAssembly(assembly);

            registries.Apply(x => x.Register(this));
        }

        private static T InstantiateType<T>(Type mappedType) where T : class
        {
            ConstructorInfo constructor = mappedType.GetTypeInfo()
                                                    .DeclaredConstructors
                                                    .SingleOrDefault(x => !x.GetParameters().Any());

            return constructor != null 
                    ? (T)constructor.Invoke(new object[] {})
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
