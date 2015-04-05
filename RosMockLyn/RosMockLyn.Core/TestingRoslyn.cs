// Copyright (c) 2014, Alexander Endris
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
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

using RosMockLyn.Core.Generation;
using RosMockLyn.Core.Interfaces;
using RosMockLyn.Core.Transformation;

namespace RosMockLyn.Core
{
    public class TestingRoslyn
    {
        public void DoSomething()
        {
            var workspace = MSBuildWorkspace.Create();

            var project =
                workspace.OpenProjectAsync(
                    @"E:\important\eigene dateien\visual studio 2013\Projects\RosMockLyn\TestProjectPort\TestProjectPort.csproj")
                    .Result;

            var compilation = project.GetCompilationAsync().Result;

            var syntaxTrees = compilation.SyntaxTrees.Where(HasInterface);

            MockGenerator mockingWalker = new MockGenerator(GenerateTransformers());
            var outputWalker = new OutputWalker();

            var syntax = syntaxTrees.Select(x => mockingWalker.GenerateMock(x)).ToList();

            outputWalker.Visit(syntaxTrees.First().GetRoot());
            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine();
            outputWalker.Visit(syntax.First().GetRoot());

            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();

            PrintSyntaxTrees(syntax);

            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();
            
            List<MetadataReference> refs = new List<MetadataReference>();

            var testProject = MetadataReference.CreateFromFile(@"E:\Important\Eigene Dateien\Visual Studio 2013\Projects\RosMockLyn\TestProjectPort\bin\Debug\TestProjectPort.dll");
            var mocking = MetadataReference.CreateFromFile(@"E:\Important\Eigene Dateien\Visual Studio 2013\Projects\RosMockLyn\RosMockLyn.Mocking\bin\Debug\RosMockLyn.Mocking.dll");
            
            refs.Add(mocking);
            refs.Add(testProject);

            var strings = Directory.GetFiles(
                @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETPortable\v4.6\Profile\Profile32\")
                .Where(x => Path.GetExtension(x).EndsWith("dll", StringComparison.InvariantCultureIgnoreCase));

            refs.AddRange(strings.Select(x => MetadataReference.CreateFromFile(x)));

            var generator = new MockRegistryGenerator();

            var generatedRegistry = generator.GenerateRegistry(syntaxTrees);

            PrintSyntaxTree(generatedRegistry.GetRoot().NormalizeWhitespace().SyntaxTree);

            CSharpCompilationOptions options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            syntax.Add(generatedRegistry);

            var csCompilation = CSharpCompilation.Create("TestAssembly", syntax, refs, options);

            var emitResult = FileSystemExtensions.Emit(csCompilation, @"D:\TestAssembly.dll");
        }

        private static void PrintSyntaxTrees(List<SyntaxTree> syntax)
        {
            foreach (var tree in syntax)
            {
                PrintSyntaxTree(tree);
            }
        }

        private static void PrintSyntaxTree(SyntaxTree tree)
        {
            StringBuilder builder = new StringBuilder();

            var split = tree.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            int lineNr = 0;
            foreach (var line in split)
            {
                builder.AppendFormat("{0}: ", ++lineNr);
                builder.AppendLine(line);
            }

            Console.WriteLine(builder.ToString());
        }

        private IEnumerable<ICodeTransformer> GenerateTransformers()
        {
            yield return new UsingsTransformer();
            yield return new NamespaceTransformer();
            yield return new MethodTransformer();
            yield return new InterfaceTransformer();
            yield return new PropertyTransformer();
            yield return new IndexerTransformer();
        }

        private bool HasInterface(SyntaxTree tree)
        {
            var root = tree.GetRoot();

            var interfaceBlockSyntaxs = from node in root.DescendantNodes().OfType<InterfaceDeclarationSyntax>() select node;

            return interfaceBlockSyntaxs.Any();
        }
    }
}