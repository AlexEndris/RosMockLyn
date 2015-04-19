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

using System.Linq;

using FluentAssertions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

using RosMockLyn.Core.Generation;
using RosMockLyn.Core.Helpers;

namespace RosMockLyn.Core.Tests.Generation
{
    [TestFixture]
    public class MockRegistryGeneratorTests
    {
        private MockRegistryGenerator _generator;

        [SetUp]
        public void SetUp()
        {
            _generator = new MockRegistryGenerator();
        }

        [Test, Category("Unit Test")]
        public void GenerateRegistry_ShouldReturnTreeWithInterfaceAsTypeArgument()
        {
            // Arrange
            string interfaceName = "IMyInterface";
            string namespaceName = "Namespace";

            SyntaxTree syntaxTree = GenerateSyntaxTree(interfaceName, namespaceName);

            // Act
            var result = _generator.GenerateRegistry(new[] {syntaxTree});

            // Assert
            var typeArgumentLists = result.GetRoot().DescendantNodesAndSelf().OfType<TypeArgumentListSyntax>();

            typeArgumentLists.Should().NotBeEmpty();
            typeArgumentLists.First()
                .Arguments.Should()
                .Contain(x => ((NameSyntax)x).ToString().Contains(interfaceName));
        }

        [Test, Category("Unit Test")]
        public void GenerateRegistry_ShouldReturnTreeWithMockImplementationAsTypeArgument()
        {
            // Arrange
            string interfaceName = "IMyInterface";
            string mockImplementationName = "MyInterfaceMock";
            string namespaceName = "Namespace";

            SyntaxTree syntaxTree = GenerateSyntaxTree(interfaceName, namespaceName);

            // Act
            var result = _generator.GenerateRegistry(new[] {syntaxTree});

            // Assert
            var typeArgumentLists = result.GetRoot().DescendantNodesAndSelf().OfType<TypeArgumentListSyntax>();

            typeArgumentLists.Should().NotBeEmpty();
            typeArgumentLists.First()
                .Arguments.Should()
                .Contain(x => ((NameSyntax)x).ToString().Contains(mockImplementationName));
        }

        [Test, Category("Unit Test")]
        public void GenerateRegistry_ShouldReturnTreeWithClassBaseTypeIInjectorRegistry()
        {
            // Arrange
            string interfaceName = "IMyInterface";
            string baseType = "IInjectorRegistry";
            string namespaceName = "Namespace";

            SyntaxTree syntaxTree = GenerateSyntaxTree(interfaceName, namespaceName);

            // Act
            var result = _generator.GenerateRegistry(new[] {syntaxTree});

            // Assert
            var typeArgumentLists = result.GetRoot().DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>();

            typeArgumentLists.Should().NotBeEmpty();
            typeArgumentLists.First()
                .BaseList.Types.Should()
                .Contain(x => x.Type.ToString().Contains(baseType));
        }

        private SyntaxTree GenerateSyntaxTree(string interfaceName, string namespaceName)
        {
            var classDeclaration = SyntaxFactory.InterfaceDeclaration(interfaceName);
            
            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(IdentifierHelper.GetIdentifier(namespaceName))
                .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(classDeclaration));

            return
                SyntaxFactory.CompilationUnit()
                    .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(namespaceDeclaration)).SyntaxTree;
        }
    }
}