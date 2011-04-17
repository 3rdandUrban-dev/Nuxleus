# Copyright (c) 2008 Yahoo! Inc.  All rights reserved.
# The copyrights embodied in the content of this file are licensed
# by Yahoo! Inc. under the BSD (revised) open source license


import wpaxes as axis

class WebPathCoreException(Exception):
    "Core runtime error evaluating WebPath"
    pass

def isstring(item):
    try: item + ''
    except: return False
    else: return True
    
def isnumber(item):
    try: item + 0
    except: return False
    else: return True
    
def isboolean(item):
    return (item is True or item is False)
    
def issequence(item):
    return isinstance(item, type([]))
        
def isnode(item):
    return hasattr(item, "nodeType")
    
def string(item):
    "convert any single item or nodeset into a string"
    if isstring(item): return item
    if isnumber(item): return str(item)    
    if item is True: return "true"
    if item is False: return "false"
    if item == []: return ""
    if issequence(item):
        return string(item[0]) # this is XPath 1.0 compat mode
    if isnode(item):
        allnodes = axis.descendent_or_self([item])
        return ''.join([n.nodeValue for n in allnodes if n.nodeType==3])
    raise WebPathCoreException, "Don't know how to convert " + item + " to string."
    
def number(item):
    "convert any single item or nodeset into a number"
    if isnumber(item): return item
    s = string(item)
    try:
        rc = float(s)
    except: raise WebPathCoreException, "Don't know how to convert " + item + "to number."
    else: return rc
    
def boolean(item):
    "convert any single item or nodeset into a boolean"
    if item is True: return True
    if item is False: return False
    if item == []: return False
    if item is "": return False
    if item is 0: return False
    if isstring(item): return True
    if isnumber(item): return True
    
    # need to check for nodesets and handle appropriately
    raise WebPathCoreException, "Don't know how to convert " + item + " to boolean."



