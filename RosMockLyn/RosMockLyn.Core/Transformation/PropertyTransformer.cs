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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RosMockLyn.Core.Helpers;
using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core.Transformation
{
    public class PropertyTransformer : CSharpSyntaxRewriter, ICodeTransformer
    {
        private const string SubstitutionContext = "SubstitutionContext";
        private const string Property = "Property";

        public TransformerType Type
        {
            get
            {
                return TransformerType.Property;
            }
        }

        public SyntaxNode Transform(SyntaxNode node)
        {
            var interfaceIdentifier = NameHelper.GetBaseInterfaceIdentifier(node);
            var propertyDeclaration = (PropertyDeclarationSyntax)node;

            var newPropertyToken = propertyDeclaration.WithExplicitInterfaceSpecifier(SyntaxFactory.ExplicitInterfaceSpecifier(interfaceIdentifier))
                                        .WithIdentifier(SyntaxFactory.Identifier(propertyDeclaration.Identifier.ValueText));

            return VisitPropertyDeclaration(newPropertyToken);
        }

        public override SyntaxNode VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            var basePropertyDeclaration = node.FirstAncestorOrSelf<PropertyDeclarationSyntax>();

            node = GeneratePropertyAccessor(node.Kind(), basePropertyDeclaration.Type);

            return base.VisitAccessorDeclaration(node);
        }

        private AccessorDeclarationSyntax GeneratePropertyAccessor(SyntaxKind syntaxKind, TypeSyntax typeSyntax)
        {
            string memberName = string.Format(
                "{0}{1}",
                GetAccessor(syntaxKind),
                Property);

            var typeList = SyntaxFactory.SeparatedList(new[] { typeSyntax });

            var substitution = SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(SubstitutionContext), SyntaxFactory.GenericName(memberName)
                                            .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(typeList))));

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
            StatementSyntax statement;
            if (accessor == SyntaxKind.SetAccessorDeclaration)
                statement = SyntaxFactory.ExpressionStatement(substitutionCall);
            else
                statement = SyntaxFactory.ReturnStatement(substitutionCall);

            var accessorDeclarationSyntax = SyntaxFactory.AccessorDeclaration(
                accessor,
                SyntaxFactory.Block(statement));

            return accessorDeclarationSyntax;
        }
    }
}