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
namespace RosMockLyn.Core
{
    using System;
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Formatting;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.MSBuild;

    public class TestingRoslyn
    {
        public void DoSomething()
        {
            typeof(CSharpFormattingOptions).ToString();

            var workspace = MSBuildWorkspace.Create();

            var project =
                workspace.OpenProjectAsync(@"E:\important\eigene dateien\visual studio 2013\Projects\RosMockLyn\TestProject\TestProject.csproj")
                    .Result;

            var compilation = project.GetCompilationAsync().Result;

            var syntaxTrees = compilation.SyntaxTrees.Where(HasInterface);

            IInterfaceMockGenerator mockingWalker = new InterfaceMockGenerator();
            var outputWalker = new OutputWalker();

            var syntax = mockingWalker.GenerateMock(syntaxTrees.First());

            outputWalker.Visit(syntaxTrees.First().GetRoot());
            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine();
            outputWalker.Visit(syntax.GetRoot());

            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine(syntax.ToString());
        }

        private bool HasInterface(SyntaxTree tree)
        {
            var root = tree.GetRoot();

            var interfaceBlockSyntaxs = from node in root.DescendantNodes().OfType<InterfaceDeclarationSyntax>() select node;


            return interfaceBlockSyntaxs.Any();
        }
    }
}