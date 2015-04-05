﻿// Copyright (c) 2015, Alexander Endris
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
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core.Preparation
{
    internal sealed class InterfaceExtractor : IInterfaceExtractor
    {
        public IEnumerable<SyntaxTree> Extract(Project project)
        {
            var compilation = project.GetCompilationAsync().Result;

            return compilation.SyntaxTrees.Where(HasInterface).Where(NotRosMockLyn);
        }

        private bool NotRosMockLyn(SyntaxTree tree)
        {
            var root = tree.GetRoot();

            var interfaceBlockSyntaxs = from node in root.DescendantNodes().OfType<NamespaceDeclarationSyntax>()
                                        where node.Name.ToString().Contains("RosMockLyn.Mocking")
                                        select node;

            return !interfaceBlockSyntaxs.Any();
        }

        private bool HasInterface(SyntaxTree tree)
        {
            var root = tree.GetRoot();

            var interfaceBlockSyntaxs = from node in root.DescendantNodes().OfType<InterfaceDeclarationSyntax>() select node;

            return interfaceBlockSyntaxs.Any();
        }
    }
}