// Copyright (c) 2014, Alexander Endris
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
namespace RosMockLyn.Core
{
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    // TODO : Change name!
    public class InterfaceMockGenerator : CSharpSyntaxRewriter, IInterfaceMockGenerator
    {
        private const string AdditionalUsing = "RosMockLyn.Mocking";
        private const string DerivesFrom = "MockBase";
        private const string ClassSuffix = "Mock";
        private const string Namespace = ".RosMockLyn";

        private const string SubstitutionContext = "SubstitutionContext";

        private const string Method = "Method";
        private const string Property = "Property";
        private const string Index = "Index";

        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            var usings = GenerateAdditionalUsings();

            node = node.AddUsings(usings);

            return base.VisitCompilationUnit(node);
        }

        private UsingDirectiveSyntax[] GenerateAdditionalUsings()
        {
            var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(AdditionalUsing));

            return new[] { usingDirective };
        }

        public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            node = node.WithName(SyntaxFactory.IdentifierName(GenerateNamespaceIdentifier(node)));

            return base.VisitNamespaceDeclaration(node);
        }

        private static string GenerateNamespaceIdentifier(NamespaceDeclarationSyntax node)
        {
            return ((IdentifierNameSyntax)node.Name).Identifier.ValueText+Namespace;
        }

        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            // Class base class visit to traverse down the tree
            // I use "VisitClassDeclaration" instead of "Interface..." because
            // we want to replace the interface with a class!
            // "NormalizeWhitespace" adds needed trivia to correctly format the resulting code (can also be done manually)
           return base.VisitClassDeclaration(MockInterface(node).NormalizeWhitespace());
        }

        public override SyntaxNode VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            if (node.CSharpKind() == SyntaxKind.GetAccessorDeclaration)
            {
                // TODO: At this point it could also be an indexer!!!
                node = GeneratePropertyGetter(node);
            }

            return base.VisitAccessorDeclaration(node);
        }

        private AccessorDeclarationSyntax GeneratePropertyGetter(AccessorDeclarationSyntax node)
        {
            var typeSyntax = node.FirstAncestorOrSelf<BasePropertyDeclarationSyntax>().Type;
            SeparatedSyntaxList<TypeSyntax> list = new SeparatedSyntaxList<TypeSyntax>();
            list = list.Add(typeSyntax);

            var accessorDeclarationSyntax = SyntaxFactory.AccessorDeclaration(
                SyntaxKind.GetAccessorDeclaration,
                SyntaxFactory.Block(
                    SyntaxFactory.ReturnStatement(
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.GenericName(
                                string.Format("{0}.Get{1}", SubstitutionContext, Property))
                                .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(list))))));
            return accessorDeclarationSyntax;
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            // Replace the original property and just change the name of the identifier.
            var newPropertyToken = node.WithIdentifier(SyntaxFactory.Identifier(node.Identifier.ValueText + "Impl"));

            // Again, call base method to be able to traverse deeper into the tree, if needed/wanted
            return base.VisitPropertyDeclaration(newPropertyToken);
        }

        private ClassDeclarationSyntax MockInterface(InterfaceDeclarationSyntax interfaceDeclaration)
        {
            return SyntaxFactory.ClassDeclaration(
                SyntaxFactory.Identifier(interfaceDeclaration.Identifier.ValueText.Substring(1) + ClassSuffix))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddBaseListTypes(
                    SyntaxFactory.IdentifierName(DerivesFrom), // MockBase, base type
                    SyntaxFactory.IdentifierName( // Interface that is being implemented
                        SyntaxFactory.Identifier(interfaceDeclaration.Identifier.ValueText)))
                .AddMembers(interfaceDeclaration.Members.ToArray());
        }

        public SyntaxTree GenerateMock(SyntaxTree treeToGenerateMockFrom)
        {
            return SyntaxFactory.SyntaxTree(Visit(treeToGenerateMockFrom.GetRoot()).NormalizeWhitespace());
        }
    }
}