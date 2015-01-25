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

using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using RosMockLyn.Mocking.Routing.Invocations;

namespace RosMockLyn.Mocking.Assertion
{
    public class Received : IReceived
    {
        private readonly string _methodName;
        private readonly IEnumerable<MethodInvocationInfo> _setupInfo;

        internal Received(string methodName, IEnumerable<MethodInvocationInfo> setupInfo)
        {
            _methodName = methodName;
            _setupInfo = setupInfo;
        }

        public void One()
        {
            Excatly(1);
        }

        public void AtLeastOne()
        {
            Assert.AreNotEqual(0, _setupInfo.Count(), string.Format("There were no calls made for method '{0}'.", _methodName));
        }

        public void Excatly(int expectedCalls)
        {
            Assert.AreEqual(
                expectedCalls,
                _setupInfo.Count(),
                string.Format("There were {0} calls to method '{1}' but {2} were expected.",
                    _setupInfo.Count(),
                    _methodName,
                    expectedCalls));
        }

        public void None()
        {
            Excatly(0);
        }
    }
}