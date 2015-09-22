using Microsoft.CodeAnalysis;

using RosMockLyn.Core.Generation;

namespace RosMockLyn.Core.Interfaces
{
    public interface IMethodGenerator
    {
        SyntaxNode Generate(MethodData methodData);
    }
}