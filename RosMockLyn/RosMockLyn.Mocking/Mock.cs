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
using System.Reflection;

using RosMockLyn.Mocking.IoC;

namespace RosMockLyn.Mocking
{
    /// <summary>
    /// The main repository for getting mock objects.
    /// </summary>
    public static class Mock
    {
        private static readonly IInjector Injector;

        static Mock()
        {
            Injector = new MockInjector();
            Injector.ScanAssemblies()
                    .Wait();
        }

        /// <summary>
        /// Creates a mock instance for the provided type.
        /// </summary>
        /// <typeparam name="T">The interface of which the mock should be creates.</typeparam>
        /// <exception cref="InvalidOperationException">If the provided type is not an interface.</exception>
        /// <returns>The created mock if registered; otherwise null</returns>
        public static T For<T>() where T : class
        {
            if (!typeof(T).GetTypeInfo().IsInterface)
                throw new InvalidOperationException("The provided type must be an interface.");

            return Injector.Resolve<T>();
        }
    }
}