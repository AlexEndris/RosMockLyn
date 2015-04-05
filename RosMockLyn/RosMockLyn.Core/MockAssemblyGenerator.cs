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

using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core
{
    public sealed class MockAssemblyGenerator : IAssemblyGenerator
    {
        private readonly IProjectRetriever _projectRetriever;

        private readonly IInterfaceExtractor _interfaceExtractor;

        private readonly IReferenceResolver _referenceResolver;

        private readonly IMockGenerator _mockGenerator;

        private readonly IMockRegistryGenerator _mockRegistryGenerator;

        private readonly IAssemblyCompiler _compiler;

        public MockAssemblyGenerator(IProjectRetriever projectRetriever, 
            IInterfaceExtractor interfaceExtractor,
            IReferenceResolver referenceResolver,
            IMockGenerator mockGenerator, 
            IMockRegistryGenerator mockRegistryGenerator,
            IAssemblyCompiler compiler)
        {
            if (projectRetriever == null)
                throw new ArgumentNullException("projectRetriever");
            if (interfaceExtractor == null)
                throw new ArgumentNullException("interfaceExtractor");
            if (referenceResolver == null)
                throw new ArgumentNullException("referenceResolver");
            if (mockGenerator == null)
                throw new ArgumentNullException("mockGenerator");
            if (mockRegistryGenerator == null)
                throw new ArgumentNullException("mockRegistryGenerator");
            if (compiler == null)
                throw new ArgumentNullException("compiler");

            _projectRetriever = projectRetriever;
            _interfaceExtractor = interfaceExtractor;
            _referenceResolver = referenceResolver;
            _mockGenerator = mockGenerator;
            _mockRegistryGenerator = mockRegistryGenerator;
            _compiler = compiler;
        }

        public void GenerateMockAssembly(GenerationOptions options)
        {
            var mainProject = _projectRetriever.OpenProject(options.ProjectPath);
            
            var referencedProjects = _projectRetriever.GetReferencedProjects(mainProject);

            var trees = referencedProjects.SelectMany(_interfaceExtractor.Extract).ToList();

            var mocks = trees.Select(_mockGenerator.GenerateMock);

            var registry = _mockRegistryGenerator.GenerateRegistry(trees);

            List<SyntaxTree> finalTrees = new List<SyntaxTree>(mocks) { registry };
            
            _compiler.Compile(mainProject, referencedProjects, finalTrees, options.OutputFilePath);
        }
    }
}