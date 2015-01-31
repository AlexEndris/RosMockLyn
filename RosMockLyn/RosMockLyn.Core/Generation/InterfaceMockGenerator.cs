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

namespace RosMockLyn.Core
{
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    // TODO : Change name!
    public class InterfaceMockGenerator : CSharpSyntaxRewriter, IInterfaceMockGenerator
    {
        private NameSyntax _interfaceIdentifier;

        private const string AdditionalUsing = "RosMockLyn.Mocking";
        private const string DerivesFrom = "MockBase";
        private const string ClassSuffix = "Mock";
        private const string Namespace = ".RosMockLyn";

        private const string SubstitutionContext = "SubstitutionContext";

        private const string Method = "Method";
        private const string Property = "Property";
        private const string Index = "Index";

        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            var usings = GenerateAdditionalUsings();

            node = node.AddUsings(usings);

            return base.VisitCompilationUnit(node);
        }

        private UsingDirectiveSyntax[] GenerateAdditionalUsings()
        {
            var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(AdditionalUsing));

            return new[] { usingDirective };
        }

        public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            node = node.WithName(SyntaxFactory.IdentifierName(GenerateNamespaceIdentifier(node)));

            return base.VisitNamespaceDeclaration(node);
        }

        private static string GenerateNamespaceIdentifier(NamespaceDeclarationSyntax node)
        {
            return ((IdentifierNameSyntax)node.Name).Identifier.ValueText+Namespace;
        }

        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            // Class base class visit to traverse down the tree
            // I use "VisitClassDeclaration" instead of "Interface..." because
            // we want to replace the interface with a class!
            // "NormalizeWhitespace" adds needed trivia to correctly format the resulting code (can also be done manually)
           return base.VisitClassDeclaration(MockInterface(node).NormalizeWhitespace());
        }

        public override SyntaxNode VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            var basePropertyDeclaration = node.FirstAncestorOrSelf<BasePropertyDeclarationSyntax>();

            if (basePropertyDeclaration is IndexerDeclarationSyntax)
            {
                // TODO indexer
                //node = node.ReplaceNode(node, GeneratePropertyAccessor(node.CSharpKind(), basePropertyDeclaration.Type));
            }
            else if (basePropertyDeclaration is PropertyDeclarationSyntax)
            {
                node = GeneratePropertyAccessor(node.CSharpKind(), basePropertyDeclaration.Type);
            }

            return base.VisitAccessorDeclaration(node);
        }

        private AccessorDeclarationSyntax GeneratePropertyAccessor(SyntaxKind syntaxKind, TypeSyntax typeSyntax)
        {
            string substitutionCall = string.Format(
                "{0}.{1}{2}",
                SubstitutionContext,
                GetAccessor(syntaxKind),
                Property);
          
            var typeList = SyntaxFactory.SeparatedList(new[] { typeSyntax });
            
            var substitution = SyntaxFactory.InvocationExpression(SyntaxFactory.GenericName(substitutionCall)
                                            .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(typeList)));

            if (syntaxKind == SyntaxKind.SetAccessorDeclaration)
            {
                var argument = SyntaxFactory.Argument(SyntaxFactory.IdentifierName("value"));
                substitution = substitution.WithArgumentList(
                        SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { argument })));
            }

            return GenerateAccessor(syntaxKind, substitution);
        }

        private string GetAccessor(SyntaxKind syntaxKind)
        {
            if (syntaxKind == SyntaxKind.GetAccessorDeclaration)
                return "Get";

            return "Set";
        }

        private AccessorDeclarationSyntax GenerateAccessor(SyntaxKind accessor, InvocationExpressionSyntax substitutionCall)
        {
            var accessorDeclarationSyntax = SyntaxFactory.AccessorDeclaration(
                accessor,
                SyntaxFactory.Block(
                    SyntaxFactory.ReturnStatement(substitutionCall)));

            return accessorDeclarationSyntax;
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            // Replace the original property and just change the name of the identifier.
            var newPropertyToken = node.WithExplicitInterfaceSpecifier(SyntaxFactory.ExplicitInterfaceSpecifier(_interfaceIdentifier))
                                       .WithIdentifier(SyntaxFactory.Identifier(node.Identifier.ValueText));

            // Again, call base method to be able to traverse deeper into the tree, if needed/wanted
            return base.VisitPropertyDeclaration(newPropertyToken);
        }

        private ClassDeclarationSyntax MockInterface(InterfaceDeclarationSyntax interfaceDeclaration)
        {
            _interfaceIdentifier = SyntaxFactory.IdentifierName(interfaceDeclaration.Identifier.ValueText);
            
            return SyntaxFactory.ClassDeclaration(
                SyntaxFactory.Identifier(interfaceDeclaration.Identifier.ValueText.Substring(1) + ClassSuffix))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddBaseListTypes(
                    SyntaxFactory.IdentifierName(DerivesFrom), // MockBase, base type
                    _interfaceIdentifier // Interface that is being implemented
                    )
                .AddMembers(interfaceDeclaration.Members.ToArray());
        }

        public SyntaxTree GenerateMock(SyntaxTree treeToGenerateMockFrom)
        {
            return SyntaxFactory.SyntaxTree(Visit(treeToGenerateMockFrom.GetRoot()).NormalizeWhitespace());
        }
    }
}