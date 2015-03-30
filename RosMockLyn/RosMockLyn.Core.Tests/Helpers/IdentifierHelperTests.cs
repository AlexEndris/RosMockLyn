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
