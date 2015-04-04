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
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core
{
    public sealed class MockAssemblyGenerator : IAssemblyGenerator
    {
        private readonly IProjectFinder _projectFinder;

        private readonly IInterfaceExtractor _interfaceExtractor;

        private readonly IMockGenerator _mockGenerator;

        private readonly IMockRegistryGenerator _mockRegistryGenerator;

        public MockAssemblyGenerator(IProjectFinder projectFinder, IInterfaceExtractor interfaceExtractor, 
            IMockGenerator mockGenerator, IMockRegistryGenerator mockRegistryGenerator)
        {
            if (projectFinder == null)
                throw new ArgumentNullException("projectFinder");
            if (interfaceExtractor == null)
                throw new ArgumentNullException("interfaceExtractor");
            if (mockGenerator == null)
                throw new ArgumentNullException("mockGenerator");
            if (mockRegistryGenerator == null)
                throw new ArgumentNullException("mockRegistryGenerator");

            _projectFinder = projectFinder;
            _interfaceExtractor = interfaceExtractor;
            _mockGenerator = mockGenerator;
            _mockRegistryGenerator = mockRegistryGenerator;
        }

        public void GenerateMockAssembly(IEnumerable<string> assemblyNames, GenerationOptions options)
        {
            var mocks = assemblyNames.AsParallel()
                .SelectMany(x => _projectFinder.GetProjects(options.SolutionRoot, x))
                .SelectMany(x => _interfaceExtractor.ExtractAsync(x).Result)
                .Select(_mockGenerator.GenerateMock).ToList();

            var registry = _mockRegistryGenerator.GenerateRegistry(mocks);

            GenerateAssembly(options.WorkingDirectory, mocks, registry);
        }

        private void GenerateAssembly(string outputPath, IEnumerable<SyntaxTree> mocks, SyntaxTree registry)
        {
            List<SyntaxTree> trees = new List<SyntaxTree>(mocks) { registry };

            CSharpCompilationOptions options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation = CSharpCompilation.Create("RosMockLyn.Mocks", trees, null, options);

            var emitResult = compilation.Emit(outputPath);
        }
    }
}