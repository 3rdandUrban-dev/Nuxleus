/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Permissive License. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Microsoft Permissive License, please send an email to 
 * ironpy@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Microsoft Permissive License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Diagnostics;

using Microsoft.Scripting.Math;
using Microsoft.Scripting;
using Microsoft.Scripting.Internal.Generation;
using Microsoft.Scripting.Internal.Ast;
using Microsoft.Scripting.Actions;

using TypeCache = IronPython.Runtime.Types.TypeCache;
using Converter = IronPython.Runtime.Converter;

namespace Microsoft.Scripting.Helpers
{
    class DefaultActionBinder :  ActionBinder
    {
        public DefaultActionBinder(CodeContext context)
            : base(context) {
        }

        protected override StandardRule<T> MakeRule<T>(Action action, object[] args) {
            switch (action.Kind) {
                case ActionKind.DoOperation:
                    return new DoOperationBinderHelper<T>(this, (DoOperationAction)action).MakeRule(args);
                //case ActionKind.GetMember:
                //    return new GetMemberBinderHelper<T>(this, (GetMemberAction)action).MakeNewRule(args);
                //case ActionKind.SetMember:
                //    return new SetMemberBinderHelper<T>(this, (SetMemberAction)action).MakeNewRule(args);
                //case ActionKind.Call:
                //    return new CallBinderHelper<T>(this).MakeRule(Context, (CallAction)action, args);
                default:
                    throw new NotImplementedException(action.ToString());
            }
        }

        public override Expression ConvertExpression(Expression expr, Type toType) {
            Type exprType = expr.ExpressionType;

            if (toType == typeof(object)) {
                if (exprType.IsValueType) {
                    return StaticUnaryExpression.Convert(expr, toType);
                } else {
                    return expr;
                }
            }

            if (toType.IsAssignableFrom(exprType)) {
                return expr;
            }

            BoundExpression be = expr as BoundExpression;
            if (be != null && be.Reference.KnownType != null) {
                if (toType.IsAssignableFrom(be.Reference.KnownType)) {
                    return StaticUnaryExpression.Convert(expr, toType);
                }
            }

            // We used to have a special case for int -> double...
            if (exprType != typeof(object)) {
                expr = StaticUnaryExpression.Convert(expr, typeof(object));
            }

            if (toType == typeof(object)) return expr;

            MethodInfo fastConvertMethod = GetFastConvertMethod(toType);
            if (fastConvertMethod != null) {
                return MethodCallExpression.Call(null, fastConvertMethod, expr);
            }

            if (typeof(Delegate).IsAssignableFrom(toType)) {
                return StaticUnaryExpression.Convert(
                    MethodCallExpression.Call(null,
                        typeof(Converter).GetMethod("ConvertToDelegate"),
                        expr,
                        ConstantExpression.Constant(toType)),
                    toType);
            }
            
            return ConditionalExpression.Condition(
                TypeBinaryExpression.TypeIs(
                    expr,
                    toType),
                StaticUnaryExpression.Convert(
                    expr,
                    toType),
                StaticUnaryExpression.Convert(
                    MethodCallExpression.Call(null, GetGenericConvertMethod(toType),
                        expr, ConstantExpression.Constant(toType.TypeHandle)),
                    toType));
        }



        private static MethodInfo GetGenericConvertMethod(Type toType) {
            if (toType.IsValueType) {
                if (toType.IsGenericType && toType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                    return typeof(Converter).GetMethod("ConvertToNullableType");
                } else {
                    return typeof(Converter).GetMethod("ConvertToValueType");
                }
            } else {
                return typeof(Converter).GetMethod("ConvertToReferenceType");
            }
        }


        private static MethodInfo GetFastConvertMethod(Type toType) {
            if (toType == typeof(char)) {
                return typeof(Converter).GetMethod("ConvertToChar");
            } else if (toType == typeof(int)) {
                return typeof(Converter).GetMethod("ConvertToInt32");
            } else if (toType == typeof(string)) {
                return typeof(Converter).GetMethod("ConvertToString");
            } else if (toType == typeof(long)) {
                return typeof(Converter).GetMethod("ConvertToInt64");
            } else if (toType == typeof(double)) {
                return typeof(Converter).GetMethod("ConvertToDouble");
            } else if (toType == typeof(bool)) {
                return typeof(Converter).GetMethod("ConvertToBoolean");
            } else if (toType == typeof(BigInteger)) {
                return typeof(Converter).GetMethod("ConvertToBigInteger");
            } else if (toType == typeof(Complex64)) {
                return typeof(Converter).GetMethod("ConvertToComplex64");
            } else if (toType == typeof(IEnumerable)) {
                return typeof(Converter).GetMethod("ConvertToIEnumerable");
            } else if (toType == typeof(float)) {
                return typeof(Converter).GetMethod("ConvertToSingle");
            } else if (toType == typeof(byte)) {
                return typeof(Converter).GetMethod("ConvertToByte");
            } else if (toType == typeof(sbyte)) {
                return typeof(Converter).GetMethod("ConvertToSByte");
            } else if (toType == typeof(short)) {
                return typeof(Converter).GetMethod("ConvertToInt16");
            } else if (toType == typeof(uint)) {
                return typeof(Converter).GetMethod("ConvertToUInt32");
            } else if (toType == typeof(ulong)) {
                return typeof(Converter).GetMethod("ConvertToUInt64");
            } else if (toType == typeof(ushort)) {
                return typeof(Converter).GetMethod("ConvertToUInt16");
            } else if (toType == typeof(Type)) {
                return typeof(Converter).GetMethod("ConvertToType");
            } else {
                return null;
            }
        }


        /// <summary>
        /// TODO Something like this method belongs on the Binder; however, it is probably
        /// something much more abstract.  This is just the first pass at removing this
        /// to get rid of the custom PythonCodeGen.
        /// </summary>
        public override void EmitConvertFromObject(CodeGen cg, Type toType) {
            if (toType == typeof(object)) return;

            MethodInfo fastConvertMethod = GetFastConvertMethod(toType);
            if (fastConvertMethod != null) {
                cg.EmitCall(fastConvertMethod);
                return;
            }

            if (toType == typeof(void)) {
                cg.Emit(OpCodes.Pop);
            } else if (typeof(Delegate).IsAssignableFrom(toType)) {
                cg.EmitType(toType);
                cg.EmitCall(typeof(Converter), "ConvertToDelegate");
                cg.Emit(OpCodes.Castclass, toType);
            } else {
                Label end = cg.DefineLabel();
                cg.Emit(OpCodes.Dup);
                cg.Emit(OpCodes.Isinst, toType);

                cg.Emit(OpCodes.Brtrue_S, end);
                cg.Emit(OpCodes.Ldtoken, toType);
                cg.EmitCall(GetGenericConvertMethod(toType));
                cg.MarkLabel(end);

                cg.Emit(OpCodes.Unbox_Any, toType); //??? this check may be redundant
            }
        }

        public override object Convert(object obj, Type toType) {
            throw new NotImplementedException();
            //return Converter.Convert(obj, toType);
        }

        public override bool CanConvertFrom(Type fromType, Type toType, NarrowingLevel level) {
            return Converter.CanConvertFrom(fromType, toType, level);
        }

        public override bool PreferConvert(Type t1, Type t2) {
            return Converter.PreferConvert(t1, t2);
        }
    }
}
