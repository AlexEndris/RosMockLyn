﻿// Copyright (c) 2015, Alexander Endris
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
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RosMockLyn.Core.Helpers;
using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core.Transformation
{
    internal sealed class InterfaceTransformer : ICodeTransformer
    {
        private const string DerivesFrom = "MockBase";

        public TransformerType Type
        {
            get
            {
                return TransformerType.Interface;
            }
        }

        public SyntaxNode Transform(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            var interfaceDeclaration = node as InterfaceDeclarationSyntax;

            if (interfaceDeclaration == null)
                throw new InvalidOperationException("Provided node múst be an InterfaceDeclaration.");

            return MockInterface(interfaceDeclaration);
        }

        private ClassDeclarationSyntax MockInterface(InterfaceDeclarationSyntax interfaceDeclaration)
        {
            var interfaceIdentifier = SyntaxFactory.IdentifierName(interfaceDeclaration.Identifier.ValueText);

            var implementationName =
                NameHelper.GetMockImplementationName(interfaceDeclaration);

            return SyntaxFactory.ClassDeclaration(
                SyntaxFactory.Identifier(implementationName))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddBaseListTypes(
                    SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(DerivesFrom)), // MockBase, base type
                    SyntaxFactory.SimpleBaseType(interfaceIdentifier)) // Interface that is being implemented
                .AddMembers(interfaceDeclaration.Members.ToArray());
        }
    }
}