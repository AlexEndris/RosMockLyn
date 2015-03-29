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

using RosMockLyn.Core.Transformation;

namespace RosMockLyn.Core
{
    public class MockRegistryGenerator
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

        public SyntaxTree GenerateRegistry(IEnumerable<SyntaxTree> interfaces)
        {
            var usingDirectiveSyntaxes = GenerateUsings(interfaces);
            var generateRegisterStatements = GenerateRegisterStatements(interfaces).ToList();

            var publicModifier = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var baseTypeList = SyntaxFactory.BaseList(
                                SyntaxFactory.SeparatedList(new BaseTypeSyntax[]
                            {
                                SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(RegistryType))
                            }));

            var returnType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword));

            var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier(InjectorName))
                .WithType(SyntaxFactory.IdentifierName(InjectorType));

            var parameters = SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(parameter));

            var methodBlock = SyntaxFactory.Block(SyntaxFactory.List(generateRegisterStatements));

            var methodDeclaration = SyntaxFactory.MethodDeclaration(returnType, Register)
                .WithModifiers(publicModifier)
                .WithParameterList(parameters)
                .WithBody(methodBlock);
            
            var classDeclaration = SyntaxFactory.ClassDeclaration(RegistryName)
                .WithModifiers(publicModifier)
                .WithBaseList(baseTypeList)
                .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(methodDeclaration));

            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(NamespaceName))
                .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(classDeclaration));

            var compilationUnit = SyntaxFactory.CompilationUnit()
                .WithUsings(usingDirectiveSyntaxes)
                .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(namespaceDeclaration));

            return compilationUnit.SyntaxTree;
        }

        private SyntaxList<UsingDirectiveSyntax> GenerateUsings(IEnumerable<SyntaxTree> interfaces)
        {
            List<UsingDirectiveSyntax> usings = new List<UsingDirectiveSyntax>();

            foreach (var tree in interfaces)
            {
                var namespaceDeclaration = (from node in tree.GetRoot().DescendantNodes().OfType<NamespaceDeclarationSyntax>()
                                      select node).First();

                var usingDirective = CreateUsingDirective(namespaceDeclaration.Name);
                var mockUsingDirective =
                    CreateUsingDirective(
                        SyntaxFactory.QualifiedName(
                            namespaceDeclaration.Name,
                            SyntaxFactory.IdentifierName(MockNamespace)));
                
                string name = usingDirective.Name.ToString();

                if (usings.Select(x => x.Name.ToString()).All(x => x != name))
                {
                    usings.Add(usingDirective);
                    usings.Add(mockUsingDirective);
                }
            }
            usings.Add(SyntaxFactory.UsingDirective(CreateMockingUsing()));

            return SyntaxFactory.List(usings);
        }

        private static QualifiedNameSyntax CreateMockingUsing()
        {
            var namespaceParts = RegistryInterfaceNamespace.Split('.');
            return SyntaxFactory.QualifiedName(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.IdentifierName(namespaceParts[0]),
                            SyntaxFactory.IdentifierName(namespaceParts[1])),
                        SyntaxFactory.IdentifierName(namespaceParts[2]));
        }

        private IEnumerable<StatementSyntax> GenerateRegisterStatements(IEnumerable<SyntaxTree> interfaces)
        {
            var dictionary = interfaces.Select(CreateNameMapping).ToDictionary(x => x.Item1, x => x.Item2);

            return dictionary.Select(x => GenerateStatement(x.Key, x.Value));
        }

        private UsingDirectiveSyntax CreateUsingDirective(NameSyntax name)
        {
            return SyntaxFactory.UsingDirective(name);
        }

        private Tuple<string, string> CreateNameMapping(SyntaxTree tree)
        {
            string interfaceName = NameHelper.GetInterfaceName(tree.GetRoot());
            string mockName = NameHelper.GetImplementationName(interfaceName);

            return Tuple.Create(interfaceName, mockName);
        }

        private StatementSyntax GenerateStatement(string interfaceName, string mockName)
        {
            TypeSyntax interfaceNameSyntax = SyntaxFactory.IdentifierName(interfaceName);
            TypeSyntax mockNameSyntax = SyntaxFactory.IdentifierName(mockName);

            var typeArguments = SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(new[] { interfaceNameSyntax, mockNameSyntax }));

            var invocation = SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName(InjectorName),
                                    SyntaxFactory.GenericName(RegisterType)
                                        .WithTypeArgumentList(typeArguments)));

            return SyntaxFactory.ExpressionStatement(invocation);
        }
    }
}