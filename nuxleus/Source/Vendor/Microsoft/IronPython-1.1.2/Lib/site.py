#####################################################################################
#
# Copyright (c) Microsoft Corporation. 
#
# This source code is subject to terms and conditions of the Microsoft Public
# License. A  copy of the license can be found in the License.html file at the
# root of this distribution. If  you cannot locate the  Microsoft Public
# License, please send an email to  dlr@microsoft.com. By using this source
# code in any fashion, you are agreeing to be bound by the terms of the 
# Microsoft Public License.
#
# You must not remove this notice, or any other, from this software.
#
#####################################################################################


import sys
import clr
from System.IO import Path, Directory, FileInfo

dir = Path.Combine(sys.prefix, 'DLLs')
if Directory.Exists(dir):
    sys.path.append(dir)
    files = Directory.GetFiles(dir)
    for file in files:
        if file.lower().endswith('.dll'):
            try:
                clr.AddReference(FileInfo(file).Name)
            except:
                pass

