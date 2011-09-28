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
using Microsoft.Scripting.Math;
using Microsoft.Scripting.Internal.Ast;

using Microsoft.Scripting.Helpers;
using ToyScript.Parser;

namespace ToyScript
{
    class ToyCompiler:ScriptCompiler
    {
        public ToyCompiler(ScriptEngine engine) : base(engine) { }

        public override SourceUnit ParseCodeDom(System.CodeDom.CodeObject codeDom)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override CodeBlock ParseExpressionCode(CompilerContext cc)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override CodeBlock ParseFile(CompilerContext cc)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override CodeBlock ParseInteractiveCode(CompilerContext cc, bool allowIncomplete, out InteractiveCodeProperties properties)
        {
            ToyParser tp = new ToyParser(cc.SourceUnit.GetCode());
            CodeBlock block = CodeBlock.MakeCodeBlock("top", tp.ParseInteractiveStatement());
            properties = InteractiveCodeProperties.None;
            return block;            
        }

        public override CodeBlock ParseStatementCode(CompilerContext cc)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
