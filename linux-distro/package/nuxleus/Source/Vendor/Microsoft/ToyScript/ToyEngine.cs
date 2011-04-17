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
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Helpers;


namespace ToyScript
{
    class ToyEngine:ScriptEngine
    {
        private ActionBinder _defaultBinder;
        private LanguageContext _defaultContext;

        public ToyEngine(LanguageProvider provider, EngineOptions engineOptions) : base(provider, engineOptions) {
            IronPython.Runtime.Operations.Ops.Bool2Object(true); //awful initialization hack

            IronPython.Runtime.Operations.Ops.RegisterAssembly(typeof(ToyEngine).Assembly);

            _defaultContext = new ToyContext(this);
            _defaultBinder = new DefaultActionBinder(new CodeContext(null, _defaultContext));
        }

        public override ScriptCompiler Compiler
        {
            get { return new ToyCompiler(this); }
        }

        public override ActionBinder DefaultBinder
        {
            get { return _defaultBinder; }
        }

        protected override LanguageContext GetLanguageContext(ScriptModule module)
        {
            return _defaultContext;
        }

        protected override LanguageContext GetLanguageContext(CompilerOptions compilerOptions)
        {
            return _defaultContext;
        }

        public override CompilerOptions GetDefaultCompilerOptions()
        {
            return new DefaultCompilerOptions();
        }

        public override IAttributesCollection GetGlobalsDictionary(IDictionary<string, object> globals)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
