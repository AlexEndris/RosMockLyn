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
    public class PropertyTransformerTests
    {
        private PropertyTransformer _transformer;

        [SetUp]
        public void SetUp()
        {
            _transformer = new PropertyTransformer();
        }

        [Test, Category("Unit Test")]
        public void Type_ShouldReturn_Property()
        {
            // Arrange
            // Act
            // Assert
            _transformer.Type.Should().Be(GeneratorType.Property);
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldThrowException_WhenNotPropertyDeclarationSyntax()
        {
            // Arrange
            // Act
            Action action = () => _transformer.Transform(SyntaxFactory.IdentifierName("x"));

            // Assert
            action.ShouldThrow<InvalidOperationException>().WithMessage("Provided*PropertyDeclaration.");
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldReturnPropertyDeclaration()
        {
            // Arrange
            var propertyDeclaration = CreatePropertyDeclaration("IInterface", "Prop", "MyType");

            // Act
            var result = _transformer.Transform(propertyDeclaration);

            // Assert
            result.Should().BeOfType<PropertyDeclarationSyntax>();
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldReturnPropertyDeclaration_WithExplicitInterfaceImplementation()
        {
            // Arrange
            string interfaceName = "IMyInterface";
            string propertyName = "Prop";

            var propertyDeclaration = CreatePropertyDeclarationReadonly(interfaceName, propertyName, "MyType");

            // Act
            var result = (PropertyDeclarationSyntax)_transformer.Transform(propertyDeclaration);

            // Assert
            result.Identifier.ToString().Should().Be(propertyName);
            result.ExplicitInterfaceSpecifier.ToString().Should().StartWith(interfaceName);
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldImplementGetAccessor()
        {
            // Arrange
            string invocation = "GetProperty";

            var propertyDeclaration = CreatePropertyDeclarationReadonly("IMyInterface", "Prop", "MyType");

            // Act
            var result = (PropertyDeclarationSyntax)_transformer.Transform(propertyDeclaration);

            // Assert
            result.DescendantNodes().OfType<MemberAccessExpressionSyntax>()
                .Should().Contain(x => x.Name.ToString().Contains(invocation));
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldImplementSetAccessor()
        {
            // Arrange
            string invocation = "SetProperty";

            var propertyDeclaration = CreatePropertyDeclaration("IMyInterface", "Prop", "MyType");

            // Act
            var result = (PropertyDeclarationSyntax)_transformer.Transform(propertyDeclaration);

            // Assert
            result.DescendantNodes().OfType<MemberAccessExpressionSyntax>()
                .Should().Contain(x => x.Name.ToString().Contains(invocation));
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldImplementSetAccessor_WithValueParameter()
        {
            // Arrange
            string value = "value";

            var propertyDeclaration = CreatePropertyDeclaration("IMyInterface", "Prop", "MyType");

            // Act
            var result = (PropertyDeclarationSyntax)_transformer.Transform(propertyDeclaration);

            // Assert
            result.DescendantNodes().OfType<InvocationExpressionSyntax>()
                .Where(x => x.ArgumentList.Arguments.Any())
                .Select(x => x.ArgumentList.Arguments).First()
                .Should().Contain(x => x.Expression.ToString() == value);
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldImplementAccessors_WithTypeParameter()
        {
            // Arrange
            string typeName = "MyType";

            var propertyDeclaration = CreatePropertyDeclaration("IMyInterface", "Prop", typeName);

            // Act
            var result = (PropertyDeclarationSyntax)_transformer.Transform(propertyDeclaration);

            // Assert
            result.DescendantNodes().OfType<TypeArgumentListSyntax>()
                .Should().OnlyContain(x => x.Arguments.Single().ToString() == typeName);
        }

        private PropertyDeclarationSyntax CreatePropertyDeclaration(string interfaceName, string propertyName, string returnType)
        {
            var getAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration);
            var setAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration);

            var propertyDeclaration = CreatePropertyDeclaration(propertyName, returnType)
                .AddAccessorListAccessors(getAccessor, setAccessor);

            return CreateEnclosingClass(interfaceName, propertyDeclaration);
        }

        private PropertyDeclarationSyntax CreatePropertyDeclarationReadonly(string interfaceName, string propertyName, string returnType)
        {
            var getAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration);
            var propertyDeclaration = CreatePropertyDeclaration(propertyName, returnType)
                .AddAccessorListAccessors(getAccessor);

            return CreateEnclosingClass(interfaceName, propertyDeclaration);
        }

        private TypeSyntax CreateTypeSyntax(string typeName)
        {
            return SyntaxFactory.IdentifierName(typeName);
        }

        private PropertyDeclarationSyntax CreatePropertyDeclaration(string propertyName, string returnType)
        {
            var type = CreateTypeSyntax(returnType);

            return SyntaxFactory.PropertyDeclaration(type, propertyName);
        }

        private static PropertyDeclarationSyntax CreateEnclosingClass(
            string interfaceName,
            PropertyDeclarationSyntax propertyDeclaration)
        {
            var baseType = SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(interfaceName));

            var classDeclaration =
                SyntaxFactory.ClassDeclaration("SomeClass").AddBaseListTypes(baseType, baseType).AddMembers(propertyDeclaration);

            return classDeclaration.DescendantNodes().OfType<PropertyDeclarationSyntax>().First();
        }
    }
}