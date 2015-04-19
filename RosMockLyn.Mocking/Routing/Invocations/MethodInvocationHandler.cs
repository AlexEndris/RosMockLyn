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

using System.Collections.Generic;
using System.Linq;

using RosMockLyn.Mocking.Matching;
using RosMockLyn.Mocking.Routing.Invocations.Interfaces;

namespace RosMockLyn.Mocking.Routing.Invocations
{
    internal class MethodInvocationHandler : IHandleMethodInvocation
    {
        private readonly IList<MethodSetupInfo> _invocations;
        private readonly IMethodCallRecorder _recorder;

        public MethodInvocationHandler()
        {
            _invocations = new List<MethodSetupInfo>();
            _recorder = new MethodCallRecorder();
        }

        public MethodInvocationHandler(IMethodCallRecorder methodCallRecorder)
        {
            _invocations = new List<MethodSetupInfo>();
            _recorder = methodCallRecorder;
        }

        public IEnumerable<MethodInvocationInfo> GetMatches(string methodName, IEnumerable<IMatcher> arguments)
        {
            return _recorder.RecordedInvocations.Where(x => x.MethodName == methodName
                                                           && MatchArguments(arguments, x.Arguments));
        }

        public MethodSetupInfo Setup(string methodName, IEnumerable<IMatcher> arguments)
        {
            return Create(methodName, arguments);
        }

        public MethodSetupInfo Setup<TReturn>(string methodName, IEnumerable<IMatcher> arguments)
        {
            return Create<TReturn>(methodName, arguments);
        }

        public void Handle(string methodName, IEnumerable<object> arguments)
        {
            _recorder.Record(methodName, arguments);

            var invocation = GetLastMatch(methodName, arguments);

            if (invocation == null)
                return;

            invocation.Execute();
        }

        public TReturn Handle<TReturn>(string methodName, IEnumerable<object> arguments)
        {
            _recorder.Record(methodName, arguments);

            var invocation = GetLastMatch(methodName, arguments);

            if (invocation == null)
                return default(TReturn);

            invocation.Execute();

            return (TReturn)invocation.ReturnValue;
        }

        private MethodSetupInfo GetLastMatch(string methodName, IEnumerable<object> arguments)
        {
            return _invocations.LastOrDefault(x => x.MethodName == methodName 
                                                    && MatchArguments(x.Arguments, arguments));
        }

        private MethodSetupInfo Create(string methodName, IEnumerable<IMatcher> arguments)
        {
            var invocation = new MethodSetupInfo(methodName, arguments);

            _invocations.Add(invocation);

            return invocation;
        }

        private MethodSetupInfo Create<TReturn>(string methodName, IEnumerable<IMatcher> arguments)
        {
            var invocation = new MethodSetupInfo(
                methodName,
                typeof(TReturn),
                default(TReturn),
                arguments);

            _invocations.Add(invocation);

            return invocation;
        }

        private bool MatchArguments(IEnumerable<IMatcher> matchers, IEnumerable<object> arguments)
        {
            return arguments.All((x, i) => matchers.ElementAt(i).Match(x));
        }
    }
}