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