# Copyright (c) 2008 Yahoo! Inc.  All rights reserved.
# The copyrights embodied in the content of this file are licensed
# by Yahoo! Inc. under the BSD (revised) open source license


# WebPath functions and operators

import sys
import wpcore as core
import wpaxes as axis

class WebPathFunctionException(Exception):
    "Function call problem in WebPath"


def getfunc(name):
    "get a function. If not found, throw WebPathFunctionException."
    fn = wp_fns.get(name)
    if fn:
        return fn
    raise WebPathFunctionException, "unknown function " + name
    
def std_true(args, ctxt=None):
    argver(args, maxargs=0) 
    return True

def std_false(args, ctxt=None):
    argver(args, maxargs=0)
    return False

def std_string(args, ctxt=None):
    "convert argument into a string, default is context node"
    argver(args, maxargs=1)
    if len(args) > 0:
        return core.string(args[0])
    elif ctxt:
        return core.string(ctxt.nodelist[0])

def std_string_length(args, ctxt):
    argver(args, [core.string], maxargs=1)
    return len(std_string(args, ctxt))

def std_contains(args, ctxt):
    # args[0] is str to search in, args[1] is what to look for
    (s, me) = argver(args, [core.string], reqargs=2, maxargs=2)
    return s.find(me) != -1

def std_concat(args,ctxt):
    if len(args)<2:
        raise WebPathFunctionException, "too few args"
    s = argver(args, [core.string],reqargs=len(args))
    concatenated_str = ''
    for str in s:
        concatenated_str+=str
    return concatenated_str

def std_count(args,ctxt):
    argver(args,maxargs=1)
    node_set = args[0]
    return len(node_set)

def std_substring(args, ctxt):
    (s, start) = argver(args, [core.string, core.number],
                             reqargs=2, maxargs=3)
    start = int(start) - 1 # XPath is 1-based
    if start < 0: start = 0
    if (len(args)>2):
        span = int(core.number(args[2]))
        if span < 0: span = 0
        return s[start:start+span]
    return s[start:]

def std_substring_before(args, ctxt):
    (s, frag) = argver(args, [core.string], reqargs=2, maxargs=2)
    loc = s.find(frag)
    if loc < 0: return ""
    return s[:loc]

def std_substring_after(args, ctxt):
    (s, frag) = argver(args, [core.string], reqargs=2, maxargs=2)
    loc = s.find(frag)
    if loc < 0: return ""
    return s[loc+len(frag):]

def std_starts_with(args, ctxt):
    (s, frag) = argver(args, [core.string], reqargs=2, maxargs=2)
    return s.startswith(frag)

def std_ends_with(args, ctxt):
    (s, frag) = argver(args, [core.string], reqargs=2, maxargs=2)
    return s.endswith(frag)

def std_normalize_space(args, ctxt):
    "remove all leading/trailing ws, all internal ws rep as single space"
    argver(args, maxargs=1)
    if len(args) > 0:
        s = core.string(args[0])
    elif ctxt:
        s = core.string(ctxt.nodelist[0])
    else: return ""
    return ' '.join(s.split())

def std_translate(args,ctxt):
    (s,s_find,s_replace) = argver(args, [core.string], reqargs=3, maxargs=3)
    mapping = {}
    #length of replace cannot be larger than find!
    find_length = len (s_find)
    replace_length = len(s_replace)
    for i in range(find_length):
        if i < replace_length:
            mapping[s_find[i]] = s_replace[i]
        else:
            mapping[s_find[i]] = ''
    keys = mapping.keys()
    for key in keys:
        s = s.replace(key, mapping[key])
    return s 


def std_root(args, ctxt):
    argver(args, maxargs=0)
    if not ctxt.nodelist[0].ownerDocument:
        return ctxt.nodelist[0] # this IS the root node
    return [ctxt.nodelist[0].ownerDocument]

def std_node(args, ctxt):
    argver(args, maxargs=0)
    return ctxt.nodelist

def std_text(args, ctxt):
    argver(args, maxargs=0)
    # as a node-test function, all args should be Nodes
    txtnodes = [n for n in ctxt.nodelist if n.nodeType==3]
    return txtnodes

def std_number(args, ctxt):
    "convert argument to a number"
    argver(args, [core.number], reqargs=1, maxargs=1)
    return core.number(args[0])

    
# EXPERIMENT: should string() take multiple params returning a sequence?

def experimental_seq(args, ctxt):
    "construct a flat sequence"
    result = []
    for arg in args:
        if core.isstring(arg):
            result.append(arg)
        else:
            try: result.extend(arg);
            except: result.append(arg)
    return result

def experimental_range(args, ctxt):
    (a,b) = argver(args, [core.number], reqargs=2, maxargs=2)
    a = int(a)
    b = int(b)+1
    # unlike python range(), we include the upper bound in the result
    if a > b:
        raise WebPathFunctionException, "Bad range(" + str(a) + "," + str(b) + ")"
    # TODO: use xrange
    return range(int(a), int(b))

def experimental_get(args, ctxt):
    argver(args, [core.string], reqargs=1, maxargs=1)
    # args[0] will be a list of convertable-to-string objects
    # or already a string itself
    try: args[0] + ''
    except: return axis.fetchDocs(map(core.string, args[0]), ctxt)
    else: return axis.fetchDocs([args[0]], ctxt)

def experimental_inline_text(args, ctxt):
    argver(args, reqargs=0, maxargs=1)
    #arg[0] needs to be a node (or default to context node)
    if len(args) == 0:
        nodes = ctxt.nodelist
    else:
        nodes = args[0]
    if len(nodes) < 1:
        return ""
    axis_nodes = axis.inline_descendent(nodes)
    rawstr = ''.join([n.nodeValue for n in axis_nodes if n.nodeType==3])
    return ' '.join(rawstr.split()) # normalize space
        

def argver(args, unpackers=[], reqargs=0, maxargs=sys.maxint):
    """verify and unpack arguments.
        args is the array of arguments
        unpackers is a 1:1 list of mapping functions
          (unpackers can be shorter than reqargs, in which case the last
          unpacker is used for the remainder of args)
        reqargs is the number of *required* arguments
        maxargs is the number of *maximum* arguments (including optional)
        raises WebPathFunctionException on missing/extra args
        returns a tuple of unpacked values equal in length to reqargs
        (you need to manually unpack optional args)"""
    if len(args) < reqargs:
        raise WebPathFunctionException, "too few args"
    if len(args) > maxargs:
        raise WebPathFunctionException, "too many args"
    results = []
    for i in range(reqargs):
        if len(unpackers) <= i:
            unpacker = unpackers[-1]
        else:
            unpacker = unpackers[i]
        results.append(unpacker(args[i]))
    return (results)


wp_fns = {
        'true' : std_true,
        'false' : std_false,
        'string' : std_string,
        'string-length' : std_string_length,
        'contains' : std_contains,
        'concat' : std_concat,
        'count' : std_count,
        'substring' : std_substring,
        'substring-before' : std_substring_before,
        'substring-after' : std_substring_after,
        'starts-with' : std_starts_with,
        'ends-with' : std_ends_with,
        'normalize-space' : std_normalize_space,
        'translate' : std_translate,
        'root' : std_root,
        'node' : std_node,
        'text' : std_text,
        'number' : std_number,
         
        # experimental functions: DO NOT USE
        'seq' : experimental_seq,
        'range' : experimental_range,
        'get' : experimental_get,
        'inline-text' : experimental_inline_text,
        }

