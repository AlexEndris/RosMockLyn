using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core
{
   internal sealed class TreeJoiner : ITreeJoiner
    {
        public string JoinTrees(IEnumerable<SyntaxTree> trees)
        {
            var usings = ExtractUsings(trees);

            trees = RemoveUsings(trees);

            var template = GenerateTemplate(usings);

            var printedTrees = PrintTrees(trees);

            return string.Format(template, string.Join(@"\r\n", printedTrees));
        }

        private IEnumerable<SyntaxTree> RemoveUsings(IEnumerable<SyntaxTree> trees)
        {
            return trees.Select(RemoveUsings);
        }

        private SyntaxTree RemoveUsings(SyntaxTree tree)
        {
            var root = tree.GetCompilationUnitRoot();

            return root.WithUsings(SyntaxFactory.List<UsingDirectiveSyntax>()).SyntaxTree;
        }

        private IEnumerable<string> ExtractUsings(IEnumerable<SyntaxTree> trees)
        {
            return trees.SelectMany(ExtractUsings);
        }

        private IEnumerable<string> ExtractUsings(SyntaxTree tree)
        {
            var root = tree.GetCompilationUnitRoot();

            return root.Usings.Select(x => x.ToString());
        }

        private string GenerateTemplate(IEnumerable<string> usings)
        {
            var joinedUsings = string.Join(@"\r\n", usings);

            return $"{joinedUsings}\r\n\r\nnamespace RosMockLyn.Mocks\r\n{{\r\n{{0}}\r\n}}";
        }

        private IEnumerable<string> PrintTrees(IEnumerable<SyntaxTree> finalTrees)
        {
            return finalTrees.Select(x => x.ToString());
        }
    }
}