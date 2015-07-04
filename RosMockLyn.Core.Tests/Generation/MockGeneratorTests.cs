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
using FluentAssertions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using NSubstitute;

using NUnit.Framework;

using RosMockLyn.Core.Generation;
using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core.Tests.Generation
{
    [TestFixture]
    public class MockGeneratorTests
    {
        private MockGenerator _generator;

        private ICodeTransformer _transformer;

        [SetUp]
        public void SetUp()
        {
            _transformer = Substitute.For<ICodeTransformer>();

            _generator = new MockGenerator(new[] {_transformer}, null);
        }

        [Test, Category("Unit Test")]
        public void GenerateMock_ShouldReturnSyntaxTree()
        {
            // Arrange
            _transformer.Type.Returns(GeneratorType.Using);
            _transformer.Transform(Arg.Any<SyntaxNode>()).Returns(SyntaxFactory.CompilationUnit());
            var tree = SyntaxFactory.CompilationUnit().SyntaxTree;

            // Act
            SyntaxTree result = _generator.GenerateMock(tree);

            // Assert
            result.Should().NotBeNull();
        }

        [Test, Category("Unit Test")]
        public void VisitCompilationUnit_ShouldCallInterfaceTransformer()
        {
            // Arrange
            _transformer.Type.Returns(GeneratorType.Using);
            _transformer.Transform(Arg.Any<SyntaxNode>()).Returns(SyntaxFactory.CompilationUnit());

            // Act
            _generator.VisitCompilationUnit(null);

            // Assert
            _transformer.Received(1).Transform(Arg.Any<SyntaxNode>());
        }

        [Test, Category("Unit Test")]
        public void VisitNamespaceDeclaration_ShouldCallInterfaceTransformer()
        {
            // Arrange
            _transformer.Type.Returns(GeneratorType.Namespace);
            _transformer.Transform(Arg.Any<SyntaxNode>()).Returns(SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("x")));

            // Act
            _generator.VisitNamespaceDeclaration(null);

            // Assert
            _transformer.Received(1).Transform(Arg.Any<SyntaxNode>());
        }

        [Test, Category("Unit Test")]
        public void VisitInterfaceDeclaration_ShouldCallInterfaceTransformer()
        {
            // Arrange
            _transformer.Type.Returns(GeneratorType.Interface);
            _transformer.Transform(Arg.Any<SyntaxNode>()).Returns(SyntaxFactory.ClassDeclaration("x"));

            // Act
            _generator.VisitInterfaceDeclaration(null);

            // Assert
            _transformer.Received(1).Transform(Arg.Any<SyntaxNode>());
        }

        [Test, Category("Unit Test")]
        public void VisitPropertyDeclaration_ShouldCallInterfaceTransformer()
        {
            // Arrange
            _transformer.Type.Returns(GeneratorType.Property);
            _transformer.Transform(Arg.Any<SyntaxNode>())
                .Returns(SyntaxFactory.PropertyDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)), "x"));

            // Act
            _generator.VisitPropertyDeclaration(null);

            // Assert
            _transformer.Received(1).Transform(Arg.Any<SyntaxNode>());
        }

        [Test, Category("Unit Test")]
        public void VisitIndexerDeclaration_ShouldCallInterfaceTransformer()
        {
            // Arrange
            _transformer.Type.Returns(GeneratorType.Indexer);
            _transformer.Transform(Arg.Any<SyntaxNode>())
                .Returns(SyntaxFactory.IndexerDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))));

            // Act
            _generator.VisitIndexerDeclaration(null);

            // Assert
            _transformer.Received(1).Transform(Arg.Any<SyntaxNode>());
        }
    }
}