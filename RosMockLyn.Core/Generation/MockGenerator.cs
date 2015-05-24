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
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core.Generation
{
    internal sealed class MockGenerator : CSharpSyntaxRewriter, IMockGenerator
    {
        private readonly IEnumerable<ICodeTransformer> _transformers;

        public MockGenerator(IEnumerable<ICodeTransformer> transformers) : base(false)
        {
            _transformers = transformers;
        }

        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            var syntaxNode = (CompilationUnitSyntax)GetTransformer(TransformerType.Using).Transform(node);

            return base.VisitCompilationUnit(syntaxNode);
        }

        public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            var syntaxNode = GetTransformer(TransformerType.Namespace).Transform(node);

            return base.VisitNamespaceDeclaration((NamespaceDeclarationSyntax)syntaxNode);
        }

        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            var syntaxNode = GetTransformer(TransformerType.Interface).Transform(node);

            return VisitClassDeclaration((ClassDeclarationSyntax)syntaxNode);
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var newMethodSyntax = GetTransformer(TransformerType.Method).Transform(node);

            return base.VisitMethodDeclaration((MethodDeclarationSyntax)newMethodSyntax);
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            var syntaxNode = GetTransformer(TransformerType.Property).Transform(node);

            return base.VisitPropertyDeclaration((PropertyDeclarationSyntax)syntaxNode);
        }

        public override SyntaxNode VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            var syntaxNode = GetTransformer(TransformerType.Indexer).Transform(node);

            return base.VisitIndexerDeclaration((IndexerDeclarationSyntax)syntaxNode);
        }

        public SyntaxTree GenerateMock(SyntaxTree treeToGenerateMockFrom)
        {
            return SyntaxFactory.SyntaxTree(Visit(treeToGenerateMockFrom.GetRoot()).NormalizeWhitespace());
        }

        private ICodeTransformer GetTransformer(TransformerType type)
        {
            return _transformers.Single(x => x.Type == type);
        }
    }
}