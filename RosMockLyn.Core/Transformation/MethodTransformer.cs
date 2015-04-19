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
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RosMockLyn.Core.Helpers;
using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core.Transformation
{
    internal sealed class MethodTransformer : ICodeTransformer
    {
        private const string SubstitutionContext = "SubstitutionContext";
        private const string Method = "Method";
        private const string Arguments = "arguments";

        public TransformerType Type
        {
            get
            {
                return TransformerType.Method;
            }
        }

        public SyntaxNode Transform(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            var methodDeclaration = node as MethodDeclarationSyntax;

            if (methodDeclaration == null)
                throw new InvalidOperationException("Provided node must be a MethodDeclaration.");

            var interfaceIdentifier = NameHelper.GetBaseInterfaceIdentifier(node);

            var newMethodSyntax = methodDeclaration.WithExplicitInterfaceSpecifier(SyntaxFactory.ExplicitInterfaceSpecifier(interfaceIdentifier))
                               .WithBody(GenerateMethodBody(methodDeclaration))
                               .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.None)); // this removes the trailing semicolon

            return newMethodSyntax;
        }

        private BlockSyntax GenerateMethodBody(MethodDeclarationSyntax node)
        {
            var returnType = node.ReturnType;

            if (IsReturnTypeVoid(returnType))
            {
                return SyntaxFactory.Block(GenerateVoidMethodBody(node));
            }

            return SyntaxFactory.Block(GenerateReturnMethodBody(node, returnType));
        }

        private static bool IsReturnTypeVoid(TypeSyntax returnType)
        {
            var predefinedType = returnType as PredefinedTypeSyntax;

            return predefinedType != null && predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword);
        }

        private StatementSyntax GenerateReturnMethodBody(MethodDeclarationSyntax node, TypeSyntax returnType)
        {
            var substitution = GenerateGenericMethodSubstitution(node, returnType);

            return SyntaxFactory.ReturnStatement(substitution);
        }

        private StatementSyntax GenerateVoidMethodBody(MethodDeclarationSyntax node)
        {
            var substitution = GenerateMethodSubstitution(node);

            return SyntaxFactory.ExpressionStatement(substitution);
        }

        private static InvocationExpressionSyntax GenerateMethodSubstitution(MethodDeclarationSyntax node)
        {
            var substitution =
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(SubstitutionContext),
                        SyntaxFactory.IdentifierName(Method)));

            substitution = AddMethodArguments(node, substitution);

            return substitution;
        }

        private static InvocationExpressionSyntax GenerateGenericMethodSubstitution(MethodDeclarationSyntax node, TypeSyntax returnType)
        {
            var typeList = SyntaxFactory.SeparatedList(new[] { returnType });

            var substitution = CreateInvocationExpression(typeList);

            substitution = AddMethodArguments(node, substitution);

            return substitution;
        }

        private static InvocationExpressionSyntax AddMethodArguments(
            MethodDeclarationSyntax node,
            InvocationExpressionSyntax substitution)
        {
            var parameters = node.ParameterList.Parameters;

            if (parameters.Any())
            {
                var arguments = parameters.Select(x => SyntaxFactory.IdentifierName(x.Identifier));

                var namedArgument = SyntaxFactory.NameColon(SyntaxFactory.IdentifierName(Arguments));
                var newObjectArry = CreateObjectArray(arguments);

                var argumentList =
                    SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(newObjectArry).WithNameColon(namedArgument));

                substitution = substitution.WithArgumentList(SyntaxFactory.ArgumentList(argumentList));
            }

            return substitution;
        }

        private static InvocationExpressionSyntax CreateInvocationExpression(SeparatedSyntaxList<TypeSyntax> typeList)
        {
            var substitution =
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(SubstitutionContext),
                        SyntaxFactory.GenericName(Method).WithTypeArgumentList(SyntaxFactory.TypeArgumentList(typeList))));
            return substitution;
        }

        private static ArrayCreationExpressionSyntax CreateObjectArray(IEnumerable<IdentifierNameSyntax> arguments)
        {
            return SyntaxFactory.ArrayCreationExpression(SyntaxFactory.ArrayType(
                SyntaxFactory.PredefinedType(
                    SyntaxFactory.Token(SyntaxKind.ObjectKeyword)))
                    .WithRankSpecifiers(SyntaxFactory.SingletonList(
                                                            SyntaxFactory.ArrayRankSpecifier(
                                                                SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                                                    SyntaxFactory.OmittedArraySizeExpression())))))
                .WithInitializer(SyntaxFactory.InitializerExpression(
                    SyntaxKind.ArrayInitializerExpression,
                    SyntaxFactory.SeparatedList<ExpressionSyntax>(arguments)));
        }
    }
}