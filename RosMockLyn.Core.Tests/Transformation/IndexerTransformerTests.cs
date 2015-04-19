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
    public class IndexerTransformerTests
    {
        private IndexerTransformer _transformer;

        [SetUp]
        public void SetUp()
        {
            _transformer = new IndexerTransformer();
        }

        [Test, Category("Unit Test")]
        public void Type_ShouldReturn_Indexer()
        {
            // Arrange
            // Act
            // Assert
            _transformer.Type.Should().Be(TransformerType.Indexer);
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldThrowException_WhenNotIndexerDeclarationSyntax()
        {
            // Arrange
            // Act
            Action action = () => _transformer.Transform(SyntaxFactory.IdentifierName("x"));

            // Assert
            action.ShouldThrow<InvalidOperationException>().WithMessage("Provided*IndexerDeclaration.");
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldReturnPropertyDeclaration()
        {
            // Arrange
            var propertyDeclaration = CreateIndexerDeclaration("IInterface", "MyType");

            // Act
            var result = _transformer.Transform(propertyDeclaration);

            // Assert
            result.Should().BeOfType<IndexerDeclarationSyntax>();
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldReturnIndexerDeclaration_WithExplicitInterfaceImplementation()
        {
            // Arrange
            string interfaceName = "IMyInterface";

            var propertyDeclaration = CreateIndexerDeclarationReadonly(interfaceName, "MyType");

            // Act
            var result = (IndexerDeclarationSyntax)_transformer.Transform(propertyDeclaration);

            // Assert
            result.ExplicitInterfaceSpecifier.ToString().Should().StartWith(interfaceName);
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldImplementGetAccessor()
        {
            // Arrange
            string invocation = "GetIndex";

            var propertyDeclaration = CreateIndexerDeclarationReadonly("IMyInterface", "MyType");

            // Act
            var result = (IndexerDeclarationSyntax)_transformer.Transform(propertyDeclaration);

            // Assert
            result.DescendantNodes().OfType<MemberAccessExpressionSyntax>()
                .Should().Contain(x => x.Name.ToString().Contains(invocation));
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldImplementSetAccessor()
        {
            // Arrange
            string invocation = "SetIndex";

            var propertyDeclaration = CreateIndexerDeclaration("IMyInterface", "MyType");

            // Act
            var result = (IndexerDeclarationSyntax)_transformer.Transform(propertyDeclaration);

            // Assert
            result.DescendantNodes().OfType<MemberAccessExpressionSyntax>()
                .Should().Contain(x => x.Name.ToString().Contains(invocation));
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldImplementSetAccessor_WithValueParameter_AndIndex()
        {
            // Arrange
            string i = "i";
            string value = "value";

            var propertyDeclaration = CreateIndexerDeclaration("IMyInterface", "MyType");

            // Act
            var result = (IndexerDeclarationSyntax)_transformer.Transform(propertyDeclaration);

            // Assert
            result.DescendantNodes().OfType<InvocationExpressionSyntax>()
                .Where(x => x.ArgumentList.Arguments.Any())
                .Select(x => x.ArgumentList.Arguments)
                .Should().Contain(x => x.Any(y => y.Expression.ToString() == value))
                .And.Contain(x => x.Any(y => y.Expression.ToString() == i));
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldImplementAccessors_WithTypeParameters()
        {
            // Arrange
            string typeName = "MyType";
            string intType = "int";

            var propertyDeclaration = CreateIndexerDeclaration("IMyInterface", typeName);

            // Act
            var result = (IndexerDeclarationSyntax)_transformer.Transform(propertyDeclaration);

            // Assert
            result.DescendantNodes().OfType<TypeArgumentListSyntax>()
                .Should().Contain(x => x.Arguments.Any(y => y.ToString() == typeName))
                .And.Contain(x => x.Arguments.Any(y => y.ToString() == intType));
        }

        private IndexerDeclarationSyntax CreateIndexerDeclaration(string interfaceName, string returnType)
        {
            var getAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration);
            var setAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration);
            var parameter = CreateParameter();

            var propertyDeclaration = CreateIndexerDeclaration(returnType)
                .AddParameterListParameters(parameter)
                    .AddAccessorListAccessors(getAccessor, setAccessor);

            return CreateEnclosingClass(interfaceName, propertyDeclaration);
        }

        private IndexerDeclarationSyntax CreateIndexerDeclarationReadonly(string interfaceName, string returnType)
        {
            var parameter = CreateParameter();
            var getAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration);
            var propertyDeclaration = CreateIndexerDeclaration(returnType)
                .AddParameterListParameters(parameter)
                .AddAccessorListAccessors(getAccessor);

            return CreateEnclosingClass(interfaceName, propertyDeclaration);
        }

        private static ParameterSyntax CreateParameter()
        {
            return SyntaxFactory.Parameter(SyntaxFactory.Identifier("i"))
                .WithType(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));
        }

        private TypeSyntax CreateTypeSyntax(string typeName)
        {
            return SyntaxFactory.IdentifierName(typeName);
        }

        private IndexerDeclarationSyntax CreateIndexerDeclaration(string returnType)
        {
            var type = CreateTypeSyntax(returnType);

            return SyntaxFactory.IndexerDeclaration(type);
        }

        private static IndexerDeclarationSyntax CreateEnclosingClass(
            string interfaceName,
            IndexerDeclarationSyntax propertyDeclaration)
        {
            var baseType = SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(interfaceName));

            var classDeclaration =
                SyntaxFactory.ClassDeclaration("SomeClass").AddBaseListTypes(baseType, baseType).AddMembers(propertyDeclaration);

            return classDeclaration.DescendantNodes().OfType<IndexerDeclarationSyntax>().First();
        }
    }
}