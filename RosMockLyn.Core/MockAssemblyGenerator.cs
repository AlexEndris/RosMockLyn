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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RosMockLyn.Core.Helpers;
using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core
{
    internal sealed class MockAssemblyGenerator : IAssemblyGenerator
    {
        private readonly IProjectRetriever _projectRetriever;

        private readonly IInterfaceExtractor _interfaceExtractor;

        private readonly IMockGenerator _mockGenerator;

        private readonly IMockRegistryGenerator _mockRegistryGenerator;

        private readonly IAssemblyCompiler _compiler;

        public MockAssemblyGenerator(IProjectRetriever projectRetriever, 
            IInterfaceExtractor interfaceExtractor,
            IMockGenerator mockGenerator, 
            IMockRegistryGenerator mockRegistryGenerator,
            IAssemblyCompiler compiler)
        {
            if (projectRetriever == null)
                throw new ArgumentNullException("projectRetriever");
            if (interfaceExtractor == null)
                throw new ArgumentNullException("interfaceExtractor");
            if (mockGenerator == null)
                throw new ArgumentNullException("mockGenerator");
            if (mockRegistryGenerator == null)
                throw new ArgumentNullException("mockRegistryGenerator");
            if (compiler == null)
                throw new ArgumentNullException("compiler");

            _projectRetriever = projectRetriever;
            _interfaceExtractor = interfaceExtractor;
            _mockGenerator = mockGenerator;
            _mockRegistryGenerator = mockRegistryGenerator;
            _compiler = compiler;
        }

        public bool GenerateMockAssembly(GenerationOptions options)
        {
            var mainProject = _projectRetriever.OpenProject(options.ProjectPath);

            var usedInterfaces = _interfaceExtractor.GetUsedInterfaceNames(mainProject);

            var referencedProjects = _projectRetriever.GetReferencedProjects(mainProject);

            var finalTrees = GenerateMocks(referencedProjects, usedInterfaces);

            return _compiler.Compile(mainProject, referencedProjects, finalTrees, options.OutputFilePath);
        }

        private List<SyntaxTree> GenerateMocks(IEnumerable<Project> referencedProjects, IEnumerable<string> usedInterfaces)
        {
            var trees = referencedProjects.SelectMany(_interfaceExtractor.Extract)
                                           .Where(x => IdentifierHelper.ContainsAnyInterface(x.GetRoot(), usedInterfaces));

            var mocks = trees.Select(_mockGenerator.GenerateMock);

            var registry = _mockRegistryGenerator.GenerateRegistry(trees);

            List<SyntaxTree> finalTrees = new List<SyntaxTree>(mocks) { registry };
            return finalTrees;
        }
    }
}
