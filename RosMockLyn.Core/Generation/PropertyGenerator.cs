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

using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RosMockLyn.Core.Helpers;
using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core.Generation
{
    public class PropertyGenerator : IPropertyGenerator
    {
        private const string SubstitutionContext = "SubstitutionContext";

        private const string Setter = "SetProperty";
        private const string Getter = "GetProperty";

        public SyntaxNode Generate(PropertyData property)
        {
            var explicitInterfaceSpecifier = SyntaxFactory.ExplicitInterfaceSpecifier(IdentifierHelper.GetIdentifier(property.InterfaceName));
            var type = IdentifierHelper.GetIdentifier(property.Type);

            return SyntaxFactory.PropertyDeclaration(type, property.Name)
                .WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier)
                .WithAccessorList(GenerateAccessors(type, property.HasSetter));
        }

        private AccessorListSyntax GenerateAccessors(TypeSyntax type, bool hasSetter)
        {
            IList<AccessorDeclarationSyntax> accessors = new List<AccessorDeclarationSyntax>
                                                             {
                                                                 GenerateGetter(type)
                                                             };

            if (hasSetter)
                accessors.Add(GenerateSetter(type));

            return SyntaxFactory.AccessorList(SyntaxFactory.List(accessors));
        }

        private AccessorDeclarationSyntax GenerateGetter(TypeSyntax type)
        {
            var substitutionCall = SyntaxFactory.ReturnStatement(GenerateSubstitutionCall(type, Getter));

            return SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, SyntaxFactory.Block(substitutionCall));
        }

        private AccessorDeclarationSyntax GenerateSetter(TypeSyntax type)
        {
            var substitution = GenerateSubstitutionCall(type, Setter);

            var argument = SyntaxFactory.Argument(SyntaxFactory.IdentifierName("value"));
            substitution = substitution.WithArgumentList(
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { argument })));

            var substitutionCall = SyntaxFactory.ExpressionStatement(substitution);

            return SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration, SyntaxFactory.Block(substitutionCall));
        }

        private static InvocationExpressionSyntax GenerateSubstitutionCall(TypeSyntax type, string accessor)
        {
            var typeList = SyntaxFactory.SingletonSeparatedList(type);

            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(SubstitutionContext),
                    SyntaxFactory.GenericName(accessor).WithTypeArgumentList(SyntaxFactory.TypeArgumentList(typeList))));
        }
    }
}