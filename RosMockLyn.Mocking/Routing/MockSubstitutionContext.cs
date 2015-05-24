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
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using RosMockLyn.Mocking.Matching;
using RosMockLyn.Mocking.Routing.Invocations;
using RosMockLyn.Mocking.Routing.Invocations.Interfaces;

namespace RosMockLyn.Mocking.Routing
{
    internal sealed class MockSubstitutionContext : ISubstitutionContext
    {
        private readonly IHandleMethodInvocation _methodInvocationHandler;
        private readonly IHandlePropertyInvocation _propertyInvocationHandler;
        private readonly IHandleIndexInvocation _indexInvocationHandler;

        public MockSubstitutionContext(IHandleMethodInvocation methodInvocationHandler,
                                       IHandlePropertyInvocation propertyInvocationHandler,
                                       IHandleIndexInvocation indexInvocationHandler)
        {
            if (methodInvocationHandler == null)
                throw new ArgumentNullException("methodInvocationHandler");
            if (propertyInvocationHandler == null)
                throw new ArgumentNullException("propertyInvocationHandler");
            if (indexInvocationHandler == null)
                throw new ArgumentNullException("indexInvocationHandler");

            _methodInvocationHandler = methodInvocationHandler;
            _propertyInvocationHandler = propertyInvocationHandler;
            _indexInvocationHandler = indexInvocationHandler;
        }

        public IEnumerable<MethodInvocationInfo> GetMatchingInvocations(string methodName, IEnumerable<IMatcher> arguments)
        {
            return _methodInvocationHandler.GetMatches(methodName, arguments);
        }

        public MethodSetupInfo SetupMethod(string methodName, IEnumerable<IMatcher> arguments)
        {
            return _methodInvocationHandler.Setup(methodName, arguments);
        }

        public MethodSetupInfo SetupMethod<TReturn>(string methodName, IEnumerable<IMatcher> arguments)
        {
            return _methodInvocationHandler.Setup<TReturn>(methodName, arguments);
        }
        
        public void SetProperty<TValue>(TValue value, [CallerMemberName]string propertyName = "")
        {
            _propertyInvocationHandler.Setup(value, propertyName);
        }

        public PropertyInvocationInfo SetProperty<TValue>([CallerMemberName]string propertyName = "")
        {
            return _propertyInvocationHandler.Setup<TValue>(propertyName);
        }

        public void SetIndex<TIndex, TValue>(TIndex index, TValue value)
        {
            _indexInvocationHandler.Setup(index, value);
        }

        public IndexerInvocationInfo SetIndex<TValue>(object index)
        {
            return _indexInvocationHandler.Setup<TValue>(index);
        }

        public TReturn GetProperty<TReturn>([CallerMemberName]string propertyName = "")
        {
            return _propertyInvocationHandler.Handle<TReturn>(propertyName);
        }

        public TReturn GetIndex<TReturn, TIndex>(TIndex index)
        {
            return _indexInvocationHandler.Handle<TIndex, TReturn>(index);
        }

        public void Method([CallerMemberName] string methodName = "", params object[] arguments)
        {
            _methodInvocationHandler.Handle(methodName, arguments);
        }

        public TReturn Method<TReturn>([CallerMemberName] string methodName = "", params object[] arguments)
        {
            return _methodInvocationHandler.Handle<TReturn>(methodName, arguments);
        }
    }
}