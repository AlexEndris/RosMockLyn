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

using AssemblyWithInterfaces;

using RosMockLyn.Mocking;

namespace GeneratedTestingAssembly
{
    public class MockSomeInterface : MockBase, ISomeInterface
    {
        int ISomeInterface.this[int index]
        {
            get
            {
                return SubstitutionContext.GetIndex<int, int>(index);
            }
        }

        int ISomeInterface.this[string index]
        {
            get
            {
                return SubstitutionContext.GetIndex<int, string>(index);
            }
            set
            {
                SubstitutionContext.SetIndex(index, value);
            }
        }

        public int IntReadonlyProperty
        {
            get
            {
                return SubstitutionContext.GetProperty<int>();
            }
        }

        public int IntNormalProperty
        {
            get
            {
                return SubstitutionContext.GetProperty<int>();
            }
            set
            {
                SubstitutionContext.SetProperty(value);
            }
        }

        public void VoidCall()
        {
            SubstitutionContext.Method();
        }

        public int IntCall()
        {
            return SubstitutionContext.Method<int>();
        }

        public void Parameters(int i, double d, string s)
        {
            SubstitutionContext.Method(arguments: new object[] { i, d, s });
        }

        public int ReturnParameters(int i, double d, string s)
        {
            return SubstitutionContext.Method<int>(arguments: new object[] { i, d, s });
        }
    }
}