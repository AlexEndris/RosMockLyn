namespace RosMockLyn.Core
{
    using Microsoft.CodeAnalysis;

    public interface IInterfaceMockGenerator
    {
        SyntaxTree GenerateMock(SyntaxTree treeToGenerateMockFrom);
    }
}