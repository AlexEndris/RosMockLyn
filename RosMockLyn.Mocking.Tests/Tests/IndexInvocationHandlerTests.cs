﻿// Copyright (c) 2015, Alexander Endris
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
using FluentAssertions;

using NUnit.Framework;

using RosMockLyn.Mocking.Routing.Invocations;

namespace RosMockLyn.Mocking.Tests
{
    [TestFixture]
    public class IndexInvocationHandlerTests
    {
        private const int Index = 1;
        private const int Value = 10;

        private IndexInvocationHandler _indexInvocationHandler;

        [SetUp]
        public void Initialize()
        {
            _indexInvocationHandler = new IndexInvocationHandler();
        }

        [Test, Category("Unit Test")]
        public void Setup_NoMatchingIndexInvocation_ShouldCreateNew()
        {
            // Arrange
            // Act
            var info = _indexInvocationHandler.Setup(Index, Value);

            // Assert
            info.Should().NotBeNull();
        }

        [Test, Category("Unit Test")]
        public void Setup_MatchingIndexInvocation_ShouldReturnExisting()
        {
            // Arrange
            var info = _indexInvocationHandler.Setup(Index, Value);

            // Act
            var info2 = _indexInvocationHandler.Setup(Index, Value);

            // Assert
            info.Should().BeSameAs(info2);
        }

        [Test, Category("Unit Test")]
        public void Handle_NoMatchingIndexInvocation_ShouldReturnDefault()
        {
            // Arrange
            // Act
            var result = _indexInvocationHandler.Handle<int, int>(Index);

            // Assert
            result.Should().Be(default(int));
        }

        [Test, Category("Unit Test")]
        public void Handle_MatchingIndexInvocation_ShouldReturnSetValue()
        {
            // Arrange
            _indexInvocationHandler.Setup(Index, Value);

            // Act
            var result = _indexInvocationHandler.Handle<int, int>(Index);

            // Assert
            result.Should().Be(Value);
        }
    }
}