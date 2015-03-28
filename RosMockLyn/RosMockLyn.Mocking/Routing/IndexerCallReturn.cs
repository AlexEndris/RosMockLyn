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

using System;

using RosMockLyn.Mocking.Routing.Invocations;

namespace RosMockLyn.Mocking.Routing
{
    public class IndexerCallReturn<TMock, TReturn> : ISetup<TMock, TReturn>
    {
        private readonly IndexerInvocationInfo _invocationInfo;

        public IndexerCallReturn(IndexerInvocationInfo invocationInfo)
        {
            _invocationInfo = invocationInfo;
        }

        public ISetup<TMock, TReturn> Returns(TReturn value)
        {
            _invocationInfo.ReturnValue = value;

            return this;
        }

        public void Throws<T>() where T : Exception
        {
            throw new NotSupportedException("It is not supported to make an indexer throw an exception.");
        }

        public void WhenCalled(Action action)
        {
            throw new NotSupportedException("It is not supported to execute code when the indexer is called.");
        }
    }
}