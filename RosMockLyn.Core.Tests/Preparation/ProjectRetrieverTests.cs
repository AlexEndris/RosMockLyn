// Copyright (c) 2015, Alexander Endris
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

using System;
using System.IO;
using System.Reflection;

using FluentAssertions;

using Microsoft.CodeAnalysis;

using NUnit.Framework;

using RosMockLyn.Core.Preparation;

namespace RosMockLyn.Core.Tests.Preparation
{
    [TestFixture]
    public class ProjectRetrieverTests
    {
        private ProjectRetriever _retriever;

        [SetUp]
        public void SetUp()
        {
            _retriever = new ProjectRetriever();
        }

        [Test, Category("Local")]
        public void OpenProject_ShouldOpenProject()
        {
            // Arrange
            var projectPath = GetProjectPath();

            // Act
            var result = _retriever.OpenProject(projectPath);

            // Assert
            result.Should().NotBeNull();
        }

        [Test, Category("Local")]
        public void GetReferencedProjects_ShouldGetReferencedProjects()
        {
            // Arrange
            var projectName = "RosMockLyn.Core.Tests";
            var project = CreateProjectWithReferencedProject(projectName);

            // Act
            var result = _retriever.GetReferencedProjects(project);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().Contain(x => x.Name == projectName);
        }

        private static string GetProjectPath()
        {
            var fileName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location) + ".csproj";
            var projectPath = Path.Combine(Environment.CurrentDirectory, "..", "..", "RosMockLyn.Core.Tests", fileName);
            return projectPath;
        }

        private Project CreateProjectWithReferencedProject(string projectName)
        {
            var workspace = new AdhocWorkspace();

            var id = ProjectId.CreateNewId(GetProjectPath());
            ProjectInfo info = ProjectInfo.Create(id, VersionStamp.Default, projectName, "blah", "C#");
            workspace = (AdhocWorkspace)workspace.AddProject(info).Solution.Workspace;

            var reference = new ProjectReference(id);

            return workspace.AddProject("Project", "C#")
                .AddProjectReference(reference);
        }
    }
}