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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RosMockLyn.Core.Interfaces;

namespace RosMockLyn.Core.Generation
{
    public class MethodGenerator : ICodeGenerator
    {
        private const string SubstitutionContext = "SubstitutionContext";
        private const string Method = "Method";
        private const string Arguments = "arguments";

        public GeneratorType Type { get; }

        public SyntaxNode Generate(MemberInfo memberInfo)
        {
            var methodInfo = memberInfo as MethodInfo;

            if (methodInfo == null)
                throw new InvalidOperationException("Supplied MemberInfo must be a MethodInfo");

            var methodName = methodInfo.Name;
            var returnType = GetType(methodInfo.ReturnType);

            var parameterInfos = methodInfo.GetParameters();

            var parameters = GetParameters(parameterInfos);

            return GenerateMethod(methodName, returnType)
                .WithExplicitInterfaceSpecifier(GenerateExplicitInterfaceSpecifier(methodInfo))
                .WithParameterList(parameters)
                .WithBody(GenerateMethodBody(methodInfo.ReturnType, parameterInfos));
        }

        private static ExplicitInterfaceSpecifierSyntax GenerateExplicitInterfaceSpecifier(MethodInfo methodInfo)
        {
            return SyntaxFactory.ExplicitInterfaceSpecifier(SyntaxFactory.IdentifierName(methodInfo.DeclaringType.Name));
        }

        private ParameterListSyntax GetParameters(IEnumerable<ParameterInfo> parameters)
        {
            if (!parameters.Any())
                return SyntaxFactory.ParameterList();

            return SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters.Select(CreateParameter)));
        }

        private TypeSyntax GetType(Type type)
        {
            return SyntaxFactory.IdentifierName(type.ToString());
        }

        private ParameterSyntax CreateParameter(ParameterInfo parameter)
        {
            return
                SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameter.Name))
                    .WithType(GetType(parameter.ParameterType));
        }

        private MethodDeclarationSyntax GenerateMethod(string methodName, TypeSyntax returnType)
        {
            return SyntaxFactory.MethodDeclaration(returnType, methodName);
        }

        private BlockSyntax GenerateMethodBody(Type returnType, IEnumerable<ParameterInfo> parameters)
        {
            if (returnType == typeof(void))
            {
                return SyntaxFactory.Block(GenerateVoidMethodBody(parameters));
            }

            return SyntaxFactory.Block(GenerateReturnMethodBody(returnType, parameters));
        }

        private StatementSyntax GenerateVoidMethodBody(IEnumerable<ParameterInfo> parameters)
        {
            var substitution = GenerateMethodSubstitution(parameters);

            return SyntaxFactory.ExpressionStatement(substitution);
        }

        private static InvocationExpressionSyntax GenerateMethodSubstitution(IEnumerable<ParameterInfo> parameters)
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
             IEnumerable<ParameterInfo> parameters,
            InvocationExpressionSyntax substitution)
        {
            if (!parameters.Any())
                return substitution;

            var arguments = parameters.Select(x => SyntaxFactory.IdentifierName(x.Name));

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

        private StatementSyntax GenerateReturnMethodBody(Type returnType, IEnumerable<ParameterInfo> parameters)
        {
            var substitution = GenerateGenericMethodSubstitution(parameters, returnType);

            return SyntaxFactory.ReturnStatement(substitution);
        }

        private static InvocationExpressionSyntax GenerateGenericMethodSubstitution(IEnumerable<ParameterInfo> parameters, Type returnType)
        {
            var typeList = SyntaxFactory.SeparatedList<TypeSyntax>(new[] { SyntaxFactory.IdentifierName(returnType.Name) });

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