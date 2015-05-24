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
using RosMockLyn.Mocking.Routing.Invocations;
using RosMockLyn.Mocking.Routing.Invocations.Interfaces;

namespace RosMockLyn.Mocking.Tests.Mocks
{
    public class HandlePropertyInvocationMock : IHandlePropertyInvocation
    {
        private PropertyInvocationInfo _invocationInfo;

        private object _returnValue;
        
        public bool SetupWithReturn_WasCalled { get; private set; }
        
        public bool Setup_WasCalled { get; private set; }
        
        public bool Handle_WasCalled { get; private set; }

        public PropertyInvocationInfo Setup<TReturn>(TReturn value, string propertyName)
        {
            SetupWithReturn_WasCalled = true;

            return _invocationInfo;
        }

        public TReturn Handle<TReturn>(string propertyName)
        {
            Handle_WasCalled = true;

            return (TReturn)(_returnValue ?? default(TReturn));
        }

        public PropertyInvocationInfo Setup<TReturn>(string propertyName)
        {
            Setup_WasCalled = true;

            return _invocationInfo;
        }

        public void SetMethodInvocationReturnValue(PropertyInvocationInfo returnValue)
        {
            _invocationInfo = returnValue;
        }

        public void SetHandleReturnValue<TReturn>(TReturn returnValue)
        {
            _returnValue = returnValue;
        }
    }
}