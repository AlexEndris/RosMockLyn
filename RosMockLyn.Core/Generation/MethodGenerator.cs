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
    public class MethodGenerator : IMethodGenerator
    {
        private const string SubstitutionContext = "SubstitutionContext";
        private const string Method = "Method";
        private const string Arguments = "arguments";

        public SyntaxNode Generate(MethodData methodData)
        {
            return GenerateMethod(methodData.MethodName, methodData.ReturnType)
                .WithExplicitInterfaceSpecifier(GenerateExplicitInterfaceSpecifier(methodData.InterfaceName))
                .WithParameterList(GetParameters(methodData.Parameters))
                .WithBody(GenerateMethodBody(methodData.ReturnType, methodData.Parameters));
        }

        private static ExplicitInterfaceSpecifierSyntax GenerateExplicitInterfaceSpecifier(string interfaceName)
        {
            var baseInterface = IdentifierHelper.GetIdentifier(interfaceName);

            return SyntaxFactory.ExplicitInterfaceSpecifier(baseInterface);
        }

        private ParameterListSyntax GetParameters(IEnumerable<Parameter> parameters)
        {
            if (!parameters.Any())
                return SyntaxFactory.ParameterList();

            return SyntaxFactory.ParameterList(
                    SyntaxFactory.SeparatedList(
                        parameters.Select(SyntaxHelper.CreateParameter)));
        }

        private MethodDeclarationSyntax GenerateMethod(string methodName, string returnType)
        {
            return SyntaxFactory.MethodDeclaration(IdentifierHelper.GetIdentifier(returnType), methodName);
        }

        private BlockSyntax GenerateMethodBody(string returnType, IEnumerable<Parameter> parameters)
        {
            if (returnType == typeof(void).Name)
            {
                return SyntaxFactory.Block(GenerateVoidMethodBody(parameters));
            }

            return SyntaxFactory.Block(GenerateReturnMethodBody(returnType, parameters));
        }

        private StatementSyntax GenerateVoidMethodBody(IEnumerable<Parameter> parameters)
        {
            var substitution = GenerateMethodSubstitution(parameters);

            return SyntaxFactory.ExpressionStatement(substitution);
        }

        private static InvocationExpressionSyntax GenerateMethodSubstitution(IEnumerable<Parameter> parameters)
        {
            var substitution =
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(SubstitutionContext),
                        SyntaxFactory.IdentifierName(Method)));

            substitution = AddMethodArguments(parameters, substitution);

            return substitution;
        }

        private static InvocationExpressionSyntax AddMethodArguments(
             IEnumerable<Parameter> parameters,
            InvocationExpressionSyntax substitution)
        {
            if (!parameters.Any())
                return substitution;

            var arguments = parameters.Select(x => SyntaxFactory.IdentifierName(x.ParameterName));

            var namedArgument = SyntaxFactory.NameColon(SyntaxFactory.IdentifierName(Arguments));
            var newObjectArry = CreateObjectArray(arguments);

            var argumentList =
                SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(newObjectArry).WithNameColon(namedArgument));

            substitution = substitution.WithArgumentList(SyntaxFactory.ArgumentList(argumentList));

            return substitution;
        }

        private static ArrayCreationExpressionSyntax CreateObjectArray(IEnumerable<IdentifierNameSyntax> arguments)
        {
            return SyntaxFactory.ArrayCreationExpression(SyntaxFactory.ArrayType(
                SyntaxFactory.PredefinedType(
                    SyntaxFactory.Token(SyntaxKind.ObjectKeyword)))
                    .WithRankSpecifiers(SyntaxFactory.SingletonList(
                                                            SyntaxFactory.ArrayRankSpecifier(
                                                                SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                                                    SyntaxFactory.OmittedArraySizeExpression())))))
                .WithInitializer(SyntaxFactory.InitializerExpression(
                    SyntaxKind.ArrayInitializerExpression,
                    SyntaxFactory.SeparatedList<ExpressionSyntax>(arguments)));
        }

        private StatementSyntax GenerateReturnMethodBody(string returnType, IEnumerable<Parameter> parameters)
        {
            var substitution = GenerateGenericMethodSubstitution(parameters, returnType);

            return SyntaxFactory.ReturnStatement(substitution);
        }

        private static InvocationExpressionSyntax GenerateGenericMethodSubstitution(IEnumerable<Parameter> parameters, string returnType)
        {
            var typeList = SyntaxFactory.SeparatedList<TypeSyntax>(new[] { SyntaxFactory.IdentifierName(returnType) });

            var substitution = CreateInvocationExpression(typeList);

            substitution = AddMethodArguments(parameters, substitution);

            return substitution;
        }

        private static InvocationExpressionSyntax CreateInvocationExpression(SeparatedSyntaxList<TypeSyntax> typeList)
        {
            var substitution =
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(SubstitutionContext),
                        SyntaxFactory.GenericName(Method).WithTypeArgumentList(SyntaxFactory.TypeArgumentList(typeList))));

            return substitution;
        }
    }
}