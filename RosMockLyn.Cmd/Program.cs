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
