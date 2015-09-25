using Microsoft.CodeAnalysis;
using RosMockLyn.Core.Generation;

namespace RosMockLyn.Core.Interfaces
{
    public interface IIndexerGenerator
    {
        SyntaxNode Generate(IndexerData indexerData);
    }
}