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
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RosMockLyn.Core.Helpers
{
    public static class NameHelper
    {
        public static IdentifierNameSyntax GetBaseInterfaceIdentifier(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            var typeDeclarationSyntax =
                (TypeDeclarationSyntax)node.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().Single();
                                                        
            var identifierNameSyntax = (IdentifierNameSyntax)typeDeclarationSyntax.BaseList.Types[1].Type;

            return identifierNameSyntax;
        }

        public static string GetMockImplementationName(SyntaxNode node, string suffix = "Mock")
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (string.IsNullOrWhiteSpace(suffix))
                throw new ArgumentNullException("suffix");

            var typeDeclarationSyntax = (TypeDeclarationSyntax)node.DescendantNodesAndSelf()
                                        .OfType<InterfaceDeclarationSyntax>()
                                        .Single();

            var interfaceName = typeDeclarationSyntax.Identifier.ToString();

            return interfaceName.Substring(1) + suffix;
        }

        public static string GetInterfaceName(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            var typeDeclarationSyntax = (TypeDeclarationSyntax)node.DescendantNodesAndSelf()
                                        .OfType<InterfaceDeclarationSyntax>()
                                        .Single();

            return typeDeclarationSyntax.Identifier.ToString();
        }

        public static string GetFullyQualifiedNamespace(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            
            var namespaceDeclaration = node.DescendantNodesAndSelf()
                                        .OfType<NamespaceDeclarationSyntax>()
                                        .Single();

            return namespaceDeclaration.Name.ToString();
        }
    }
}