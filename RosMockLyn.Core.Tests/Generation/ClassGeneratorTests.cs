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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using RosMockLyn.Core.Generation;

namespace RosMockLyn.Core.Tests.Generation
{
    [TestFixture]
    public class ClassGeneratorTests
    {
        private ClassGenerator _classGenerator;

        [SetUp]
        public void SetUp()
        {
            _classGenerator = new ClassGenerator();
        }

        [Test, Category("Unit Test")]
        public void Generate_ReturnsClassDeclarationSyntax()
        {
            // Arrange
            // Act
            var result = _classGenerator.Generate(new ClassData("ClassName", "IMyInterface"));

            // Assert
            result.Should().BeOfType<ClassDeclarationSyntax>();
        }

        [Test, Category("Unit Test")]
        public void Generate_ClassData_ReturnsClassDeclarationSyntaxWithSpecifiedName()
        {
            // Arrange
            const string className = "MyClass";

            // Act
            var result = _classGenerator.Generate(new ClassData(className, "IMyInterface"));

            // Assert
            result.Should().BeOfType<ClassDeclarationSyntax>()
                .And.Match<ClassDeclarationSyntax>(x => x.Identifier.ToString() == className);
        }

        [Test, Category("Unit Test")]
        public void Generate_ClassData_ReturnsClassDeclarationSyntaxWithSpecifiedInterface()
        {
            // Arrange
            const string interfaceName = "IMyInterface";

            // Act
            var result = _classGenerator.Generate(new ClassData("MyClass", interfaceName));

            // Assert
            result.Should().BeOfType<ClassDeclarationSyntax>()
                .And.Match<ClassDeclarationSyntax>(x => x.BaseList.Types.Any(y => y.Type.ToString().Contains(interfaceName)));
        }

        [Test, Category("Unit Test")]
        public void Generate_ClassData_ReturnsClassDeclarationSyntaxWithMockBase()
        {
            // Arrange
            const string baseClassName = "MockBase";

            // Act
            var result = _classGenerator.Generate(new ClassData("MyClass", "IMyInterface"));

            // Assert
            result.Should().BeOfType<ClassDeclarationSyntax>()
                .And.Match<ClassDeclarationSyntax>(x => x.BaseList.Types.Any(y => y.Type.ToString().Contains(baseClassName)));
        }

        [Test, Category("Unit Test")]
        public void Generate_ClassData_ReturnsClassDeclarationSyntaxWithPublicModifier()
        {
            // Arrange
            // Act
            var result = _classGenerator.Generate(new ClassData("MyClass", "IMyInterface"));

            // Assert
            result.Should().BeOfType<ClassDeclarationSyntax>()
                .And.Match<ClassDeclarationSyntax>(x => x.Modifiers.Any(SyntaxKind.PublicKeyword));
        }

        [Test, Category("Unit Test")]
        public void Generate_ClassData_ReturnsClassDeclarationSyntaxWithSealedModifier()
        {
            // Arrange
            // Act
            var result = _classGenerator.Generate(new ClassData("MyClass", "IMyInterface"));

            // Assert
            result.Should().BeOfType<ClassDeclarationSyntax>()
                .And.Match<ClassDeclarationSyntax>(x => x.Modifiers.Any(SyntaxKind.SealedKeyword));
        }
    }
}