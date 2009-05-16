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
using Microsoft.Scripting.Internal.Generation;

namespace Microsoft.Scripting.Helpers
{
    class DefaultCommandLine : CommandLine
    {

        protected override int RunCommand(string command)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override int RunFile(string filename)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
