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
using System;

namespace RosMockLyn.Mocking
{
    public interface ISetup
    {
        /// <summary>
        /// Makes the method throw the specified exception when called.
        /// </summary>
        /// <typeparam name="T">The exception that should be thrown when called.</typeparam>
        void Throws<T>() where T : Exception;
    }

    public interface ISetup<TMock> : ISetup
    {
        /// <summary>
        /// Makes the method execute the specified action when called.
        /// </summary>
        /// <param name="action">The action that should be executed when called.</param>
        void WhenCalled(Action action);
    }

    public interface ISetup<TMock, in TReturn> : ISetup
    {
        /// <summary>
        /// Makes the method return the specifies value.
        /// </summary>
        /// <param name="value">The value that should be returned.</param>
        /// <returns>The setup object.</returns>
        ISetup<TMock, TReturn> Returns(TReturn value);

        /// <summary>
        /// Makes the method execute the specified action when called.
        /// </summary>
        /// <param name="action">The action that should be executed when called.</param>
        /// <returns>The setup object.</returns>
        ISetup<TMock, TReturn> WhenCalled(Action action);
    }
}