﻿// Copyright (c) 2015, Alexander Endris
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
using System.Linq;

using FluentAssertions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

using RosMockLyn.Core.Helpers;
using RosMockLyn.Core.Preparation;

namespace RosMockLyn.Core.Tests.Preparation
{
    [TestFixture]
    public class InterfaceExtractorTests
    {
        private InterfaceExtractor _extractor;

        [SetUp]
        public void SetUp()
        {
            _extractor = new InterfaceExtractor();
        }

        [Test, Category("Unit Test")]
        public void Extract_ShouldFilterRosMockLynMockingNamespace()
        {
            // Arrange
            var compilation = CreateCompilationWithInterface("RosMockLyn.Mocking", "x");
            var project = CreateProjectWithSyntaxNode(compilation);

            // Act
            var result = _extractor.Extract(project);

            // Assert
            result.Should().BeEmpty();
        }

        [Test, Category("Unit Test")]
        public void Extract_ShouldFilterNonInterfaceTrees()
        {
            // Arrange
            var compilation = CreateCompilationWithClass("SomeNameSpace", "x");
            var project = CreateProjectWithSyntaxNode(compilation);

            // Act
            var result = _extractor.Extract(project);

            // Assert
            result.Should().BeEmpty();
        }

        [Test, Category("Unit Test")]
        public void Extract_ShouldReturnInterfaces()
        {
            // Arrange
            string interfaceName = "IMyInterface";
            var compilation = CreateCompilationWithInterface("SomeNameSpace", interfaceName);
            var project = CreateProjectWithSyntaxNode(compilation);

            // Act
            var result = _extractor.Extract(project);

            // Assert
            result.Should().NotBeEmpty();
            result.First().GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>()
                .Should().Contain(x => x.Identifier.ToString() == interfaceName);
        }

        [Test, Category("Unit Test")]
        public void GetUsedInterfaces_ShouldReturnNamesOfUsedInterfaces()
        {
            // Arrange
            string interfaceName = "IMyInterface";
            var compilation = CreateClassWithMockCalls(interfaceName);
            var project = CreateProjectWithSyntaxNode(compilation);
            
            // Act
            var result = _extractor.GetUsedInterfaceNames(project);

            // Assert
            result.Should().Contain(interfaceName);
        }

        [Test, Category("Unit Test")]
        public void GetUsedInterfaces_ShouldReturnNamesOfUsedInterfaces_OnlyOnce()
        {
            // Arrange
            string interfaceName = "IMyInterface";
            var compilation = CreateClassWithMockCalls(interfaceName, interfaceName);
            var project = CreateProjectWithSyntaxNode(compilation);
            
            // Act
            var result = _extractor.GetUsedInterfaceNames(project);

            // Assert
            result.Should().HaveCount(1);
            result.Should().Contain(interfaceName);
        }

        [Test, Category("Unit Test")]
        public void GetUsedInterfaces_ShouldReturnNamesOfUsedInterfaces_AllDistinct()
        {
            // Arrange
            string interfaceName = "IMyInterface";
            string interfaceName2 = "IMyInterface2";
            var compilation = CreateClassWithMockCalls(interfaceName, interfaceName2);
            var project = CreateProjectWithSyntaxNode(compilation);
            
            // Act
            var result = _extractor.GetUsedInterfaceNames(project);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(interfaceName)
                .And.Contain(interfaceName2);
        }

        private Project CreateProjectWithSyntaxNode(SyntaxNode root)
        {
            var workspace = new AdhocWorkspace();
            return workspace.AddProject("Project", "C#").AddDocument("Tree", root).Project;
        }

        private SyntaxNode CreateCompilationWithInterface(string namespaceName, string interfaceName)
        {
            var interfaceDeclaration = SyntaxFactory.InterfaceDeclaration(interfaceName);

            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(IdentifierHelper.GetIdentifier(namespaceName))
                .AddMembers(interfaceDeclaration);

            return SyntaxFactory.CompilationUnit().AddMembers(namespaceDeclaration);
        }

        private SyntaxNode CreateCompilationWithClass(string namespaceName, string className)
        {
            var interfaceDeclaration = SyntaxFactory.ClassDeclaration(className);

            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(IdentifierHelper.GetIdentifier(namespaceName))
                .AddMembers(interfaceDeclaration);

            return SyntaxFactory.CompilationUnit().AddMembers(namespaceDeclaration);
        }

        private SyntaxNode CreateClassWithMockCalls(params string[] interfaceNames)
        {
            return
                SyntaxFactory.CompilationUnit()
                    .WithMembers(
                        SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                            SyntaxFactory.ClassDeclaration(@"Test")
                                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                                .WithMembers(
                                    SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                                        SyntaxFactory.MethodDeclaration(
                                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                                            SyntaxFactory.Identifier(@"Test_Methods"))
                                            .WithModifiers(
                                                SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                                            .WithBody(
                                                SyntaxFactory.Block(
                                                    SyntaxFactory.List<StatementSyntax>(
                                                        interfaceNames.Select(CreateMockForCall))))))));
        }

        private ExpressionStatementSyntax CreateMockForCall(string interfaceName)
        {
            return SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(@"Mock"),
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(@"For"))
                        .WithTypeArgumentList(
                            SyntaxFactory.TypeArgumentList(
                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                    SyntaxFactory.IdentifierName(interfaceName)))))));
        }
    }
}