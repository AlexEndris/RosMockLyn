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
using System.Collections.Generic;
using System.Reflection;

namespace RosMockLyn.Utilities
{
    public static class Extensions
    {
        public static T Received<T>(this T mock, int expectedCalls)
        {
            var realMock = mock as IMock;
            if (realMock == null) throw new InvalidOperationException("mock is no mock");

            realMock.Received(expectedCalls);

            return mock;
        }

        public static void Returns<T>(this T returnType, T value)
        {
            Call lastCall = MockBase.Recorder.GetLastCall();

            if (!CheckReturnType(lastCall.ReturnType, typeof(T)))
            {
                throw new InvalidOperationException();
            }

            lastCall.MockedObject.Returns(lastCall.CalledMember, value);
        }


        // Change Name!!!
        private static bool CheckReturnType(Type expectedReturnType, Type actualType)
        {
            return expectedReturnType == actualType;
        }


        public static bool IsAssignableTo<T>(this Type type)
        {
            return typeof(T).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        public static bool IsAssignableFrom<T>(this Type type)
        {
            return type.GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo());
        }

        public static void Apply<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items) 
                action(item);
        }
    }

}