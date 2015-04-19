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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core.Transformation
{
    internal sealed class NamespaceTransformer : ICodeTransformer
    {
        private const string Namespace = "RosMockLyn";

        public TransformerType Type
        {
            get
            {
                return TransformerType.Namespace;
            }
        }

        public SyntaxNode Transform(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            var namespaceDeclaration = node as NamespaceDeclarationSyntax;

            if (namespaceDeclaration == null)
                throw new InvalidOperationException("Provided node must be a NamespaceDeclaration.");

            var namespaceName = SyntaxFactory.QualifiedName(namespaceDeclaration.Name, SyntaxFactory.IdentifierName(Namespace));

            return namespaceDeclaration.WithName(namespaceName);
        }
    }
}