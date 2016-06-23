using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace RosMockLyn.Core.Interfaces
{
    internal interface ITreeJoiner
    {
        string JoinTrees(IEnumerable<SyntaxTree> trees);
    }
}