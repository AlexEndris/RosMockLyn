using FluentAssertions;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RosMockLyn.Core.Helpers;
using RosMockLyn.Core.Transformation;

namespace RosMockLyn.Core.Tests
{
    [TestClass]
    public class IdentifierHelperTests
    {
        [TestMethod]
        public void ShouldCreateQualifiedNameSyntax()
        {
            // Arrange
            const string QualifiedName = "This.Is.A.Test";

            // Act
            var qualifiedNameSyntax = IdentifierHelper.GetIdentifier(QualifiedName);

            // Assert
            qualifiedNameSyntax.Should().BeOfType<QualifiedNameSyntax>();
            qualifiedNameSyntax.ToString().Should().Be(QualifiedName);
        }

        [TestMethod]
        public void ShouldCreateIdentifierNameSyntax()
        {
            // Arrange
            const string IdentifierName = "Test";

            // Act
            var qualifiedNameSyntax = IdentifierHelper.GetIdentifier(IdentifierName);

            // Assert
            qualifiedNameSyntax.Should().BeOfType<IdentifierNameSyntax>();
            qualifiedNameSyntax.ToString().Should().Be(IdentifierName);
        }
    }
}
