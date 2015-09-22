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
using System.Linq;

using FluentAssertions;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

using RosMockLyn.Core.Interfaces;
using RosMockLyn.Core.Transformation;

namespace RosMockLyn.Core.Tests.Transformation
{
    [TestFixture]
    public class MethodTransformerTests
    {
        private MethodTransformer _transformer;

        [SetUp]
        public void SetUp()
        {
            _transformer = new MethodTransformer();
        }

        [Test, Category("Unit Test")]
        public void Type_ShouldReturn_Method()
        {
            // Arrange
            // Act
            // Assert
            _transformer.Type.Should().Be(GeneratorType.Method);
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldThrowException_WhenNotMethodDeclarationSyntax()
        {
            // Arrange
            // Act
            Action action = () => _transformer.Transform(SyntaxFactory.IdentifierName("x"));

            // Assert
            action.ShouldThrow<InvalidOperationException>().WithMessage("Provided*MethodDeclaration.");
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldReturnMethodDeclaration_WithExplicitInterfaceImplementation()
        {
            // Arrange
            string interfaceName = "IMyInterface";
            string methodName = "MyMethod";

            var methodDeclarationSyntax = CreateMethodDeclaration(interfaceName, methodName);

            // Act
            var result = (MethodDeclarationSyntax)_transformer.Transform(methodDeclarationSyntax);

            // Assert
            result.Identifier.ToString().Should().Be(methodName);
            result.ExplicitInterfaceSpecifier.ToString().Should().StartWith(interfaceName);
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldReturnMethodDeclaration_WithReturnType_Void()
        {
            // Arrange
            string interfaceName = "IMyInterface";
            string methodName = "MyMethod";

            var methodDeclarationSyntax = CreateMethodDeclaration(interfaceName, methodName);

            // Act
            var result = (MethodDeclarationSyntax)_transformer.Transform(methodDeclarationSyntax);

            // Assert
            result.ReturnType.ToString().Should().Contain("void");
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldReturnMethodDeclaration_WithSpecifiedReturnType()
        {
            // Arrange
            string interfaceName = "IMyInterface";
            string methodName = "MyMethod";
            string returnType = "MyType";

            var methodDeclarationSyntax = CreateMethodDeclarationWithReturnType(
                interfaceName,
                methodName,
                SyntaxFactory.IdentifierName(returnType));

            // Act
            var result = (MethodDeclarationSyntax)_transformer.Transform(methodDeclarationSyntax);

            // Assert
            result.ReturnType.ToString().Should().Contain(returnType);
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldReturnMethodDeclaration_WithMethodCallInside()
        {
            // Arrange
            string interfaceName = "IMyInterface";
            string methodName = "MyMethod";

            var methodDeclarationSyntax = CreateMethodDeclaration(interfaceName, methodName);

            // Act
            var result = (MethodDeclarationSyntax)_transformer.Transform(methodDeclarationSyntax);

            // Assert
            result.Body.DescendantNodes().OfType<InvocationExpressionSyntax>().Should().NotBeEmpty();
            result.Body.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Should().NotBeEmpty();
            result.Body.DescendantNodes().OfType<GenericNameSyntax>().Should().BeEmpty();
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldReturnMethodDeclaration_WithGenericMethodCallInside()
        {
            // Arrange
            string interfaceName = "IMyInterface";
            string methodName = "MyMethod";
            string returnType = "MyType";

            var methodDeclarationSyntax = CreateMethodDeclarationWithReturnType(
                interfaceName,
                methodName,
                SyntaxFactory.IdentifierName(returnType));

            // Act
            var result = (MethodDeclarationSyntax)_transformer.Transform(methodDeclarationSyntax);

            // Assert
            result.Body.DescendantNodes().OfType<InvocationExpressionSyntax>().Should().NotBeEmpty();
            result.Body.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Should().NotBeEmpty();
            result.Body.DescendantNodes().OfType<GenericNameSyntax>().Should().NotBeEmpty();
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldReturnMethodDeclaration_WithParameter()
        {
            // Arrange
            string interfaceName = "IMyInterface";
            string methodName = "MyMethod";
            string parameterName = "parameter";

            var methodDeclarationSyntax = CreateMethodDeclarationWithParameter(
                interfaceName,
                methodName,
                parameterName);

            // Act
            var result = (MethodDeclarationSyntax)_transformer.Transform(methodDeclarationSyntax);

            // Assert
            result.Body.DescendantNodes().OfType<ArgumentListSyntax>().Should().NotBeEmpty();
        }

        private MethodDeclarationSyntax CreateMethodDeclarationWithReturnType(string interfaceName, string methodName, TypeSyntax returnType)
        {
            var methodDeclaration = SyntaxFactory.MethodDeclaration(returnType, methodName);

            var baseType = SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(interfaceName));

            var classDeclaration = SyntaxFactory.ClassDeclaration("SomeClass")
                .AddBaseListTypes(baseType, baseType)
                .AddMembers(methodDeclaration);

            return classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
        }

        private MethodDeclarationSyntax CreateMethodDeclarationWithParameter(string interfaceName, string methodName, string parameterName)
        {
            var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameterName));
            var returnType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword));

            var methodDeclaration = SyntaxFactory.MethodDeclaration(returnType, methodName).AddParameterListParameters(parameter);

            var baseType = SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(interfaceName));

            var classDeclaration = SyntaxFactory.ClassDeclaration("SomeClass")
                .AddBaseListTypes(baseType, baseType)
                .AddMembers(methodDeclaration);

            return classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
        }

        private MethodDeclarationSyntax CreateMethodDeclaration(string interfaceName, string methodName)
        {
            TypeSyntax type = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword));

            return CreateMethodDeclarationWithReturnType(interfaceName, methodName, type);
        }
    }
}