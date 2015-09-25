using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RosMockLyn.Core.Helpers;
using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core
{
    public class MockGeneratorV2
    {
        private readonly IClassGenerator _classGenerator;
        private readonly IMethodGenerator _methodGenerator;
        private readonly IPropertyGenerator _propertyGenerator;
        private readonly IIndexerGenerator _indexerGenerator;

        public MockGeneratorV2(IClassGenerator classGenerator, 
            IMethodGenerator methodGenerator,
            IPropertyGenerator propertyGenerator,
            IIndexerGenerator indexerGenerator)
        {
            _classGenerator = classGenerator;
            _methodGenerator = methodGenerator;
            _propertyGenerator = propertyGenerator;
            _indexerGenerator = indexerGenerator;
        }

        public SyntaxTree Generate(MockGenerationParameters mockGenerationParameters)
        {
            var methods = mockGenerationParameters.MethodDatas.Select(_methodGenerator.Generate);
            var properties = mockGenerationParameters.PropertyDatas.Select(_propertyGenerator.Generate);
            var indexers = mockGenerationParameters.IndexerDatas.Select(_indexerGenerator.Generate);

            var classSyntax = _classGenerator.Generate(mockGenerationParameters.ClassData)
                .WithMembers(SyntaxFactory.List(methods.Union(properties).Union(indexers)));

            var namespaceSyntax = SyntaxFactory.NamespaceDeclaration(IdentifierHelper.GetIdentifier(mockGenerationParameters.NamespaceName))
                .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(classSyntax));

            var compilationUnit = SyntaxFactory.CompilationUnit()
                .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(namespaceSyntax));
            
            return compilationUnit.SyntaxTree;
        }
    }
}