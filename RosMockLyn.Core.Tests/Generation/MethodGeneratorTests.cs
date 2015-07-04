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

using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

using RosMockLyn.Core.Generation;

namespace RosMockLyn.Core.Tests.Generation
{
    [TestFixture]
    public class MethodGeneratorTests
    {
        private MethodGenerator _methodGenerator;

        [SetUp]
        public void SetUp()
        {
            _methodGenerator = new MethodGenerator();
        }

        [Test, Category("Unit Test")]
        public void Generate_ReturnsMethodDeclarationSyntax()
        {
            // Arrange
            // Act
            var syntaxNode = _methodGenerator.Generate(new MethodData("IInterface", "TestMethod", "Void", Enumerable.Empty<Parameter>()));

            // Assert
            syntaxNode.Should().BeOfType<MethodDeclarationSyntax>();
        }

        [Test, Category("Unit Test")]
        public void Generate_MethodInfo_ReturnedMethodHasCorrectName()
        {
            // Arrange
            const string methodName = "TestMethod";

            // Act
            var syntaxNode = _methodGenerator.Generate(new MethodData("IInterface", methodName, "Void", Enumerable.Empty<Parameter>()));

            // Assert
            syntaxNode.Should().BeOfType<MethodDeclarationSyntax>()
                .And.Match<MethodDeclarationSyntax>(x => x.Identifier.ToString() == methodName);
        }

        [Test, Category("Unit Test")]
        public void Generate_MethodWithParameters_ReturnedMethodHasParameters()
        {
            // Arrange
            var parameter = new Parameter("int", "i");

            // Act
            var syntaxNode = _methodGenerator.Generate(new MethodData("IInterface", "TestMethod", "Void", new List<Parameter> { parameter }));

            // Assert
            syntaxNode.Should().BeOfType<MethodDeclarationSyntax>()
                .And.Match<MethodDeclarationSyntax>(x => x.ParameterList.Parameters.Count > 0);
        }

        [Test, Category("Unit Test")]
        public void Generate_MethodWithReturnValue_ReturnedMethodHasAReturnValue()
        {
            // Arrange
            // Act
            var syntaxNode = _methodGenerator.Generate(new MethodData("IInterface", "TestMethod", "int", Enumerable.Empty<Parameter>()));

            // Assert
            syntaxNode.Should().BeOfType<MethodDeclarationSyntax>()
                .And.Match<MethodDeclarationSyntax>(x => x.ReturnType.ChildTokens().All(t => !t.IsKind(SyntaxKind.VoidKeyword)));
        }

        [Test, Category("Unit Test")]
        public void Generate_Method_ReturnedMethodHasSubstitutionContextCall()
        {
            // Arrange
            // Act
            var syntaxNode = _methodGenerator.Generate(new MethodData("IInterface", "TestMethod", "Void", Enumerable.Empty<Parameter>()));

            // Assert
            var memberAccessExpressionSyntaxes = syntaxNode.DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>();

            memberAccessExpressionSyntaxes.Should().NotBeEmpty();

            var memberAccessExpression = memberAccessExpressionSyntaxes.First();

            memberAccessExpression.Name.ToString().Should().Be("Method");
        }

        [Test, Category("Unit Test")]
        public void Generate_MethodWithParameters_ReturnedMethodHasSubstitutionContextCallWithParameters()
        {
            // Arrange
            var parameter = new Parameter("int", "i");

            // Act
            var syntaxNode = _methodGenerator.Generate(new MethodData("IInterface", "TestMethod", "Void", new List<Parameter> { parameter }));

            // Assert
            var memberAccessExpressionSyntaxes = syntaxNode.DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>();

            memberAccessExpressionSyntaxes.Should().NotBeEmpty();

            var memberAccessExpression = memberAccessExpressionSyntaxes.First();
            var invocationExpressionSyntax = syntaxNode.DescendantNodes().OfType<InvocationExpressionSyntax>().First();

            memberAccessExpression.Name.ToString().Should().Be("Method");
            invocationExpressionSyntax.ArgumentList.Arguments.Should().NotBeEmpty();
        }

        [Test, Category("Unit Test")]
        public void Generate_MethodWithReturnValue_ReturnedMethodHasSubstitutionContextCallWithReturnValue()
        {
            // Arrange
            // Act
            var syntaxNode = _methodGenerator.Generate(new MethodData("IInterface", "TestMethod", "int", Enumerable.Empty<Parameter>()));

            // Assert
            var memberAccessExpressionSyntaxes = syntaxNode.DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>();

            memberAccessExpressionSyntaxes.Should().NotBeEmpty();

            var memberAccessExpression = memberAccessExpressionSyntaxes.First().Name.As<GenericNameSyntax>();

            memberAccessExpression.Identifier.ToString().Should().Be("Method");
            memberAccessExpression.TypeArgumentList.Arguments.Should().NotBeEmpty();
        }
    }
}