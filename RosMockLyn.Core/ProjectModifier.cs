using Microsoft.CodeAnalysis;

using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core
{
    internal sealed class ProjectModifier : IProjectModifier
    {
        private readonly IProjectRetriever _projectRetriever;

        public ProjectModifier(IProjectRetriever projectRetriever)
        {
            _projectRetriever = projectRetriever;
        }
        
        public bool AddFileToProject(string mockFileContents, string generatedFilePath, string projectPath)
        {
            var project = _projectRetriever.OpenProject(projectPath);

            return AddFile(project, mockFileContents, generatedFilePath);
        }

        private bool AddFile(Project project, string mockFileContents, string generatedFilePath)
        {
            var originalSolution = project.Solution;

            var newDocument = project.AddDocument("RosMockLyn.Mocks.generated.cs", mockFileContents, null, generatedFilePath);

            return originalSolution.Workspace.TryApplyChanges(newDocument.Project.Solution);
        }
    }
}