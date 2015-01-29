﻿// Copyright (c) 2015, Alexander Endris
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
using System.Collections.Generic;
using System.Linq;

using RosMockLyn.Mocking.Matching;

namespace RosMockLyn.Mocking
{
    public static class Arg
    {
        public static TReturn IsAny<TReturn>()
        {
            return MatchCondition.Create<TReturn>(x => x == null || typeof(TReturn).IsAssignableFrom(x.GetType()));
        }

        public static TReturn IsNull<TReturn>()
        {
            return MatchCondition.Create<TReturn>(x => x == null);
        }

        public static TReturn IsNotNull<TReturn>()
        {
            return MatchCondition.Create<TReturn>(x => x != null);
        }

        public static TReturn Is<TReturn>(Predicate<TReturn> predicate)
        {
            return MatchCondition.Create<TReturn>(predicate);
        }

        public static TReturn IsIn<TReturn>(IEnumerable<TReturn> validMatches)
        {
            return MatchCondition.Create<TReturn>(validMatches.Contains);
        }

        public static TReturn IsIn<TReturn>(params TReturn[] validMatches)
        {
            return IsIn(validMatches.AsEnumerable());
        }

        public static TReturn IsNotIn<TReturn>(IEnumerable<TReturn> invalidMatches)
        {
            return MatchCondition.Create<TReturn>(x => !invalidMatches.Contains(x));
        }

        public static TReturn IsNotIn<TReturn>(params TReturn[] invalidMatches)
        {
            return IsNotIn(invalidMatches.AsEnumerable());
        }

        public static TReturn IsInRange<TReturn>(TReturn from, TReturn to, Range range)
            where TReturn : IComparable
        {
            return MatchCondition.Create<TReturn>(
                x =>
                    {
                        if (x == null)
                            return false;

                        if (range == Range.Exclusive)
                            return x.CompareTo(from) > 0 && x.CompareTo(to) < 0;

                        return x.CompareTo(from) >= 0 && x.CompareTo(to) <= 0;
                    });
        }
    }

    public enum Range
    {
        Inclusive,
        Exclusive
    }
}