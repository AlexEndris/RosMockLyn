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

using RosMockLyn.Mocking.Matching;
using RosMockLyn.Mocking.Routing.Invocations;
using RosMockLyn.Mocking.Routing.Invocations.Interfaces;

namespace RosMockLyn.Mocking.Tests.Mocks
{
    internal class HandleMethodInvocationMock : IHandleMethodInvocation
    {
        private IEnumerable<MethodInvocationInfo> _invocationInfos;
        private MethodSetupInfo _setupInfo;

        private object _returnValue;

        public bool Setup_WasCalled { get; private set; }
        public bool SetupGeneric_WasCalled { get; private set; }
        public bool GetMatches_WasCalled { get; private set; }
        public bool Handle_WasCalled { get; private set; }
        public bool HandleGeneric_WasCalled { get; private set; }

        public void SetGetMatchesReturnValue(IEnumerable<MethodInvocationInfo> returnValue)
        {
            _invocationInfos = returnValue;
        }

        public void SetGetMatchesReturnValue(MethodSetupInfo returnValue)
        {
            _setupInfo = returnValue;
        }

        public void SetHandleReturnValue<TReturn>(TReturn returnValue)
        {
            _returnValue = returnValue;
        }

        public IEnumerable<MethodInvocationInfo> GetMatches(string methodName, IEnumerable<IMatcher> arguments)
        {
            GetMatches_WasCalled = true;
            return _invocationInfos;
        }

        public MethodSetupInfo Setup(string methodName, IEnumerable<IMatcher> arguments)
        {
            Setup_WasCalled = true;
            return _setupInfo;
        }

        public MethodSetupInfo Setup<TReturn>(string methodName, IEnumerable<IMatcher> arguments)
        {
            SetupGeneric_WasCalled = true;
            return _setupInfo;
        }

        public void Handle(string methodName, IEnumerable<object> arguments)
        {
            Handle_WasCalled = true;
        }

        public TReturn Handle<TReturn>(string methodName, IEnumerable<object> arguments)
        {
            HandleGeneric_WasCalled = true;

            return (TReturn)(_returnValue ?? default(TReturn));
        }
    }
}