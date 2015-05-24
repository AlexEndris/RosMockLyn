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
using System.Linq;

using RosMockLyn.Mocking.Routing.Invocations.Interfaces;

namespace RosMockLyn.Mocking.Routing.Invocations
{
    public class PropertyInvocationHandler : IHandlePropertyInvocation
    {
        private readonly IList<PropertyInvocationInfo> _invocations;

        public PropertyInvocationHandler()
        {
            _invocations = new List<PropertyInvocationInfo>();
        }

        public PropertyInvocationInfo Setup<TReturn>(TReturn value, string propertyName)
        {
            var info = GetMatchOrCreate<TReturn>(propertyName);

            info.ReturnValue = value;

            return info;
        }

        public PropertyInvocationInfo Setup<TReturn>(string propertyName)
        {
            var info = GetMatchOrCreate<TReturn>(propertyName);

            return info;
        }

        public TReturn Handle<TReturn>(string propertyName)
        {
            var invocation = GetMatchOrCreate<TReturn>(propertyName);

            return (TReturn)invocation.ReturnValue;
        }

        private PropertyInvocationInfo GetMatchOrCreate<TReturn>(string propertyName)
        {
            return GetMatchOrDefault(propertyName) ?? Create<TReturn>(propertyName);
        }

        private PropertyInvocationInfo GetMatchOrDefault(string propertyName)
        {
            return _invocations.FirstOrDefault(x => x.PropertyName == propertyName);
        }

        private PropertyInvocationInfo Create<TReturn>(string propertyName)
        {
            var invocation = new PropertyInvocationInfo(
                propertyName,
                typeof(TReturn),
                default(TReturn));

            _invocations.Add(invocation);

            return invocation;
        }
    }
}