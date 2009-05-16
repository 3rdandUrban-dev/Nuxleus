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
using System.Text;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Shell;
using Microsoft.Scripting.Internal;
using System.Reflection;
using Microsoft.Scripting.Helpers;

[assembly: ToyExtensionType(typeof(int), typeof(DefaultIntOps))]
namespace Microsoft.Scripting.Helpers
{
    public static class DefaultIntOps {
        [OperatorMethod]
        public static int Add(int x, int y)
        {
            return x + y;
        }
        [OperatorMethod]
        public static int Multiply(int x, int y) {
            return x * y;
        }
        [OperatorMethod]
        public static int Divide(int x, int y) {
            return x / y;
        }
        [OperatorMethod]
        public static int Subtract(int x, int y) {
            return x - y;
        }
    }

    class ToyExtensionTypeAttribute : ExtensionTypeAttribute {
        public ToyExtensionTypeAttribute(Type extends, Type extended)
            : base(extends, extended) {
        }

        public override ExtensionNameTransformer Transformer {
            get {
                return MyTransformer;
            }
        }

        private IEnumerable<TransformedName> MyTransformer(MemberInfo member, TransformReason reason) {
            if (member.IsDefined(typeof(OperatorMethodAttribute), false)) {
                Operators op;
                switch(member.Name) {
                    case "Add": op = Operators.Add; break;
                    case "Subtract": op = Operators.Subtract; break;
                    case "Divide": op = Operators.Divide; break;
                    case "Multiply": op = Operators.Multiply; break;
                    default: throw new NotImplementedException();
                }
                yield return new TransformedName(
                    new OperatorMapping(op, false, true, false),
                    ContextId.Empty);
            }

            yield return new TransformedName(member.Name, ContextId.Empty);
        }
    }
}
