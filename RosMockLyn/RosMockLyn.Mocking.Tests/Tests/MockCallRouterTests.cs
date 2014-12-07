// Copyright (c) 2014, Alexander Endris
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

using RosMockLyn.Mocking.Routing;
using RosMockLyn.Mocking.Tests.Mocks;

namespace RosMockLyn.Mocking.Tests
{
    [TestClass]
    public class MockCallRouterTests
    {
        private MockSubstitutionContext substitutionContext;

        private ArgumentMatcherMock _matcherMock;

        [TestInitialize]
        public void Initialize()
        {
            _matcherMock = new ArgumentMatcherMock();

            substitutionContext = new MockSubstitutionContext(_matcherMock);
        }

        [TestMethod]
        public void RoutingWithNoReturnType_MatchingMethodInvocation_ShouldReturnInfo()
        {
            // Arrange
            string methodName = "method";
            object[] arguments = { 1, 2.0f, "test" };

            _matcherMock.SetNoMatch();

            // Act
            substitutionContext.Route(methodName, arguments);

            // Assert
            _matcherMock.SetMatch();
            var info = substitutionContext.GetMatchingInvocationInfo(methodName, arguments);

            info.Should().NotBeNull();
        }

        [TestMethod]
        public void RoutingWithNoReturnType_NoMatchingMethodInvocation_ShouldReturnNull()
        {
            // Arrange
            string methodName = "method";
            object[] arguments = { 1, 2.0f, "test" };

            _matcherMock.SetNoMatch();

            // Act
            // Assert
            _matcherMock.SetMatch();
            var info = substitutionContext.GetMatchingInvocationInfo(methodName, arguments);

            info.Should().BeNull();
        }

        [TestMethod]
        public void RoutingWithReturnType_MatchingMethodInvocation_ShouldReturnInfo()
        {
            // Arrange
            string methodName = "method";
            object[] arguments = { 1, 2.0f, "test" };

            _matcherMock.SetNoMatch();

            // Act
            substitutionContext.Route<int>(methodName, arguments);

            // Assert
            _matcherMock.SetMatch();
            var info = substitutionContext.GetMatchingInvocationInfo(methodName, arguments);

            info.Should().NotBeNull();
        }

        [TestMethod]
        public void RoutingWithNoReturnType_NoMatchingMethodInvocation_ShouldCreateNew()
        {
            // Arrange
            string methodName = "method";
            object[] arguments = { 1, 2.0f, "test" };

            _matcherMock.SetNoMatch();

            // Act
            substitutionContext.Route(methodName, arguments);

            // Assert
            _matcherMock.SetMatch();
            var info = substitutionContext.GetMatchingInvocationInfo(methodName, arguments);

            info.Arguments.Should().BeEquivalentTo(arguments);
            info.MethodName.Should().Be(methodName);
            info.ReturnType.Should().BeNull();
            info.ReturnValue.Should().BeNull();
        }

        [TestMethod]
        public void RoutingWithReturnType_NoMatchingMethodInvocation_ShouldCreateNew()
        {
            // Arrange
            string methodName = "method";
            object[] arguments = { 1, 2.0f, "test" };

            _matcherMock.SetNoMatch();

            // Act
            substitutionContext.Route<int>(methodName, arguments);

            // Assert
            _matcherMock.SetMatch();
            var info = substitutionContext.GetMatchingInvocationInfo(methodName, arguments);

            info.Arguments.Should().BeEquivalentTo(arguments);
            info.MethodName.Should().Be(methodName);
            info.ReturnType.Should().Be(typeof(int));
            info.ReturnValue.Should().Be(default(int));
        }

        [TestMethod]
        public void RoutingWithNoReturnType_ShouldIncreaseCallCount()
        {
            // Arrange
            string methodName = "method";
            object[] arguments = { 1, 2.0f, "test" };

            _matcherMock.SetNoMatch();

            // Act
            substitutionContext.Route(methodName, arguments);

            // Assert
            _matcherMock.SetMatch();
            var info = substitutionContext.GetMatchingInvocationInfo(methodName, arguments);

            info.Calls.Should().Be(1);
        }

        [TestMethod]
        public void RoutingWithReturnType_ShouldIncreaseCallCount()
        {
            // Arrange
            string methodName = "method";
            object[] arguments = { 1, 2.0f, "test" };

            _matcherMock.SetNoMatch();

            // Act
            substitutionContext.Route<int>(methodName, arguments);

            // Assert
            _matcherMock.SetMatch();
            var info = substitutionContext.GetMatchingInvocationInfo(methodName, arguments);

            info.Calls.Should().Be(1);
        }
    }
}