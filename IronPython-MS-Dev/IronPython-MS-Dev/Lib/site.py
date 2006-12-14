#####################################################################################
#  
#  Copyright (c) Microsoft Corporation. All rights reserved.
# 
#  This source code is subject to terms and conditions of the Shared Source License
#  for IronPython. A copy of the license can be found in the License.html file
#  at the root of this distribution. If you can not locate the Shared Source License
#  for IronPython, please send an email to ironpy@microsoft.com.
#  By using this source code in any fashion, you are agreeing to be bound by
#  the terms of the Shared Source License for IronPython.
# 
#  You must not remove this notice, or any other, from this software.
# 
######################################################################################


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

# for importing the local python system library and site-package in the rPath distro
sys.path.append('/usr/lib/python2.4')
sys.path.append('/usr/lib/python2.4/site-packages')

# FePy additions

import encodings

import imp
import sys
import os

def override_builtin(name):
    sys.modules[name] = module = imp.new_module(name)
    path = os.path.join(sys.prefix, 'Lib', name + '.py')
    execfile(path, module.__dict__)

override_builtin('socket')

def install_sre_py():
    override_builtin('_sre')
    import sre
    sys.modules['re'] = sre

install_sre_py()

import cli
cli.install('cipher')

import _pth_support
import _codecs_errors
