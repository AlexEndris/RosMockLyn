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

using FluentAssertions;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

using RosMockLyn.Core.Helpers;

namespace RosMockLyn.Core.Tests.Helpers
{
    [TestFixture]
    public class NameHelperTests
    {
        #region GetBaseInterfaceIdentifier

        [Test, Category("Unit Test")]
        public void GetBaseInterfaceIdentifier_ShouldReturnBaseTypeIdentifier_WhenSelf()
        {
            // Arrange
            string expected = "IAmInterface";

            var firstType = SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("SomeBase"));
            var secondType = SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(expected));

            var classDeclaration =
                SyntaxFactory.ClassDeclaration("MyClass")
                    .WithBaseList(
                        SyntaxFactory.BaseList(
                            SyntaxFactory.SeparatedList<BaseTypeSyntax>(new[] { firstType, secondType })));

            // Act
            var actual = NameHelper.GetBaseInterfaceIdentifier(classDeclaration);

            // Assert
            actual.ToString().Should().Be(expected);
        }

        [Test, Category("Unit Test")]
        public void GetBaseInterfaceIdentifier_ShouldReturnBaseTypeIdentifier_WhenAncestor()
        {
            // Arrange
            string expected = "IAmInterface";

            var firstType = SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("SomeBase"));
            var secondType = SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(expected));

            var methodDeclaration =
                SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                    "Method");

            var classDeclaration =
                SyntaxFactory.ClassDeclaration("MyClass")
                    .WithBaseList(SyntaxFactory.BaseList(
                            SyntaxFactory.SeparatedList<BaseTypeSyntax>(new[] { firstType, secondType })))
                    .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(methodDeclaration));

            // Act
            var actual = NameHelper.GetBaseInterfaceIdentifier(classDeclaration.Members.First());

            // Assert
            actual.ToString().Should().Be(expected);
        }

        [Test, Category("Unit Test")]
        public void GetBaseInterfaceIdentifier_ShouldThrowException_WhenMultipleClasses()
        {
            // Arrange
            var firstType = SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("SomeBase"));
            var secondType = SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("SomeOtherBase"));

            var firstClass = SyntaxFactory.ClassDeclaration("MyClass")
                    .WithBaseList(SyntaxFactory.BaseList(
                            SyntaxFactory.SeparatedList<BaseTypeSyntax>(new[] { firstType, secondType })));

            var secondClass = SyntaxFactory.ClassDeclaration("MyClass")
                    .WithBaseList(SyntaxFactory.BaseList(
                            SyntaxFactory.SeparatedList<BaseTypeSyntax>(new[] { firstType, secondType })));

            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("Namespace"))
                .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(new[] {firstClass, secondClass}));
            
            // Act
            Action actual = () => NameHelper.GetBaseInterfaceIdentifier(namespaceDeclaration);

            // Assert
            actual.ShouldThrow<InvalidOperationException>();
        }

        #endregion

        #region GetMockImplementationName

        [Test, Category("Unit Test")]
        public void GetMockImplementationName_ShouldReturnMockImplementationName_WhenInterfaceDeclarationSelf()
        {
            // Arrange
            string expected = "DoSomethingMock";
            string interfaceName = "IDoSomething";

            var interfaceDeclaration = SyntaxFactory.InterfaceDeclaration(interfaceName);

            // Act
            string result = NameHelper.GetMockImplementationName(interfaceDeclaration);

            // Assert
            result.Should().Be(expected);
        }

        [Test, Category("Unit Test")]
        public void GetMockImplementationName_ShouldReturnMockImplementationName_WhenInterfaceDeclarationDescendant()
        {
            // Arrange
            string expected = "DoSomethingMock";
            string interfaceName = "IDoSomething";

            var interfaceDeclaration = SyntaxFactory.InterfaceDeclaration(interfaceName);
            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("NameSpace"))
                .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(interfaceDeclaration));

            // Act
            string result = NameHelper.GetMockImplementationName(namespaceDeclaration);

            // Assert
            result.Should().Be(expected);
        }

        [Test, Category("Unit Test")]
        public void GetMockImplementationName_ShouldReturnCustomMockImplementationName_WhenSuffixSpecified()
        {
            // Arrange
            string expected = "DoSomethingImpl";
            string interfaceName = "IDoSomething";

            var interfaceDeclaration = SyntaxFactory.InterfaceDeclaration(interfaceName);
            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("NameSpace"))
                .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(interfaceDeclaration));

            // Act
            string result = NameHelper.GetMockImplementationName(namespaceDeclaration, "Impl");

            // Assert
            result.Should().Be(expected);
        }

        [Test, Category("Unit Test")]
        public void GetMockImplementationName_ShouldThrowException_WhenMultiple()
        {
            // Arrange
            string interfaceName = "IDoSomething";

            var firstInterface = SyntaxFactory.InterfaceDeclaration(interfaceName);
            var secondInterface = SyntaxFactory.InterfaceDeclaration(interfaceName);
            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("NameSpace"))
                .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(new[] { firstInterface, secondInterface }));

            // Act
            Action result = () => NameHelper.GetMockImplementationName(namespaceDeclaration);

            // Assert
            result.ShouldThrow<InvalidOperationException>();
        }

        #endregion

        #region GetInterfaceName

        [Test, Category("Unit Test")]
        public void GetInterfaceName_ShouldReturnInterfaceName_WhenSelf()
        {
            // Arrange
            string expected = "IAmInterface";

            var interfaceDeclaration = SyntaxFactory.InterfaceDeclaration(expected);

            // Act
            string result = NameHelper.GetInterfaceName(interfaceDeclaration);

            // Assert
            result.Should().Be(expected);
        }

        [Test, Category("Unit Test")]
        public void GetInterfaceName_ShouldReturnInterfaceName_WhenDescendant()
        {
            // Arrange
            string expected = "IAmInterface";

            var interfaceDeclaration = SyntaxFactory.InterfaceDeclaration(expected);
            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("NameSpace"))
                .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(interfaceDeclaration));

            // Act
            string result = NameHelper.GetInterfaceName(namespaceDeclaration);

            // Assert
            result.Should().Be(expected);
        }

        [Test, Category("Unit Test")]
        public void GetInterfaceName_ShouldThrowException_WhenMultipleInterfaces()
        {
            // Arrange
            string interfaceName = "IAmInterface";

            var firstInterface = SyntaxFactory.InterfaceDeclaration(interfaceName);
            var secondInterface = SyntaxFactory.InterfaceDeclaration(interfaceName);
            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("NameSpace"))
                .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(new[] { firstInterface, secondInterface }));

            // Act
            Action result = () => NameHelper.GetInterfaceName(namespaceDeclaration);

            // Assert
            result.ShouldThrow<InvalidOperationException>();
        }

        #endregion

        #region GetFullyQualifiedNamespace

        [Test, Category("Unit Test")]
        public void GetFullyQualifiedNamespace_ShouldReturnFullyQualifiedNamespace_WhenSelf()
        {
            // Arrange
            string expected = "This.Is.A.Test";

            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(IdentifierHelper.GetIdentifier(expected));

            // Act
            string result = NameHelper.GetFullyQualifiedNamespace(namespaceDeclaration);

            // Assert
            result.Should().Be(expected);
        }

        [Test, Category("Unit Test")]
        public void GetFullyQualifiedNamespace_ShouldReturnFullyQualifiedNamespace_WhenDescendant()
        {
            // Arrange
            string expected = "This.Is.A.Test";

            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(IdentifierHelper.GetIdentifier(expected));

            var compilation = SyntaxFactory.CompilationUnit()
                .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(namespaceDeclaration));

            // Act
            string result = NameHelper.GetFullyQualifiedNamespace(compilation);

            // Assert
            result.Should().Be(expected);
        }

        [Test, Category("Unit Test")]
        public void GetFullyQualifiedNamespace_ShouldThrowException_WhenMultipleNamespaces()
        {
            // Arrange
            var firstNamespace = SyntaxFactory.NamespaceDeclaration(IdentifierHelper.GetIdentifier("First.Namespace"));
            var secondNamespace = SyntaxFactory.NamespaceDeclaration(IdentifierHelper.GetIdentifier("Second.Namespace"))
                .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(firstNamespace));

            // Act
            Action result = () => NameHelper.GetFullyQualifiedNamespace(secondNamespace);

            // Assert
            result.ShouldThrow<InvalidOperationException>();
        }

        #endregion
    }
}