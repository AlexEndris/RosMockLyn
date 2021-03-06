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
using System;
using System.Collections.Generic;

using RosMockLyn.Mocking.Matching;

namespace RosMockLyn.Mocking.Routing.Invocations
{
    public class MethodSetupInfo
    {
        public string MethodName { get; private set; }
     
        public int Calls { get; private set; }
      
        public Type ReturnType { get; private set; }
        
        public object ReturnValue { get; set; }
        
        public Action WhenCalled { get; set; }
        
        public IEnumerable<IMatcher> Arguments { get; private set; }
        
        public Exception ExcpetionToThrow { get; set; }

        public MethodSetupInfo(string methodName, IEnumerable<IMatcher> arguments)
            : this(methodName, null, null, arguments)
        {
        }

        public MethodSetupInfo(string methodName, Type returnType, object returnValue, IEnumerable<IMatcher> arguments)
        {
            MethodName = methodName;
            ReturnType = returnType;
            ReturnValue = returnValue;
            Arguments = arguments;
            Calls = 0;
        }

        public void Execute()
        {
            Calls++;

            if (ExcpetionToThrow != null)
                throw ExcpetionToThrow;

            if (WhenCalled != null)
                WhenCalled();
        }
    }
}