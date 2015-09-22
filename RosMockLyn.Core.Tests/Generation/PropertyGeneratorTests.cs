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
    public class PropertyGeneratorTests
    {
        private PropertyGenerator _propertyGenerator;

        [SetUp]
        public void SetUp()
        {
            _propertyGenerator = new PropertyGenerator();
        }

        [Test, Category("Unit Test")]
        public void Generate_PropertyData_ReturnsPropertyDeclarationSyntax()
        {
            // Arrange
            // Act
            var syntaxNode = _propertyGenerator.Generate(new PropertyData("IInterface", "int", "Name", true));

            // Assert
            syntaxNode.Should().BeOfType<PropertyDeclarationSyntax>();
        }

        [Test, Category("Unit Test")]
        public void Generate_PropertyData_ReturnedPropertyHasCorrectName()
        {
            // Arrange
            const string PropertyName = "TestProperty";

            // Act
            var syntaxNode = _propertyGenerator.Generate(new PropertyData("IInterface", "int", PropertyName, true));

            // Assert
            syntaxNode.Should().BeOfType<PropertyDeclarationSyntax>()
                .And.Match<PropertyDeclarationSyntax>(x => x.Identifier.ToString() == PropertyName);
        }

        [Test, Category("Unit Test")]
        public void Generate_PropertyWithNoSetter_ReturnedPropertyHasNoSetter()
        {
            // Arrange
            // Act
            var syntaxNode = _propertyGenerator.Generate(new PropertyData("IInterface", "int", "TestMethod", false));

            // Assert
            syntaxNode.Should().BeOfType<PropertyDeclarationSyntax>();
            syntaxNode.DescendantNodes().OfType<AccessorDeclarationSyntax>().Should().HaveCount(1);
            syntaxNode.DescendantTokens().Select(x => x.Kind()).Should().NotContain(SyntaxKind.SetKeyword);
        }

        [Test, Category("Unit Test")]
        public void Generate_Property_ReturnedPropertyHasAGetter()
        {
            // Arrange
            // Act
            var syntaxNode = _propertyGenerator.Generate(new PropertyData("IInterface", "int", "TestMethod", false));

            // Assert
            syntaxNode.Should().BeOfType<PropertyDeclarationSyntax>();
            syntaxNode.DescendantTokens().Select(x => x.Kind()).Should().Contain(SyntaxKind.GetKeyword);
        }

        [Test, Category("Unit Test")]
        public void Generate_PropertyWithSetter_ReturnedPropertyHasASetter()
        {
            // Arrange
            // Act
            var syntaxNode = _propertyGenerator.Generate(new PropertyData("IInterface", "int", "TestMethod", true));

            // Assert
            syntaxNode.Should().BeOfType<PropertyDeclarationSyntax>();
            syntaxNode.DescendantNodes().OfType<AccessorDeclarationSyntax>().Should().HaveCount(2);
            syntaxNode.DescendantTokens().Select(x => x.Kind()).Should().Contain(SyntaxKind.SetKeyword);
        }

        [Test, Category("Unit Test")]
        public void Generate_Property_ReturnedPropertyHasTheSpecifiedReturnType()
        {
            // Arrange
            // Act
            const string ReturnType = "int";

            var syntaxNode = _propertyGenerator.Generate(new PropertyData("IInterface", ReturnType, "TestMethod", true));

            // Assert
            syntaxNode.Should().BeOfType<PropertyDeclarationSyntax>();
            syntaxNode.DescendantNodes().OfType<IdentifierNameSyntax>().Should().Contain(x => x.ToString().Contains(ReturnType));
        }

        [Test, Category("Unit Test")]
        public void Generate_Property_ReturnedPropertyHasSubstitutionContextCall()
        {
            // Arrange
            // Act
            var syntaxNode = _propertyGenerator.Generate(new PropertyData("IInterface", "int", "TestMethod", false));

            // Assert
            var memberAccessExpressionSyntaxes = syntaxNode.DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>();

            memberAccessExpressionSyntaxes.Should().NotBeEmpty();

            var memberAccessExpression = memberAccessExpressionSyntaxes.First();

            memberAccessExpression.Name.ToString().Should().Contain("GetProperty");
        }

        [Test, Category("Unit Test")]
        public void Generate_PropertyWithSetter_ReturnedPropertyHasSubstitutionContextCallForSetter()
        {
            // Arrange
            // Act
            var syntaxNode = _propertyGenerator.Generate(new PropertyData("IInterface", "int", "TestMethod", true));

            // Assert
            var memberAccessExpressionSyntaxes = syntaxNode.DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>();

            memberAccessExpressionSyntaxes.Should().NotBeEmpty();

            var memberAccessExpression = memberAccessExpressionSyntaxes.ElementAt(1);
            var invocationExpressionSyntax = syntaxNode.DescendantNodes().OfType<InvocationExpressionSyntax>().ElementAt(1);

            memberAccessExpression.Name.ToString().Should().Contain("SetProperty");
            invocationExpressionSyntax.ArgumentList.Arguments.Should().NotBeEmpty();
        }
    }
}