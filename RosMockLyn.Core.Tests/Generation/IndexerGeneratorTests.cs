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

using System.Linq;

using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

using RosMockLyn.Core.Generation;

namespace RosMockLyn.Core.Tests.Generation
{
    [TestFixture]
    public class IndexerGeneratorTests
    {
        private IndexerGenerator _indexerGenerator;

        [SetUp]
        public void SetUp()
        {
            _indexerGenerator = new IndexerGenerator();
        }

        [Test, Category("Unit Test")]
        public void Generate_IndexerData_ReturnsIndexerDeclarationSyntax()
        {
            // Arrange
            // Act
            var syntaxNode = _indexerGenerator.Generate(new IndexerData("IInterface", "int", new[] { new Parameter("int", "i") }, true));

            // Assert
            syntaxNode.Should().BeOfType<IndexerDeclarationSyntax>();
        }

        [Test, Category("Unit Test")]
        public void Generate_IndexerData_ReturnedIndexerHasCorrectParameters()
        {
            // Arrange
            const string ParameterType = "int";
            const string ParameterName = "i";

            // Act
            var syntaxNode = _indexerGenerator.Generate(new IndexerData("IInterface", "int", new[] { new Parameter(ParameterType, ParameterName), }, true));

            // Assert
            syntaxNode.Should().BeOfType<IndexerDeclarationSyntax>()
                .And.Match<IndexerDeclarationSyntax>(x => x.ParameterList.Parameters.Any(y => y.Type.ToString() == ParameterType && y.Identifier.ToString() == ParameterName));
        }

        [Test, Category("Unit Test")]
        public void Generate_IndexerWithNoSetter_ReturnedIndexerHasNoSetter()
        {
            // Arrange
            // Act
            var syntaxNode = _indexerGenerator.Generate(new IndexerData("IInterface", "int", new[] { new Parameter("int", "i"), }, false));

            // Assert
            syntaxNode.Should().BeOfType<IndexerDeclarationSyntax>();
            syntaxNode.DescendantNodes().OfType<AccessorDeclarationSyntax>().Should().HaveCount(1);
            syntaxNode.DescendantTokens().Select(x => x.Kind()).Should().NotContain(SyntaxKind.SetKeyword);
        }

        [Test, Category("Unit Test")]
        public void Generate_Indexer_ReturnedIndexerHasAGetter()
        {
            // Arrange
            // Act
            var syntaxNode = _indexerGenerator.Generate(new IndexerData("IInterface", "int", new[] { new Parameter("int", "i"), }, false));

            // Assert
            syntaxNode.Should().BeOfType<IndexerDeclarationSyntax>();
            syntaxNode.DescendantTokens().Select(x => x.Kind()).Should().Contain(SyntaxKind.GetKeyword);
        }

        [Test, Category("Unit Test")]
        public void Generate_IndexerWithSetter_ReturnedIndexerHasASetter()
        {
            // Arrange
            // Act
            var syntaxNode = _indexerGenerator.Generate(new IndexerData("IInterface", "int", new[] { new Parameter("int", "i"), }, true));

            // Assert
            syntaxNode.Should().BeOfType<IndexerDeclarationSyntax>();
            syntaxNode.DescendantNodes().OfType<AccessorDeclarationSyntax>().Should().HaveCount(2);
            syntaxNode.DescendantTokens().Select(x => x.Kind()).Should().Contain(SyntaxKind.SetKeyword);
        }

        [Test, Category("Unit Test")]
        public void Generate_Indexer_ReturnedIndexerHasTheSpecifiedReturnType()
        {
            // Arrange
            // Act
            const string ReturnType = "int";

            var syntaxNode = _indexerGenerator.Generate(new IndexerData("IInterface", ReturnType, new[] { new Parameter("int", "i"), }, true));

            // Assert
            syntaxNode.Should().BeOfType<IndexerDeclarationSyntax>();
            syntaxNode.DescendantNodes().OfType<IdentifierNameSyntax>().Should().Contain(x => x.ToString().Contains(ReturnType));
        }

        [Test, Category("Unit Test")]
        public void Generate_Indexer_ReturnedIndexerHasSubstitutionContextCall()
        {
            // Arrange
            // Act
            var syntaxNode = _indexerGenerator.Generate(new IndexerData("IInterface", "int", new[] { new Parameter("int", "i"), }, false));

            // Assert
            var memberAccessExpressionSyntaxes = syntaxNode.DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>();

            memberAccessExpressionSyntaxes.Should().NotBeEmpty();

            var memberAccessExpression = memberAccessExpressionSyntaxes.First();

            memberAccessExpression.Name.ToString().Should().Contain("GetIndex");
        }

        [Test, Category("Unit Test")]
        public void Generate_IndexerWithSetter_ReturnedIndexerHasSubstitutionContextCallForSetter()
        {
            // Arrange
            // Act
            var syntaxNode = _indexerGenerator.Generate(new IndexerData("IInterface", "int", new[] { new Parameter("int", "i"), }, true));

            // Assert
            var memberAccessExpressionSyntaxes = syntaxNode.DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>();

            memberAccessExpressionSyntaxes.Should().NotBeEmpty();

            var memberAccessExpression = memberAccessExpressionSyntaxes.ElementAt(1);
            var invocationExpressionSyntax = syntaxNode.DescendantNodes().OfType<InvocationExpressionSyntax>().ElementAt(1);

            memberAccessExpression.Name.ToString().Should().Contain("SetIndex");
            invocationExpressionSyntax.ArgumentList.Arguments.Should().NotBeEmpty();
        }
    }
}