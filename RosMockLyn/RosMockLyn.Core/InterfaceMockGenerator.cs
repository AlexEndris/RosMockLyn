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
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class InterfaceMockGenerator : CSharpSyntaxRewriter, IInterfaceMockGenerator
    {
        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            // Class base class visit to traverse down the tree
            // I use "VisitClassDeclaration" instead of "Interface..." because
            // we want to replace the interface with a class!
            // "NormalizeWhitespace" adds needed trivia to correctly format the resulting code (can also be done manually)
           return base.VisitClassDeclaration(ImplementInterface(node).NormalizeWhitespace());
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            // Replace the original property and just change the name of the identifier.
            var newPropertyToken = node.ReplaceToken(
                node.Identifier,
                SyntaxFactory.Identifier(node.Identifier.ValueText+"Impl"));

            // Again, call base method to be able to traverse deeper into the tree, if needed/wanted
            return base.VisitPropertyDeclaration(newPropertyToken);
        }

        private ClassDeclarationSyntax ImplementInterface(InterfaceDeclarationSyntax interfaceDeclaration)
        {
            return SyntaxFactory.ClassDeclaration(
                        SyntaxFactory.Identifier(interfaceDeclaration.Identifier.ValueText.Substring(1)))
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddBaseListTypes(
                        SyntaxFactory.IdentifierName(
                            SyntaxFactory.Identifier(interfaceDeclaration.Identifier.ValueText)))
                    .AddMembers(interfaceDeclaration.Members.ToArray());
        }

        public SyntaxTree GenerateMock(SyntaxTree treeToGenerateMockFrom)
        {
            return SyntaxFactory.SyntaxTree(Visit(treeToGenerateMockFrom.GetRoot()).NormalizeWhitespace());
        }
    }
}