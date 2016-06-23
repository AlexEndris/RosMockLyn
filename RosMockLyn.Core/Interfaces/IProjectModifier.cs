namespace RosMockLyn.Core.Interfaces
{
    public interface IProjectModifier
    {
        bool AddFileToProject(string mockFileContents, string generatedFilePath, string projectPath);
    }
}