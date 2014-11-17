﻿// Copyright (c) 2014, Alexander Endris
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

using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneratedTestingAssembly
{
    public abstract class Mock : IMock
    {
        public static ISomeInterface For<T>() where T : class
        {
            return new MockSomeInterface();
        }

        public static readonly ICallRecorder recorder = new CallRecorder();

        protected bool asserting;
        private int expectedCalls;

        public void Returns<T>(string calledMember, T value)
        {

            SetReturnValue(calledMember, value);
        }

        private void SetReturnValue(string calledMember, object value)
        {
            FieldInfo fieldInfo = this.GetType()
                .GetField(
                    string.Format("{0}_ReturnValue", calledMember),
                    BindingFlags.Instance | BindingFlags.NonPublic);

            fieldInfo.SetValue(this, value);
        }

        public void Received(int expectedCalls)
        {
            asserting = true;
            this.expectedCalls = expectedCalls;
        }

        public void AssertCalled(int actual)
        {
            Assert.AreEqual(expectedCalls, actual);
            asserting = false;
        }

        protected void Record([CallerMemberName] string caller = "")
        {
            recorder.Record(this, caller);
        }

    }
}