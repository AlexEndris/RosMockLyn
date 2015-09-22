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
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RosMockLyn.Core.Helpers;
using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core.Generation
{
	public sealed class IndexerGenerator : IIndexerGenerator
	{
        private const string SubstitutionContext = "SubstitutionContext";
        private const string Setter = "SetIndex";
        private const string Getter = "GetIndex";

        public SyntaxNode Generate(IndexerData indexerData)
        {
            var explicitInterfaceSpecifier = SyntaxFactory.ExplicitInterfaceSpecifier(IdentifierHelper.GetIdentifier(indexerData.InterfaceName));
            var type = IdentifierHelper.GetIdentifier(indexerData.Type);

            return SyntaxFactory.IndexerDeclaration(type)
                .WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier)
                .WithParameterList(GenerateParameterList(indexerData.Parameters))
                .WithAccessorList(GenerateAccessors(type, indexerData.Parameters, indexerData.HasSetter));
        }

        private BracketedParameterListSyntax GenerateParameterList(IEnumerable<Parameter> parameters)
        {
            return SyntaxFactory.BracketedParameterList(
                    SyntaxFactory.SeparatedList(
                        parameters.Select(SyntaxHelper.CreateParameter)));
        }
        
        private AccessorListSyntax GenerateAccessors(NameSyntax type, IEnumerable<Parameter> parameters, bool hasSetter)
        {
            IList<AccessorDeclarationSyntax> accessors = new List<AccessorDeclarationSyntax>
                                                             {
                                                                 GenerateGetter(type, parameters)
                                                             };

            if (hasSetter)
                accessors.Add(GenerateSetter(type, parameters));

            return SyntaxFactory.AccessorList(SyntaxFactory.List(accessors));
        }

        private AccessorDeclarationSyntax GenerateGetter(TypeSyntax type, IEnumerable<Parameter> parameters)
        {
            var substitutionCall = SyntaxFactory.ReturnStatement(GenerateSubstitutionCall(type, Getter, parameters));

            return SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, SyntaxFactory.Block(substitutionCall));
        }

        private AccessorDeclarationSyntax GenerateSetter(TypeSyntax type, IEnumerable<Parameter> parameters)
        {
            var substitution = GenerateSubstitutionCall(type, Setter, parameters);

            var argument = SyntaxFactory.Argument(SyntaxFactory.IdentifierName("value"));
            substitution = substitution.WithArgumentList(
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { argument })));

            var substitutionCall = SyntaxFactory.ExpressionStatement(substitution);

            return SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration, SyntaxFactory.Block(substitutionCall));
        }

        private static InvocationExpressionSyntax GenerateSubstitutionCall(TypeSyntax type, string accessor, IEnumerable<Parameter> parameters)
        {
            var typeList = SyntaxFactory.SingletonSeparatedList(type);

            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(SubstitutionContext),
                    SyntaxFactory.GenericName(accessor)
                            .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(typeList))))
                        .WithArgumentList(
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList(parameters.Select(GetArgument))));
        }

        private static ArgumentSyntax GetArgument(Parameter parameter)
        {
            return SyntaxFactory.Argument(IdentifierHelper.GetIdentifier(parameter.ParameterName));
        }
    }
}