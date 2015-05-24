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
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using RosMockLyn.Core.Helpers;

namespace RosMockLyn.Core.Tests.Helpers
{
    [TestFixture]
    public class IdentifierHelperTests
    {
        [Test, Category("Unit Test")]
        public void GetIdentifier_ShouldCreateQualifiedNameSyntax()
        {
            // Arrange
            const string QualifiedName = "This.Is.A.Test";

            // Act
            var qualifiedNameSyntax = IdentifierHelper.GetIdentifier(QualifiedName);

            // Assert
            qualifiedNameSyntax.Should().BeOfType<QualifiedNameSyntax>();
            qualifiedNameSyntax.ToString().Should().Be(QualifiedName);
        }

        [Test, Category("Unit Test")]
        public void GetIdentifier_ShouldCreateIdentifierNameSyntax()
        {
            // Arrange
            const string IdentifierName = "Test";

            // Act
            var qualifiedNameSyntax = IdentifierHelper.GetIdentifier(IdentifierName);

            // Assert
            qualifiedNameSyntax.Should().BeOfType<IdentifierNameSyntax>();
            qualifiedNameSyntax.ToString().Should().Be(IdentifierName);
        }

        [Test, Category("Unit Test")]
        public void AppendIdentifier_ShouldJoinPartsByDot()
        {
            // Arrange
            string expected = "This.Is.A.Test";

            // Act
            string actual = IdentifierHelper.AppendIdentifier("This", "Is", "A", "Test");

            // Assert
            actual.Should().Be(expected);
        }
    }
}
