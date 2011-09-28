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
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Diagnostics;

using Microsoft.Scripting.Internal.Generation;
using Microsoft.Scripting.Internal.Ast;

using IronPython.Compiler;

using Microsoft.Scripting;
using Microsoft.Scripting.Internal;
using Microsoft.Scripting.Actions;
using System.Threading;

namespace Microsoft.Scripting.Helpers {
    public class DoOperationBinderHelper<T> : BinderHelper<T> {
        private ActionBinder _binder;
        protected DoOperationAction _action;

        public DoOperationBinderHelper(ActionBinder binder, DoOperationAction action) {
            this._binder = binder;
            this._action = action;
        }

        public StandardRule<T> MakeRule(object[] args) {
            // First check for IDynamicObject and let the object try to handle this
            IActionable ido = args[0] as IActionable;
            if (ido != null) {
                StandardRule<T> rule = ido.GetRule<T>(_action, _binder.Context, args);
                if (rule != null) return rule;
            }

            // Next check for methods on the objects themselves - depending on arity of operation
            if (_action.IsUnary)
            {
                throw new NotImplementedException();
            }
            else
            {
                DynamicType dt = DynamicHelpers.GetDynamicType(args[0]);

                DynamicTypeSlot dts;
                if (dt.TryResolveSlot(_binder.Context, SymbolTable.StringToId(GetName()), out dts) ||
                    dt.TryResolveSlot(_binder.Context, SymbolTable.StringToId(GetSpecialName()), out dts)) {
                    BuiltinFunction bf = GetBuiltinFunction(dts);
                    if (bf != null) {
                        StandardRule<T> rule = new StandardRule<T>();
                        rule.SetTest(MakeTypeTest(rule, args));
                        MethodBase[] methods = bf.Targets;
                        for (int i = 0; i < methods.Length; i++) {
                            methods[i] = (MethodBase)methods[i];
                        }
                        MethodBinder binder = MethodBinder.MakeBinder(_binder, _action.ToString(), methods, BinderType.Normal);
                        MethodCandidate mc = binder.MakeBindingTarget(CallType.None, CompilerHelpers.ObjectTypes(args));
                        Expression call = mc.Target.MakeExpression(_binder, rule.Parameters);

                        rule.SetTarget(rule.MakeReturn(_binder, call));
                        return rule;
                    }
                }
                throw new NotImplementedException();
            }
        }

        private string GetName() {
            switch(_action.Operation) {
                case Operators.Add: return "Add";
                case Operators.Subtract: return "Subtract";
                case Operators.Multiply: return "Multiply";
                case Operators.Divide: return "Divide";
                default: throw new NotImplementedException();
            }
        }

        private string GetSpecialName() {
            switch (_action.Operation) {
                case Operators.Add: return "op_Addition";
                case Operators.Subtract: return "op_Subtraction";
                case Operators.Multiply: return "op_Multiplication";
                case Operators.Divide: return "op_Division";
                default: throw new NotImplementedException();
            }
        }

        public static BuiltinFunction GetBuiltinFunction(DynamicTypeSlot dts) {
            if (dts is BuiltinFunction)
                return (BuiltinFunction)dts;

            BuiltinMethodDescriptor bmd = dts as BuiltinMethodDescriptor;
            if (bmd != null) {
                return bmd.Template;
            }

            return null;
        }

        private Expression MakeTypeTest(StandardRule<T> rule, object[] args)
        {
            return ConstantExpression.Constant(true);
        }
    }
}
