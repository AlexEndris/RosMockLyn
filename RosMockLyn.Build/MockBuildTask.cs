// Copyright (c) 2015, Alexander Endris
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
// * Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
// * Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
// OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
// NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

using Autofac;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using RosMockLyn.Core;
using RosMockLyn.Core.Interfaces;
using RosMockLyn.Core.IoC;

namespace RosMockLyn.Build
{
    public class MockBuildTask : Task
    {
        [Required]
        public string SolutionRoot { get; set; }

        [Required]
        public string GeneratedAssemblyPath { get; set; }

        [Required]
        public string TestProjectPath { get; set; }

        [Required]
        public string TestAssemblyPath { get; set; }

        [Output]
        public string GeneratedAssemblyFullPath { get; set; }

        [Output]
        public string GeneratedFile { get; set; }

        static MockBuildTask()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => DependencyResolver.GetAssembly(args.Name);
        }
        
        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            var container = BuildContainer();
            var assemblyGenerator = container.Resolve<IAssemblyGenerator>();
            var assemblyManipulator = container.Resolve<IAssemblyManipulator>();

            var generationOptions = CreateGenerationOptions();

            if (!assemblyGenerator.GenerateMockAssembly(generationOptions))
                return false;

            if (!assemblyManipulator.AddReferenceToAssembly(TestAssemblyPath, GeneratedAssemblyPath))
                return false;

            GenerateOutput();

            return true;
        }

        private static IContainer BuildContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule<ModuleRegistry>();

            return builder.Build();
        }

        private GenerationOptions CreateGenerationOptions()
        {
            return new GenerationOptions {
                                               ProjectPath = TestProjectPath,
                                               OutputFilePath = GeneratedAssemblyPath,
                                               SolutionRoot = SolutionRoot
                                         };
        }

        private void GenerateOutput()
        {
            GeneratedFile = Path.GetFileName(GeneratedAssemblyPath);
            GeneratedAssemblyFullPath = Path.GetFullPath(GeneratedAssemblyPath);
        }
    }
}