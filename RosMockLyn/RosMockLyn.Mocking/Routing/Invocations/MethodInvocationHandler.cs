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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RosMockLyn.Mocking.Matching;
using RosMockLyn.Mocking.Routing.Invocations.Interfaces;

namespace RosMockLyn.Mocking.Routing.Invocations
{
    public class MethodInvocationHandler : IHandleMethodInvocation
    {
        private readonly IArgumentMatcher _matcher;
        private readonly IList<MethodInvocationInfo> _invocations;

        public MethodInvocationHandler()
        {
            _matcher = new ArgumentMatcher();
            _invocations = new List<MethodInvocationInfo>();
        }

        internal MethodInvocationHandler(IArgumentMatcher argumentMatcher)
        {
            _matcher = argumentMatcher;
            _invocations = new List<MethodInvocationInfo>();
        }

        public MethodInvocationInfo Get(string methodName, IEnumerable arguments)
        {
            return GetMatchOrDefault(methodName, arguments);
        }

        public MethodInvocationInfo Setup(string methodName, IEnumerable arguments)
        {
            return GetMatchOrCreate(methodName, arguments);
        }

        public MethodInvocationInfo Setup<TReturn>(string methodName, IEnumerable arguments)
        {
            return GetMatchOrCreate<TReturn>(methodName, arguments);
        }

        public void Handle(string methodName,IEnumerable arguments)
        {
            var invocation = GetMatchOrCreate(methodName, arguments);

            invocation.Execute();
        }

        public TReturn Handle<TReturn>(string methodName, IEnumerable arguments)
        {
            var invocation = GetMatchOrCreate<TReturn>(methodName, arguments);

            invocation.Execute();

            return (TReturn)invocation.ReturnValue;
        }

        private MethodInvocationInfo GetMatchOrCreate(string methodName, IEnumerable arguments)
        {
            return GetMatchOrDefault(methodName, arguments)
                   ?? Create(methodName, arguments);
        }

        private MethodInvocationInfo GetMatchOrCreate<TReturn>(string methodName, IEnumerable arguments)
        {
            return GetMatchOrDefault(methodName, arguments)
                   ?? Create<TReturn>(methodName, arguments);
        }

        private MethodInvocationInfo GetMatchOrDefault(string methodName, IEnumerable arguments)
        {
            return _invocations.FirstOrDefault(x => x.MethodName == methodName
                                                    && _matcher.Match(x.Arguments, arguments));
        }

        private MethodInvocationInfo Create(string methodName, IEnumerable arguments)
        {
            var invocation = new MethodInvocationInfo(methodName, arguments);

            _invocations.Add(invocation);

            return invocation;
        }

        private MethodInvocationInfo Create<TReturn>(string methodName, IEnumerable arguments)
        {
            var invocation = new MethodInvocationInfo(
                methodName,
                typeof(TReturn),
                default(TReturn),
                arguments);

            _invocations.Add(invocation);

            return invocation;
        }

    }
}