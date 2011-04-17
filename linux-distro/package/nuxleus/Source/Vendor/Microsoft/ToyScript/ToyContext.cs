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

namespace ToyScript
{
    class ToyContext : LanguageContext
    {

        public ToyContext(ScriptEngine engine) : base(engine) { }

        public override bool ShowCls
        {
            get
            {
                return true;
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }
    }
}
