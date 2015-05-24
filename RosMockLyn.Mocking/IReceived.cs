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
namespace RosMockLyn.Mocking
{
    /// <summary>
    /// Asserts that a specific amount of calls have been made to amount.
    /// </summary>
    public interface IReceived
    {
        /// <summary>
        /// Gets the amount of times the method has been called.
        /// </summary>
        int Calls { get; }

        /// <summary>
        /// Asserts that the method has been called exactly once.
        /// </summary>
        void One();

        /// <summary>
        /// Asserts that the method has been called at least once.
        /// </summary>
        void AtLeastOne();
        
        /// <summary>
        /// Asserts that the method has been called exactly the specified amount of times.
        /// </summary>
        /// <param name="expectedCalls">The amount of times the method has been called.</param>
        void Excatly(int expectedCalls);

        /// <summary>
        /// Asserts that the method has been called at least as often as specified.
        /// </summary>
        /// <param name="amountOfCalls">The amount of times the method has to be called at least.</param>
        void AtLeast(int amountOfCalls);

        /// <summary>
        /// Asserts that the method has never been called.
        /// </summary>
        void None();
    }
}