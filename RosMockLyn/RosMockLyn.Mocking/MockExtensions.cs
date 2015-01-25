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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using RosMockLyn.Mocking.Assertion;
using RosMockLyn.Mocking.Matching;
using RosMockLyn.Mocking.Routing;

namespace RosMockLyn.Mocking
{
    public static class MockExtensions
    {
        public static ISetup<TMock> Setup<TMock>(this TMock mock, Expression<Action<TMock>> expression)
        {
            var realMock = TryGetMock(mock);

            var methodCallExpression = (MethodCallExpression)expression.Body;

            var arguments = CreateMatchersFromArguments(methodCallExpression.Arguments);

            var methodInvocationInfo = realMock.SubstitutionContext.SetupMethod(methodCallExpression.Method.Name, arguments);

            return new MethodCall<TMock>(methodInvocationInfo);
        }

        public static ISetup<TMock, TReturn> Setup<TMock, TReturn>(this TMock mock, Expression<Func<TMock, TReturn>> expression)
        {
            var realMock = TryGetMock(mock);

            if (expression.Body.ToString().Contains("get_Item")) // HACK: Indexer appears as a MethodCallExpression in a lambda.
                return SetupIndex<TMock, TReturn>(realMock, (MethodCallExpression)expression.Body);

            return Setup<TMock, TReturn>(realMock, (dynamic)expression.Body);
        }
        
        public static IReceived Received<T>(this T mock, Expression<Action<T>> expression)
        {
            return Received(mock, (MethodCallExpression)expression.Body);
        }

        public static IReceived Received<T, TReturn>(this T mock, Expression<Func<T, TReturn>> expression)
        {
            return Received(mock, (MethodCallExpression)expression.Body);
        }

        private static ISetup<TMock, TReturn> Setup<TMock, TReturn>(IMock mock, MemberExpression expression)
        {
            var propertyInvocationInfo = mock.SubstitutionContext.SetProperty<TReturn>(expression.Member.Name);

            return new PropertyCallReturn<TMock, TReturn>(propertyInvocationInfo);
        }

        private static ISetup<TMock, TReturn> SetupIndex<TMock, TReturn>(IMock mock, MethodCallExpression expression)
        {
            var index = GetArguments(expression.Arguments).OfType<object>().First();

            var indexerInvocationInfo = mock.SubstitutionContext.SetIndex<TReturn>(index);

            return new IndexerCallReturn<TMock, TReturn>(indexerInvocationInfo);
        }

        private static ISetup<TMock, TReturn> Setup<TMock, TReturn>(IMock mock, MethodCallExpression expression)
        {
            var methodCallExpression = expression;

            var arguments = CreateMatchersFromArguments(methodCallExpression.Arguments);

            var setupInfo = mock.SubstitutionContext.SetupMethod<TReturn>(methodCallExpression.Method.Name, arguments);

            return new MethodCallReturn<TMock, TReturn>(setupInfo);
        }

        private static IReceived Received<T>(T mock, MethodCallExpression expression)
        {
            var realMock = TryGetMock(mock);

            var arguments = CreateMatchersFromArguments(expression.Arguments);

            var setupInfo = realMock.SubstitutionContext.GetMatchingInvocations(expression.Method.Name, arguments);

            return new Received(expression.Method.Name, setupInfo);
        }

        private static IMock TryGetMock<T>(T mock)
        {
            var realMock = mock as IMock;
            if (realMock == null)
            {
                throw new InvalidOperationException("mock is no mock"); // TODO: Better Exception!
            }
            return realMock;
        }

        private static IEnumerable<IMatcher> CreateMatchersFromArguments(IEnumerable<Expression> arguments)
        {
            return arguments.Select(MatcherFactory.Create).ToList();
        }

        // TODO: refactor so this method isn't needed anymore!!!
        private static IEnumerable GetArguments(IEnumerable<Expression> arguments)
        {
            foreach (var argument in arguments)
            {
                switch (argument.NodeType)
                {
                    case ExpressionType.Constant:
                        yield return ((ConstantExpression)argument).Value;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

    }
}