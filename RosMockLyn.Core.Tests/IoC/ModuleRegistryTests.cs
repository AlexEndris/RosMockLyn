// Copyright (c) 2015, Alexander Endris
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// * Neither the name of RosMockLyn nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using Autofac;

using FluentAssertions;

using NUnit.Framework;

using RosMockLyn.Core.Interfaces;
using RosMockLyn.Core.IoC;

namespace RosMockLyn.Core.Tests.IoC
{
	[TestFixture]
	public class ModuleRegistryTests
	{
		private IContainer _container;

		[SetUp]
		public void SetUp()
		{
			ContainerBuilder builder = new ContainerBuilder();
			builder.RegisterModule<ModuleRegistry>();

			_container = builder.Build();
		}

		[Test, Category("Unit Test")]
		public void MethodGenerator_IsRegistered()
		{
			// Arrange
			// Act
			// Assert
			_container.IsRegistered<IMethodGenerator>().Should().BeTrue();
			_container.Resolve<IMethodGenerator>().Should().NotBeNull();
		}

		[Test, Category("Unit Test")]
		public void MockAssemblyGenerator_IsRegistered()
		{
			// Arrange
			// Act
			// Assert
			_container.IsRegistered<IAssemblyGenerator>().Should().BeTrue();
			_container.Resolve<IAssemblyGenerator>().Should().NotBeNull();
		}

		[Test, Category("Unit Test")]
		public void ProjectRetriever_IsRegistered()
		{
			// Arrange
			// Act
			// Assert
			_container.IsRegistered<IProjectRetriever>().Should().BeTrue();
			_container.Resolve<IProjectRetriever>().Should().NotBeNull();
		}

		[Test, Category("Unit Test")]
		public void InterfaceExtractor_IsRegistered()
		{
			// Arrange
			// Act
			// Assert
			_container.IsRegistered<IInterfaceExtractor>().Should().BeTrue();
			_container.Resolve<IInterfaceExtractor>().Should().NotBeNull();
		}

		[Test, Category("Unit Test")]
		public void MockGenerator_IsRegistered()
		{
			// Arrange
			// Act
			// Assert
			_container.IsRegistered<IMockGenerator>().Should().BeTrue();
			_container.Resolve<IMockGenerator>().Should().NotBeNull();
		}

		[Test, Category("Unit Test")]
		public void MockRegistryGenerator_IsRegistered()
		{
			// Arrange
			// Act
			// Assert
			_container.IsRegistered<IMockRegistryGenerator>().Should().BeTrue();
			_container.Resolve<IMockRegistryGenerator>().Should().NotBeNull();
		}

		[Test, Category("Unit Test")]
		public void AssemblyCompiler_IsRegistered()
		{
			// Arrange
			// Act
			// Assert
			_container.IsRegistered<IAssemblyCompiler>().Should().BeTrue();
			_container.Resolve<IAssemblyCompiler>().Should().NotBeNull();
		}

		[Test, Category("Unit Test")]
		public void AssemblyManipulator_IsRegistered()
		{
			// Arrange
			// Act
			// Assert
			_container.IsRegistered<IAssemblyManipulator>().Should().BeTrue();
			_container.Resolve<IAssemblyManipulator>().Should().NotBeNull();
		}
	}
}