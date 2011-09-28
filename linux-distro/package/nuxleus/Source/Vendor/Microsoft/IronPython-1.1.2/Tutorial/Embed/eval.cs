/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public
 * License. A  copy of the license can be found in the License.html file at the
 * root of this distribution. If  you cannot locate the  Microsoft Public
 * License, please send an email to  dlr@microsoft.com. By using this source
 * code in any fashion, you are agreeing to be bound by the terms of the 
 * Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using IronPython.Hosting;

public class Eval {
    public static void Main(string[] args) {
        PythonEngine pe = new PythonEngine();
        if (args.Length > 0) {
            try {
                Console.WriteLine(pe.Evaluate(args[0]));
            } catch {
                Console.WriteLine("Error");
            }
        } else Console.WriteLine("eval <expression>");
    }
}