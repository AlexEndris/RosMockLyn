using Microsoft.CodeAnalysis;
using RosMockLyn.Core.Generation;

namespace RosMockLyn.Core.Interfaces
{
    public interface IPropertyGenerator
    {
        SyntaxNode Generate(PropertyData property);
    }
}