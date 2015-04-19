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

using RosMockLyn.Mocking.Routing.Invocations.Interfaces;

namespace RosMockLyn.Mocking.Routing.Invocations
{
    public class IndexInvocationHandler : IHandleIndexInvocation
    {
        private readonly IList<IndexerInvocationInfo> _invocations;

        public IndexInvocationHandler()
        {
            _invocations = new List<IndexerInvocationInfo>();
        }

        public IndexerInvocationInfo Setup<TIndex, TReturn>(TIndex index, TReturn value)
        {
            return GetMatchOrCreate(index, value);
        }

        public IndexerInvocationInfo Setup<TReturn>(object index)
        {
            return GetMatchOrCreate<TReturn>(index);
        }

        public TReturn Handle<TIndex, TReturn>(TIndex index)
        {
            var info = GetMatchOrCreate<TIndex, TReturn>(index);

            return (TReturn)(info.ReturnValue ?? default(TReturn));
        }

        private IndexerInvocationInfo GetMatchOrCreate<TIndex, TReturn>(TIndex index, TReturn value = default(TReturn))
        {
            return GetMatchOrDefault<TIndex, TReturn>(index) ?? Create(index, value);
        }

        private IndexerInvocationInfo GetMatchOrCreate<TReturn>(object index)
        {
            return GetMatchOrDefault<TReturn>(index) ?? Create<TReturn>(index);
        }

        private IndexerInvocationInfo GetMatchOrDefault<TIndex, TReturn>(TIndex index)
        {
            return _invocations.Where(x => x.IndexType == typeof(TIndex) && x.ReturnType == typeof(TReturn))
                    .FirstOrDefault(x => Equals((TIndex)x.Index, index));
        }

        private IndexerInvocationInfo GetMatchOrDefault<TReturn>(object index)
        {
            return _invocations.Where(x => x.IndexType == index.GetType() && x.ReturnType == typeof(TReturn))
                    .FirstOrDefault(x => Equals(x.Index, index));
        }

        private IndexerInvocationInfo Create<TIndex, TReturn>(TIndex index, TReturn value)
        {
            var invocation = new IndexerInvocationInfo(typeof(TReturn), value, typeof(TIndex), index);

            _invocations.Add(invocation);

            return invocation;
        }

        private IndexerInvocationInfo Create<TReturn>(object index)
        {
            var invocation = new IndexerInvocationInfo(typeof(TReturn), default(TReturn), index.GetType(), index);

            _invocations.Add(invocation);

            return invocation;
        }
    }
}