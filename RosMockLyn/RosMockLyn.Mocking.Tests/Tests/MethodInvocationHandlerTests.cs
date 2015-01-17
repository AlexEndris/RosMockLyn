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

using FluentAssertions;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using RosMockLyn.Mocking.Routing.Invocations;
using RosMockLyn.Mocking.Tests.Mocks;

namespace RosMockLyn.Mocking.Tests
{
    [TestClass]
    public class MethodInvocationHandlerTests
    {
        private const string MethodName = "method";
        private static readonly object[] Arguments = { 1, 2.0f, "test" };

        private MethodInvocationHandler _invocationHandler;

        private ArgumentMatcherMock _matcherMock;

        [TestInitialize]
        public void Initialize()
        {
            _matcherMock = new ArgumentMatcherMock();
            
            _invocationHandler = new MethodInvocationHandler(_matcherMock);
        }

        [TestMethod, TestCategory("Unit Test")]
        public void Get_MatchingMethodInvocation_ShouldReturnInfo()
        {
            // Arrange
            _matcherMock.SetNoMatch();

            // Act
            _invocationHandler.Handle(MethodName, Arguments);

            // Assert
            _matcherMock.SetMatch();
            var info = _invocationHandler.Get(MethodName, Arguments);
            
            info.Should().NotBeNull();
        }

        [TestMethod, TestCategory("Unit Test")]
        public void Get_NoMatchingMethodInvocation_ShouldReturnNull()
        {
            // Arrange
            _matcherMock.SetNoMatch();

            // Act
            // Assert
            _matcherMock.SetMatch();
            var info = _invocationHandler.Get(MethodName, Arguments);

            info.Should().BeNull();
        }

        [TestMethod, TestCategory("Unit Test")]
        public void Handle_NoMatchingMethodInvocation_ShouldCreateNew()
        {
            // Arrange
            _matcherMock.SetNoMatch();

            // Act
            _invocationHandler.Handle(MethodName, Arguments);

            // Assert
            _matcherMock.SetMatch();
            var info = _invocationHandler.Get(MethodName, Arguments);

            info.Arguments.Should().BeEquivalentTo(Arguments);
            info.MethodName.Should().Be(MethodName);
            info.ReturnType.Should().BeNull();
            info.ReturnValue.Should().BeNull();
        }

        [TestMethod, TestCategory("Unit Test")]
        public void HandleGeneric_NoMatchingMethodInvocation_ShouldCreateNew()
        {
            // Arrange
            _matcherMock.SetNoMatch();

            // Act
            _invocationHandler.Handle<int>(MethodName, Arguments);

            // Assert
            _matcherMock.SetMatch();
            var info = _invocationHandler.Get(MethodName, Arguments);

            info.Arguments.Should().BeEquivalentTo(Arguments);
            info.MethodName.Should().Be(MethodName);
            info.ReturnType.Should().Be(typeof(int));
            info.ReturnValue.Should().Be(default(int));
        }

        [TestMethod, TestCategory("Unit Test")]
        public void Handle_ShouldIncreaseCallCount()
        {
            // Arrange
            _matcherMock.SetNoMatch();

            // Act
            _invocationHandler.Handle(MethodName, Arguments);

            // Assert
            _matcherMock.SetMatch();
            var info = _invocationHandler.Get(MethodName, Arguments);

            info.Calls.Should().Be(1);
        }

        [TestMethod, TestCategory("Unit Test")]
        public void HandleGeneric_ShouldIncreaseCallCount()
        {
            // Arrange
            _matcherMock.SetNoMatch();

            // Act
            _invocationHandler.Handle<int>(MethodName, Arguments);

            // Assert
            _matcherMock.SetMatch();
            var info = _invocationHandler.Get(MethodName, Arguments);

            info.Calls.Should().Be(1);
        }

        [TestMethod, TestCategory("Unit Test")]
        public void Setup_NoMatchingMethodInvocation_ShouldCreateNew()
        {
            // Arrange
            _matcherMock.SetNoMatch();

            // Act
            var before = _invocationHandler.Get(MethodName, Arguments);
            var after = _invocationHandler.Setup(MethodName, Arguments);

            // Assert
            before.Should().BeNull();
            after.Should().NotBeNull();
        }

        [TestMethod, TestCategory("Unit Test")]
        public void SetupGeneric_NoMatchingMethodInvocation_ShouldCreateNew()
        {
            // Arrange
            _matcherMock.SetNoMatch();

            // Act
            var before = _invocationHandler.Get(MethodName, Arguments);
            var after = _invocationHandler.Setup<int>(MethodName, Arguments);

            // Assert
            before.Should().BeNull();
            after.Should().NotBeNull();
        }
    }
}