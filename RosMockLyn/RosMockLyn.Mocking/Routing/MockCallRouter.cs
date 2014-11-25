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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;

using RosMockLyn.Mocking.Matching;

namespace RosMockLyn.Mocking.Routing
{
    internal sealed class MockCallRouter : ICallRouter
    {
        private readonly IArgumentMatcher _matcher;
        private readonly IList<MethodInvocationInfo> _invocations;

        public MockCallRouter()
        {
            _matcher = new ArgumentMatcher();
            _invocations = new List<MethodInvocationInfo>();
        }

        public MockCallRouter(IArgumentMatcher matcher)
        {
            _matcher = matcher;
            _invocations = new List<MethodInvocationInfo>();
        }

        public MethodInvocationInfo GetMatchingInvocationInfo(string methodName, params object[] arguments)
        {
            return GetMatchOrDefault(methodName, arguments);
        }

        public MethodInvocationInfo Setup(string methodName, params object[] arguments)
        {
            return CreateInvocation(methodName, arguments);
        }

        public MethodInvocationInfo Setup<TReturn>(string methodName, params object[] arguments)
        {
            return CreateInvocation<TReturn>(methodName, arguments);
        }

        public void Route([CallerMemberName] string methodName = "", params object[] arguments)
        {
            var invocation = GetMatchOrDefault(methodName, arguments)
                             ?? CreateInvocation(methodName, arguments);

            invocation.Calls++;
        }

        public TReturn Route<TReturn>([CallerMemberName] string methodName = "", params object[] arguments)
        {
            var invocation = GetMatchOrDefault(methodName, arguments)
                            ?? CreateInvocation<TReturn>(methodName, arguments);

            invocation.Calls++;

            return (TReturn)invocation.ReturnValue;
        }

        private MethodInvocationInfo GetMatchOrDefault(string methodName, IEnumerable arguments)
        {
            return _invocations.FirstOrDefault(x => x.MethodName == methodName 
                                                    && _matcher.Match(x.Arguments, arguments));
        }

        private MethodInvocationInfo CreateInvocation(string methodName, IEnumerable arguments)
        {
            MethodInvocationInfo invocation = new MethodInvocationInfo(methodName,arguments);

            _invocations.Add(invocation);

            return invocation;
        }

        private MethodInvocationInfo CreateInvocation<TReturn>(string methodName, IEnumerable arguments)
        {
            MethodInvocationInfo invocation = new MethodInvocationInfo(
                methodName,
                arguments,
                typeof(TReturn),
                default(TReturn));

            _invocations.Add(invocation);

            return invocation;
        }
    }
}