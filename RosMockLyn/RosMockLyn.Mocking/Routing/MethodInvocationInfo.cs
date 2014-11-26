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

namespace RosMockLyn.Mocking.Routing
{
    public class MethodInvocationInfo
    {
        public MethodInvocationInfo(string methodName, IEnumerable arguments)
            : this(methodName, arguments, null, null)
        {
        }

        public MethodInvocationInfo(string methodName, IEnumerable arguments, Type returnType, object returnValue)
        {
            MethodName = methodName;
            Arguments = arguments;
            ReturnType = returnType;
            ReturnValue = returnValue;
            Calls = 0;
        }

        public string MethodName { get; private set; }
        public int Calls { get; set; }
        public Type ReturnType { get; private set; }
        public object ReturnValue { get; set; }
        public IEnumerable Arguments { get; private set; }
        public Action WhenCalled { get; set; }

        public void Execute()
        {
            Calls++;
            

            if (WhenCalled != null)
                WhenCalled();
        }
    }
}