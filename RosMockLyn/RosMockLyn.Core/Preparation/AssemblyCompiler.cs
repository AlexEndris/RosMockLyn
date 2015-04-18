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
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Host.Mef;

using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core.Preparation
{
    internal sealed class AssemblyCompiler : IAssemblyCompiler
    {
        public bool Compile(Project mainProject, IEnumerable<Project> referencedProjects, IEnumerable<SyntaxTree> trees, string outputFilePath)
        {
            var assemblyName = Path.GetFileNameWithoutExtension(outputFilePath);

            var project = CreateSolution(mainProject, referencedProjects, assemblyName);

            trees.Select(x => x.GetRoot())
                .Apply(x => project = project.AddDocument(GetClassNameFromTree(x), x).Project);

            return Compile(project, outputFilePath);
        }

        private static bool Compile(Project project, string outputFilePath)
        {
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation = project.GetCompilationAsync().Result.WithOptions(options);

            var emitResult = compilation.Emit(outputFilePath);

            return emitResult.Success;
        }

        private Project CreateSolution(Project mainProject, IEnumerable<Project> referencedProjects, string assemblyName)
        {
            var workspace = new AdhocWorkspace(MefHostServices.DefaultHost);

            ProjectId id = ProjectId.CreateNewId(assemblyName);

            SolutionInfo solutionInfo = SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Default);

            ProjectInfo projectInfo = ProjectInfo.Create(id, VersionStamp.Default, assemblyName, assemblyName, "C#");

            var solution = workspace.AddSolution(solutionInfo)
                .AddProject(projectInfo)
                .AddMetadataReferences(id, mainProject.MetadataReferences);

            referencedProjects.Apply(x => solution = solution.AddMetadataReference(id, GetReferenceFromProject(x)));

            return solution.GetProject(id);
        }

        private MetadataReference GetReferenceFromProject(Project project)
        {
            return MetadataReference.CreateFromFile(project.OutputFilePath);
        }

        private string GetClassNameFromTree(SyntaxNode rootNode)
        {
            return rootNode.DescendantNodes().OfType<ClassDeclarationSyntax>().Single().Identifier.ToString();
        }
    }
}