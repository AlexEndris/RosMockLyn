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
        public void Compile(Project mainProject, IEnumerable<Project> referencedProjects, IEnumerable<SyntaxTree> trees, string outputFilePath)
        {
            var project = CreateSolution(mainProject, referencedProjects);

            trees.Select(x => x.GetRoot())
                .Apply(x => project = project.AddDocument(GetClassNameFromTree(x), x).Project);

            Compile(mainProject, project, outputFilePath);
        }

        private static void Compile(Project mainProject, Project project, string outputFilePath)
        {
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation = project.GetCompilationAsync().Result.WithOptions(options);

            var emitResult = compilation.Emit(outputFilePath);
        }

        private Project CreateSolution(Project mainProject, IEnumerable<Project> referencedProjects)
        {
            var workspace = new AdhocWorkspace(MefHostServices.DefaultHost);

            ProjectId id = ProjectId.CreateNewId("MockAssembly");

            SolutionInfo solutionInfo = SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Default);

            ProjectInfo projectInfo = ProjectInfo.Create(id, VersionStamp.Default, "MockAssembly", "MockAssembly", "C#");

            var solution = workspace.AddSolution(solutionInfo)
                .AddProject(projectInfo)
                .AddMetadataReferences(id, mainProject.MetadataReferences);

            var outputDirectory = Path.GetDirectoryName(mainProject.OutputFilePath);
            referencedProjects.Apply(x => solution = solution.AddMetadataReference(id, GetReferenceFromProject(outputDirectory, x)));

            return solution.GetProject(id);
        }

        private MetadataReference GetReferenceFromProject(string outputDirectory, Project project)
        {
            var fileName = Path.GetFileName(project.OutputFilePath);

            return MetadataReference.CreateFromFile(Path.Combine(outputDirectory, fileName));
        }

        private string GetClassNameFromTree(SyntaxNode rootNode)
        {
            return rootNode.DescendantNodes().OfType<ClassDeclarationSyntax>().Single().Identifier.ToString();
        }
    }
}