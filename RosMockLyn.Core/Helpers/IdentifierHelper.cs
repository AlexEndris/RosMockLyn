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

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RosMockLyn.Core.Helpers
{
    internal static class IdentifierHelper
    {
        public static NameSyntax GetIdentifier(string fullyQualifiedName)
        {
            if (string.IsNullOrWhiteSpace(fullyQualifiedName))
                throw new ArgumentNullException("fullyQualifiedName");

            var identifiers = CreateIdentifiers(fullyQualifiedName);

            return identifiers.Count() == 1 
                ? identifiers.First() 
                : AggregateIdentifiers(identifiers);
        }

        public static string AppendIdentifier(params string[] parts)
        {
            return string.Join(".", parts);
        }

        private static IEnumerable<IdentifierNameSyntax> CreateIdentifiers(string fullyQualifiedName)
        {
            var identifiers =
                fullyQualifiedName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(SyntaxFactory.IdentifierName);
            return identifiers;
        }

        private static NameSyntax AggregateIdentifiers(IEnumerable<IdentifierNameSyntax> identifiers)
        {
            var seed = identifiers.First();

            return identifiers.Skip(1).Aggregate<IdentifierNameSyntax, NameSyntax, QualifiedNameSyntax>(
                seed,
                SyntaxFactory.QualifiedName,
                syntax => (QualifiedNameSyntax)syntax);
        }
    }
}