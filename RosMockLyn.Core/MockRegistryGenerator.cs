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

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RosMockLyn.Core.Helpers;
using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core
{
    internal sealed class MockRegistryGenerator : IMockRegistryGenerator
    {
        private const string RegistryInterfaceNamespace = "RosMockLyn.Mocking.IoC";

        private const string InjectorType = "IInjector";

        private const string InjectorName = "injector";
        
        private const string RegisterType = "RegisterType";
        
        private const string NamespaceName = "MockRegistry";

        private const string MockNamespace = "RosMockLyn";

        private const string RegistryName = "GeneratedRegistry";
        
        private const string RegistryType = "IInjectorRegistry";

        private const string Register = "Register";

        private static readonly SyntaxTokenList PublicModifier = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

        public SyntaxTree GenerateRegistry(IEnumerable<SyntaxTree> interfaces)
        {
            var methodDeclaration = CreateMethodDeclaration(interfaces);
            var classDeclaration = CreateClassDeclaration(methodDeclaration);

            return CreateSyntaxTree(classDeclaration);
        }

        private MethodDeclarationSyntax CreateMethodDeclaration(IEnumerable<SyntaxTree> interfaces)
        {
            var returnType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword));
            var parameters = CreateParameters();
            var methodBlock = CreateMethodBlock(interfaces);

            return SyntaxFactory.MethodDeclaration(returnType, Register)
                    .WithModifiers(PublicModifier)
                    .WithParameterList(parameters)
                    .WithBody(methodBlock);
        }

        private static ParameterListSyntax CreateParameters()
        {
            var injectorType = IdentifierHelper.AppendIdentifier(RegistryInterfaceNamespace, InjectorType);
            var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier(InjectorName))
                                         .WithType(IdentifierHelper.GetIdentifier(injectorType));
            
            return SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(parameter));
        }

        private BlockSyntax CreateMethodBlock(IEnumerable<SyntaxTree> interfaces)
        {
            var generateRegisterStatements = GenerateRegisterStatements(interfaces).ToList();
            var methodBlock = SyntaxFactory.Block(SyntaxFactory.List(generateRegisterStatements));

            return methodBlock;
        }

        private IEnumerable<StatementSyntax> GenerateRegisterStatements(IEnumerable<SyntaxTree> interfaces)
        {
            var tuples = interfaces.Select(CreateNameMapping);

            return tuples.Select(x => GenerateStatement(x.Item1, x.Item2));
        }

        private Tuple<string, string> CreateNameMapping(SyntaxTree tree)
        {
            string fullyQualifiedNamespace = NameHelper.GetFullyQualifiedNamespace(tree.GetRoot());
            string interfaceName = IdentifierHelper.AppendIdentifier(fullyQualifiedNamespace, NameHelper.GetInterfaceName(tree.GetRoot()));
            string mockName = IdentifierHelper.AppendIdentifier(
                fullyQualifiedNamespace,
                MockNamespace,
                NameHelper.GetMockImplementationName(tree.GetRoot()));

            return Tuple.Create(interfaceName, mockName);
        }

        private StatementSyntax GenerateStatement(string interfaceName, string mockName)
        {
            var typeArguments = CreateTypeArguments(interfaceName, mockName);
            var invocation = SyntaxFactory.InvocationExpression(CreateMemberAccessExpression(typeArguments));

            return SyntaxFactory.ExpressionStatement(invocation);
        }

        private static TypeArgumentListSyntax CreateTypeArguments(string interfaceName, string mockName)
        {
            TypeSyntax interfaceNameSyntax = IdentifierHelper.GetIdentifier(interfaceName);
            TypeSyntax mockNameSyntax = IdentifierHelper.GetIdentifier(mockName);

            return SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(new[] { interfaceNameSyntax, mockNameSyntax }));
        }

        private static MemberAccessExpressionSyntax CreateMemberAccessExpression(TypeArgumentListSyntax typeArguments)
        {
            return SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(InjectorName),
                SyntaxFactory.GenericName(RegisterType)
                    .WithTypeArgumentList(typeArguments));
        }

        private static ClassDeclarationSyntax CreateClassDeclaration(MemberDeclarationSyntax methodDeclaration)
        {
            var baseTypeList = CreateBaseTypes();
            var classDeclaration =
                SyntaxFactory.ClassDeclaration(RegistryName)
                    .WithModifiers(PublicModifier)
                    .WithBaseList(baseTypeList)
                    .WithMembers(SyntaxFactory.SingletonList(methodDeclaration));

            return classDeclaration;
        }

        private static BaseListSyntax CreateBaseTypes()
        {
            var registryType = IdentifierHelper.AppendIdentifier(RegistryInterfaceNamespace, RegistryType);

            return SyntaxFactory.BaseList(
                    SyntaxFactory.SeparatedList(
                        new BaseTypeSyntax[] { SyntaxFactory.SimpleBaseType(IdentifierHelper.GetIdentifier(registryType)) }));
        }

        private static SyntaxTree CreateSyntaxTree(ClassDeclarationSyntax classDeclaration)
        {
            var namespaceDeclaration =
                SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(NamespaceName))
                    .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(classDeclaration));

            var compilationUnit =
                SyntaxFactory.CompilationUnit()
                    .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(namespaceDeclaration));

            return compilationUnit.SyntaxTree;
        }
    }
}