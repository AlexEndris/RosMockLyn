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

using RosMockLyn.Mocking.Matching;
using RosMockLyn.Mocking.Routing;
using RosMockLyn.Mocking.Routing.Invocations.Interfaces;
using RosMockLyn.Mocking.Tests.Mocks;

namespace RosMockLyn.Mocking.Tests
{
    [TestClass]
    public class MockSubstitutionContextTests
    {
        private const string MethodName = "method";
        private static readonly object[] Arguments = { 1, 2.0f, "test" };


        private MockSubstitutionContext _substitutionContext;
        private HandleIndexInvocationMock _handleIndexInvocationMock;
        private HandleMethodInvocationMock _handleMethodInvocationMock;
        private HandlePropertyInvocationMock _handlePropertyInvocationMock;
        private IMatcher _matcherMock;

        [TestInitialize]
        public void Initialize()
        {
            _handleIndexInvocationMock = new HandleIndexInvocationMock();
            _handleMethodInvocationMock = new HandleMethodInvocationMock();
            _handlePropertyInvocationMock = new HandlePropertyInvocationMock();
            //_matcherMock = new MatcherMock();

            _substitutionContext = new MockSubstitutionContext(
                _handleMethodInvocationMock,
                _handlePropertyInvocationMock,
                _handleIndexInvocationMock);
        }

        [TestMethod, TestCategory("Unit Test")]
        public void MethodCall_ShouldDelegate()
        {
            // Arrange
            // Act
            _substitutionContext.CallMethod(MethodName, Arguments);

            // Assert
            _handleMethodInvocationMock.Handle_WasCalled.Should().BeTrue();
        }

        [TestMethod, TestCategory("Unit Test")]
        public void MethodCallWithReturnValue_ShouldDelegate()
        {
            // Arrange
            // Act
            _substitutionContext.CallMethod<int>(MethodName, Arguments);

            // Assert
            _handleMethodInvocationMock.HandleGeneric_WasCalled.Should().BeTrue();
        }

        [TestMethod, TestCategory("Unit Test")]
        public void SetupMethod_ShouldDelegate()
        {
            // Arrange
            // Act
            _substitutionContext.SetupMethod(MethodName, _matcherMock.ToEnumerable());

            // Assert
            _handleMethodInvocationMock.Setup_WasCalled.Should().BeTrue();
        }

        [TestMethod, TestCategory("Unit Test")]
        public void SetupMethodWithReturnValue_ShouldDelegate()
        {
            // Arrange
            // Act
            _substitutionContext.SetupMethod<int>(MethodName, _matcherMock.ToEnumerable());

            // Assert
            _handleMethodInvocationMock.SetupGeneric_WasCalled.Should().BeTrue();
        }

        [TestMethod, TestCategory("Unit Test")]
        public void GetInvocationMatch_ShouldDelegate()
        {
            // Arrange
            // Act
            _substitutionContext.GetMatchingInvocations(MethodName, _matcherMock.ToEnumerable());

            // Assert
            _handleMethodInvocationMock.GetMatches_WasCalled.Should().BeTrue();
        }

        [TestMethod, TestCategory("Unit Test")]
        public void SetProperty_ShouldDelegate()
        {
            // Arrange
            // Act
            _substitutionContext.SetProperty(1, MethodName);
            // Assert
            _handlePropertyInvocationMock.SetupWithReturn_WasCalled.Should().BeTrue();
        }

        [TestMethod, TestCategory("Unit Test")]
        public void SetProperty_WithoutReturnValue_ShouldDelegate()
        {
            // Arrange
            // Act
            _substitutionContext.SetProperty<int>(MethodName);
            // Assert
            _handlePropertyInvocationMock.Setup_WasCalled.Should().BeTrue();
        }

        [TestMethod, TestCategory("Unit Test")]
        public void GetProperty_ShouldDelegate()
        {
            // Arrange
            // Act
            _substitutionContext.GetProperty<int>(MethodName);
            // Assert
            _handlePropertyInvocationMock.Handle_WasCalled.Should().BeTrue();
        }

        [TestMethod, TestCategory("Unit Test")]
        public void SetIndex_ShouldDelegate()
        {
            // Arrange
            // Act
            _substitutionContext.SetIndex(1, MethodName);
            // Assert
            _handleIndexInvocationMock.SetupWithReturn_WasCalled.Should().BeTrue();
        }

        [TestMethod, TestCategory("Unit Test")]
        public void SetIndex_WithoutReturnValue_ShouldDelegate()
        {
            // Arrange
            // Act
            _substitutionContext.SetIndex<int>(1);
            // Assert
            _handleIndexInvocationMock.Setup_WasCalled.Should().BeTrue();
        }

        [TestMethod, TestCategory("Unit Test")]
        public void GetIndex_ShouldDelegate()
        {
            // Arrange
            // Act
            _substitutionContext.GetIndex<int,int>(1);
            // Assert
            _handleIndexInvocationMock.Handle_WasCalled.Should().BeTrue();
        }
    }
}