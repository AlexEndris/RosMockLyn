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

using FluentAssertions;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

using RosMockLyn.Core.Interfaces;
using RosMockLyn.Core.Transformation;

namespace RosMockLyn.Core.Tests.Transformation
{
    [TestFixture]
    public class InterfaceTransformerTests
    {
        private InterfaceTransformer _transformer;

        [SetUp]
        public void SetUp()
        {
            _transformer = new InterfaceTransformer();
        }

        [Test, Category("Unit Test")]
        public void Type_ShouldReturn_Interface()
        {
            // Arrange
            // Act
            // Assert
            _transformer.Type.Should().Be(GeneratorType.Interface);
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldThrowException_WhenNotInterfaceDeclarationSyntax()
        {
            // Arrange
            
            // Act
            Action action = () => _transformer.Transform(SyntaxFactory.IdentifierName("x"));

            // Assert
            action.ShouldThrow<InvalidOperationException>().WithMessage("Provided*InterfaceDeclaration.");
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldReturnClassDeclaration_WithMockName()
        {
            // Arrange
            string expected = "InterfaceMock";

            var interfaceDeclaration = CreateInterfaceDeclaration("IInterface");

            // Act
            var result = _transformer.Transform(interfaceDeclaration);

            // Assert
            result.Should()
                .BeOfType<ClassDeclarationSyntax>()
                .Which.Should()
                .Match<ClassDeclarationSyntax>(x => x.Identifier.ToString() == expected);
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldReturnClassDeclaration_WithBaseType_MockBase()
        {
            // Arrange
            string expected = "MockBase";

            var interfaceDeclaration = CreateInterfaceDeclaration("IInterface");

            // Act
            var result = (ClassDeclarationSyntax)_transformer.Transform(interfaceDeclaration);

            // Assert
            result.BaseList.Types.Should().Contain(x => x.Type.ToString() == expected);
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldReturnClassDeclaration_WithBaseType_InterfaceName()
        {
            // Arrange
            string expected = "IInterface";

            var interfaceDeclaration = CreateInterfaceDeclaration(expected);

            // Act
            var result = (ClassDeclarationSyntax)_transformer.Transform(interfaceDeclaration);

            // Assert
            result.BaseList.Types.Should().Contain(x => x.Type.ToString() == expected);
        }

        [Test, Category("Unit Test")]
        public void Transform_ShouldReturnClassDeclaration_WithMember()
        {
            // Arrange
            string expected = "MyMember";

            var interfaceDeclaration = CreateInterfaceDeclarationWithMember("IInterface", expected);

            // Act
            var result = (ClassDeclarationSyntax)_transformer.Transform(interfaceDeclaration);

            // Assert
            result.Members.Should()
                .Contain(x => ((MethodDeclarationSyntax)x).Identifier.ToString() == expected);
        }

        private InterfaceDeclarationSyntax CreateInterfaceDeclaration(string interfaceName)
        {
            return SyntaxFactory.InterfaceDeclaration(interfaceName);
        }

        private InterfaceDeclarationSyntax CreateInterfaceDeclarationWithMember(string interfaceName, string memberName)
        {
            var member = SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                    SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                        memberName));

            return CreateInterfaceDeclaration(interfaceName)
                    .WithMembers(member);
        }
    }
}