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
using System.Linq;
using System.Reflection.Metadata;

namespace RosMockLyn.Core
{
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

    // TODO : Change name!
    public class InterfaceMockGenerator : CSharpSyntaxRewriter, IInterfaceMockGenerator
    {
        private NameSyntax _interfaceIdentifier;

        private const string AdditionalUsingPart1 = "RosMockLyn";
        private const string AdditionalUsingPart2 = "Mocking";
        private const string DerivesFrom = "MockBase";
        private const string ClassSuffix = "Mock";
        private const string Namespace = ".RosMockLyn";

        private const string SubstitutionContext = "SubstitutionContext";

        private const string Method = "Method";
        private const string Property = "Property";
        private const string Index = "Index";

        private const string Arguments = "arguments";

        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            var usings = GenerateAdditionalUsings(node);
            node = node.AddUsings(usings);

            return base.VisitCompilationUnit(node);
        }

        private UsingDirectiveSyntax[] GenerateAdditionalUsings(CompilationUnitSyntax node)
        {
            var namespaceDeclaration = node.DescendantNodes().OfType<NamespaceDeclarationSyntax>().Single();

            var additionalUsing =
                SyntaxFactory.UsingDirective(
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.IdentifierName(AdditionalUsingPart1),
                        SyntaxFactory.IdentifierName(AdditionalUsingPart2)));
            var blah =
                SyntaxFactory.UsingDirective(
                    SyntaxFactory.IdentifierName(((IdentifierNameSyntax)namespaceDeclaration.Name).Identifier.ValueText));

            return new[] { additionalUsing, blah };
        }

        public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            node = node.WithName(SyntaxFactory.IdentifierName(GenerateNamespaceIdentifier(node)));

            return base.VisitNamespaceDeclaration(node);
        }

        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            // Class base class visit to traverse down the tree
            // I use "VisitClassDeclaration" instead of "Interface..." because
            // we want to replace the interface with a class!
            // "NormalizeWhitespace" adds needed trivia to correctly format the resulting code (can also be done manually)
           return base.VisitClassDeclaration(MockInterface(node).NormalizeWhitespace());
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var newMethodSyntax = node.WithExplicitInterfaceSpecifier(SyntaxFactory.ExplicitInterfaceSpecifier(_interfaceIdentifier))
                                    .WithBody(GenerateMethodBody(node))
                                      // this removes the trailing semicolon
                                      .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.None));

            return base.VisitMethodDeclaration(newMethodSyntax);
        }

        public override SyntaxNode VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            var basePropertyDeclaration = node.FirstAncestorOrSelf<BasePropertyDeclarationSyntax>();

            var indexerDeclarationSyntax = basePropertyDeclaration as IndexerDeclarationSyntax;

            if (indexerDeclarationSyntax != null)
            {
                var indexDeclaration = indexerDeclarationSyntax;

                node = GenerateIndexAccessor(node.CSharpKind(), basePropertyDeclaration.Type, indexDeclaration.ParameterList.Parameters);
            }
            else if (basePropertyDeclaration is PropertyDeclarationSyntax)
            {
                node = GeneratePropertyAccessor(node.CSharpKind(), basePropertyDeclaration.Type);
            }

            return base.VisitAccessorDeclaration(node);
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            // Replace the original property and just change the name of the identifier.
            var newPropertyToken = node.WithExplicitInterfaceSpecifier(SyntaxFactory.ExplicitInterfaceSpecifier(_interfaceIdentifier))
                                       .WithIdentifier(SyntaxFactory.Identifier(node.Identifier.ValueText));

            // Again, call base method to be able to traverse deeper into the tree, if needed/wanted
            return base.VisitPropertyDeclaration(newPropertyToken);
        }

        public override SyntaxNode VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            // Replace the original indexer and just change the name of the identifier.
            var newPropertyToken = node.WithExplicitInterfaceSpecifier(SyntaxFactory.ExplicitInterfaceSpecifier(_interfaceIdentifier));

            // Again, call base method to be able to traverse deeper into the tree, if needed/wanted
            return base.VisitIndexerDeclaration(newPropertyToken);
        }

        public SyntaxTree GenerateMock(SyntaxTree treeToGenerateMockFrom)
        {
            return SyntaxFactory.SyntaxTree(Visit(treeToGenerateMockFrom.GetRoot()).NormalizeWhitespace());
        }

        private ClassDeclarationSyntax MockInterface(InterfaceDeclarationSyntax interfaceDeclaration)
        {
            _interfaceIdentifier = SyntaxFactory.IdentifierName(interfaceDeclaration.Identifier.ValueText);
            
            return SyntaxFactory.ClassDeclaration(
                SyntaxFactory.Identifier(interfaceDeclaration.Identifier.ValueText.Substring(1) + ClassSuffix))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddBaseListTypes(
                    SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(DerivesFrom)), // MockBase, base type
                    SyntaxFactory.SimpleBaseType(_interfaceIdentifier) // Interface that is being implemented
                )
                .AddMembers(interfaceDeclaration.Members.ToArray());
        }

        private AccessorDeclarationSyntax GenerateIndexAccessor(SyntaxKind syntaxKind, TypeSyntax typeSyntax, SeparatedSyntaxList<ParameterSyntax> parameters)
        {
            string memberName = string.Format(
                "{0}{1}",
                GetAccessor(syntaxKind),
                Index);

            var typeList = SyntaxFactory.SeparatedList(new[] { typeSyntax, parameters.First().Type });
            var indices = parameters.Select(x => SyntaxFactory.IdentifierName(x.Identifier)).Select(SyntaxFactory.Argument);
            var indicesList = SyntaxFactory.ArgumentList(
                    SyntaxFactory.SeparatedList(indices));

            var substitution = SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(SubstitutionContext),
                        SyntaxFactory.GenericName(memberName)
                                                .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(typeList))))
                                            .WithArgumentList(indicesList);

            if (syntaxKind == SyntaxKind.SetAccessorDeclaration)
            {
                var argument = SyntaxFactory.Argument(SyntaxFactory.IdentifierName("value"));
                substitution = substitution.WithArgumentList(
                        SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { argument }).AddRange(indices)));
            }

            return GenerateAccessor(syntaxKind, substitution);
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
                        SyntaxFactory.IdentifierName(SubstitutionContext),SyntaxFactory.GenericName(memberName)
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

        private static string GenerateNamespaceIdentifier(NamespaceDeclarationSyntax node)
        {
            return ((IdentifierNameSyntax)node.Name).Identifier.ValueText + Namespace;
        }

        private BlockSyntax GenerateMethodBody(MethodDeclarationSyntax node)
        {
            var returnType = node.ReturnType;

            var predefinedType = returnType as PredefinedTypeSyntax;

            if (predefinedType != null && predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword))
            {
                return SyntaxFactory.Block(GenerateVoidMethodBody(node));
            }

            return SyntaxFactory.Block(GenerateReturnMethodBody(node, returnType));
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

            var substitution =
                SyntaxFactory.InvocationExpression(
                                        SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(SubstitutionContext),
                        SyntaxFactory.GenericName(Method)
                        .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(typeList))));

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