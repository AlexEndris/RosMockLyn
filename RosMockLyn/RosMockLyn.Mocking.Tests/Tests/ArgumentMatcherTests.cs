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

using NUnit.Framework;

using RosMockLyn.Mocking.Matching;

namespace RosMockLyn.Mocking.Tests
{
    [TestFixture]
    public class ArgumentMatcherTests
    {
        #region IsAny

        [Test, Category("Unit Test")]
        public void ArgIsAny_CorrectType_ShouldMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsAny<int>());

            // Act
            var result = matchCondition.Matches(2);

            // Assert
            result.Should().BeTrue();
        }

        [Test, Category("Unit Test")]
        public void ArgIsAny_WrongType_ShouldNotMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsAny<int>());

            // Act
            var result = matchCondition.Matches("1");

            // Assert
            result.Should().BeFalse();
        }

        [Test, Category("Unit Test")]
        public void ArgIsAny_Null_ShouldMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsAny<string>());

            // Act
            var result = matchCondition.Matches(null);

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region Is

        [Test, Category("Unit Test")]
        public void ArgIs_FuncReturnsTrue_ShouldMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.Is<int>(x => true));

            // Act
            var result = matchCondition.Matches(1);

            // Assert
            result.Should().BeTrue();
        }

        [Test, Category("Unit Test")]
        public void ArgIs_FuncReturnsFalse_ShouldNotMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.Is<int>(x => false));

            // Act
            var result = matchCondition.Matches(1);

            // Assert
            result.Should().BeFalse();
        }

        [Test, Category("Unit Test")]
        public void ArgIs_WrongType_ShouldNotMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.Is<int>(x => true));

            // Act
            var result = matchCondition.Matches("1");

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region IsNull

        [Test, Category("Unit Test")]
        public void ArgIsNull_NotNull_ShouldNotMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsNull<string>());

            // Act
            var result = matchCondition.Matches("1");

            // Assert
            result.Should().BeFalse();
        }

        [Test, Category("Unit Test")]
        public void ArgIsNull_Null_ShouldMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsNull<string>());

            // Act
            var result = matchCondition.Matches(null);

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region IsNotNull

        [Test, Category("Unit Test")]
        public void ArgIsNotNull_NotNull_ShouldMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsNotNull<string>());

            // Act
            var result = matchCondition.Matches("1");

            // Assert
            result.Should().BeTrue();
        }

        [Test, Category("Unit Test")]
        public void ArgIsNotNull_Null_ShouldNotMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsNotNull<string>());

            // Act
            var result = matchCondition.Matches(null);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region IsIn

        [Test, Category("Unit Test")]
        public void ArgIsIn_ArgumentIsIn_ShouldMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsIn<int>(1));

            // Act
            var result = matchCondition.Matches(1);

            // Assert
            result.Should().BeTrue();
        }

        [Test, Category("Unit Test")]
        public void ArgIsIn_ArgumentIsNotIn_ShouldNotMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsIn<int>(2));

            // Act
            var result = matchCondition.Matches(1);

            // Assert
            result.Should().BeFalse();
        }

        [Test, Category("Unit Test")]
        public void ArgIsIn_NullIsIn_ShouldMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsIn<string>(new[] { (string)null }));

            // Act
            var result = matchCondition.Matches(null);

            // Assert
            result.Should().BeTrue();
        }

        [Test, Category("Unit Test")]
        public void ArgIsIn_NullIsNotIn_ShouldNotMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsIn<string>("1"));

            // Act
            var result = matchCondition.Matches(null);

            // Assert
            result.Should().BeFalse();
        }

        [Test, Category("Unit Test")]
        public void ArgIsIn_NothingIsIn_ShouldNotMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsIn<string>());

            // Act
            var result = matchCondition.Matches("1");

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region IsNotIn

        [Test, Category("Unit Test")]
        public void ArgIsNotIn_ArgumentIsIn_ShouldNotMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsNotIn<int>(1));

            // Act
            var result = matchCondition.Matches(1);

            // Assert
            result.Should().BeFalse();
        }

        [Test, Category("Unit Test")]
        public void ArgIsNotIn_ArgumentIsNotIn_ShouldMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsNotIn<int>(2));

            // Act
            var result = matchCondition.Matches(1);

            // Assert
            result.Should().BeTrue();
        }

        [Test, Category("Unit Test")]
        public void ArgIsNotIn_NullIsIn_ShouldNotMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsNotIn<string>(new[] { (string)null }));

            // Act
            var result = matchCondition.Matches(null);

            // Assert
            result.Should().BeFalse();
        }

        [Test, Category("Unit Test")]
        public void ArgIsNotIn_NullIsNotIn_ShouldMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsNotIn<string>("1"));

            // Act
            var result = matchCondition.Matches(null);

            // Assert
            result.Should().BeTrue();
        }

        [Test, Category("Unit Test")]
        public void ArgIsNotIn_NothingIsIn_ShouldMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsNotIn<string>());

            // Act
            var result = matchCondition.Matches(null);

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region IsInRange

        [Test, Category("Unit Test")]
        public void ArgIsInRange_IsInRange_ShouldMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsInRange<int>(1, 5, Range.Exclusive));

            // Act
            var result = matchCondition.Matches(2);

            // Assert
            result.Should().BeTrue();
        }

        [Test, Category("Unit Test")]
        public void ArgIsInRange_IsNotInRange_ShouldMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsInRange<int>(1, 5, Range.Exclusive));

            // Act
            var result = matchCondition.Matches(6);

            // Assert
            result.Should().BeFalse();
        }

        [Test, Category("Unit Test")]
        public void ArgIsInRange_IsInRangeInclusive_ShouldMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsInRange<int>(1, 5, Range.Inclusive));

            // Act
            var result = matchCondition.Matches(5);

            // Assert
            result.Should().BeTrue();
        }

        [Test, Category("Unit Test")]
        public void ArgIsInRange_IsNotInRangeExclusive_ShouldMatch()
        {
            // Arrange
            var matchCondition = GetMatcher(Arg.IsInRange<int>(1, 5, Range.Exclusive));

            // Act
            var result = matchCondition.Matches(5);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        private MatchCondition GetMatcher(object obj)
        {
            return MatchCondition.LastCreatedCondition;
        }

    }
}