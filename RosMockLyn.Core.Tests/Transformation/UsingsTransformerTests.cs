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
using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

using RosMockLyn.Core.Helpers;
using RosMockLyn.Core.Interfaces;
using RosMockLyn.Core.Transformation;

namespace RosMockLyn.Core.Tests.Transformation
{
    [TestFixture]
    public class UsingsTransformerTests
    {
        private UsingsTransformer _transformer;

        [SetUp]
        public void SetUp()
        {
            _transformer = new UsingsTransformer();
        }

        [Test, Category("Unit Test")]
        public void Type_ShouldReturn_Using()
        {
            // Arrange
            // Act
            // Assert
            _transformer.Type.Should().Be(TransformerType.Using);
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldThrowException_WhenNotCompilationUnitDeclarationProvided()
        {
            // Arrange
            // Act
            Action action = () => _transformer.Transform(SyntaxFactory.IdentifierName("x"));

            // Assert
            action.ShouldThrow<InvalidOperationException>().WithMessage("Provided*CompilationUnit.");
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldReturnCompilationUnit()
        {
            // Arrange
            var compilation = CreateCompilationUnitWithNamespace("MyNamespace");

            // Act
            var result = _transformer.Transform(compilation);

            // Assert
            result.Should().BeOfType<CompilationUnitSyntax>();
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldAddRosMockLynMockingUsing()
        {
            // Arrange
            string expected = "RosMockLyn.Mocking";

            var compilation = CreateCompilationUnitWithNamespace("MyNamespace");

            // Act
            var result = _transformer.Transform(compilation);

            // Assert
            var usings = result.DescendantNodes().OfType<UsingDirectiveSyntax>();

            usings.Should().Contain(x => x.Name.ToString() == expected);
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldAddRCurrentNamespaceAsUsing()
        {
            // Arrange
            string expected = "MyNamespace";

            var compilation = CreateCompilationUnitWithNamespace(expected);

            // Act
            var result = _transformer.Transform(compilation);

            // Assert
            var usings = result.DescendantNodes().OfType<UsingDirectiveSyntax>();

            usings.Should().Contain(x => x.Name.ToString() == expected);
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldAddCurrentNamespaceAsUsing_WhenFullyQualifiedName()
        {
            // Arrange
            string expected = "This.Is.My.Namespace";

            var compilation = CreateCompilationUnitWithNamespace(expected);

            // Act
            var result = _transformer.Transform(compilation);

            // Assert
            var usings = result.DescendantNodes().OfType<UsingDirectiveSyntax>();

            usings.Should().Contain(x => x.Name.ToString() == expected);
        }

        private CompilationUnitSyntax CreateCompilationUnitWithNamespace(string namespaceName)
        {
            return
                SyntaxFactory.CompilationUnit()
                    .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                            SyntaxFactory.NamespaceDeclaration(IdentifierHelper.GetIdentifier(namespaceName))));
        }
    }
}