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
using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

using RosMockLyn.Mocking.Routing.Invocations;
using RosMockLyn.Mocking.Tests.Mocks;

namespace RosMockLyn.Mocking.Tests
{
    [TestFixture]
    public class MethodInvocationHandlerTests
    {
        private const string MethodName = "method";
        private static readonly object[] Arguments = { 1 };

        private MethodInvocationHandler _invocationHandler;

        private MethodCallRecorderMock _recorderMock;
        private MatcherMock _matcherMock;

        [SetUp]
        public void Initialize()
        {
            _recorderMock = new MethodCallRecorderMock();
            _matcherMock = new MatcherMock();

            _invocationHandler = new MethodInvocationHandler(_recorderMock);
        }

        [Test, Category("Unit Test")]
        public void GetMatches_MatchingMethodInvocation_ShouldReturnInfos()
        {
            // Arrange
            _recorderMock.SetRecordedInvocationsReturnValue(CreateInvocations());
            _matcherMock.SetMatchReturnValue(true);

            // Act
            var info = _invocationHandler.GetMatches(MethodName, _matcherMock.ToEnumerable());

            // Assert
            info.Should().NotBeEmpty();
        }

        [Test, Category("Unit Test")]
        public void GetMatches_NoMatchingMethodInvocation_ShouldReturnEmpty()
        {
            // Arrange
            _recorderMock.SetRecordedInvocationsReturnValue(CreateInvocations());
            _matcherMock.SetMatchReturnValue(false);

            // Act
            var info = _invocationHandler.GetMatches(MethodName, _matcherMock.ToEnumerable());

            // Assert
            info.Should().BeEmpty();
        }

        [Test, Category("Unit Test")]
        public void Setup_ShouldCreateSetup()
        {
            // Arrange
            // Act
            var methodSetupInfo = _invocationHandler.Setup(MethodName, _matcherMock.ToEnumerable());

            // Assert
            methodSetupInfo.Should().NotBeNull();
        }

        [Test, Category("Unit Test")]
        public void SetupGeneric_ShouldCreateSetup()
        {
            // Arrange
            // Act
            var methodSetupInfo = _invocationHandler.Setup<int>(MethodName, _matcherMock.ToEnumerable());

            // Assert
            methodSetupInfo.Should().NotBeNull();
        }

        [Test, Category("Unit Test")]
        public void Setup_ShouldMakeSetupAvailableForHandle()
        {
            // Arrange
            bool called = false;
            _matcherMock.SetMatchReturnValue(true);

            // Act
            var methodSetupInfo = _invocationHandler.Setup(MethodName, _matcherMock.ToEnumerable());

            // Assert
            methodSetupInfo.WhenCalled = () => called = true;
            _invocationHandler.Handle(MethodName, _matcherMock.ToEnumerable());

            called.Should().BeTrue();
        }

        [Test, Category("Unit Test")]
        public void SetupGeneric_ShouldMakeSetupAvailableForHandle()
        {
           // Arrange
            bool called = false;
            _matcherMock.SetMatchReturnValue(true);

            // Act
            var methodSetupInfo = _invocationHandler.Setup<int>(MethodName, _matcherMock.ToEnumerable());

            // Assert
            methodSetupInfo.WhenCalled = () => called = true;
            _invocationHandler.Handle<int>(MethodName, _matcherMock.ToEnumerable());

            called.Should().BeTrue();
        }

        [Test, Category("Unit Test")]
        public void Handle_ShouldRecord()
        {
            // Arrange
            // Act
            _invocationHandler.Handle(MethodName, Arguments);

            // Assert
            _recorderMock.Record_WasCalled.Should().BeTrue();
        }

        [Test, Category("Unit Test")]
        public void HandleGeneric_ShouldRecord()
        {
            // Arrange
            // Act
            _invocationHandler.Handle<int>(MethodName, Arguments);

            // Assert
            _recorderMock.Record_WasCalled.Should().BeTrue();
        }

        private IEnumerable<MethodInvocationInfo> CreateInvocations()
        {
            return new MethodInvocationInfo(MethodName, Arguments).ToEnumerable();
        }
    }
}