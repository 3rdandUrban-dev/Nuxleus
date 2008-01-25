# Copyright (c) 2008 Yahoo! Inc.  All rights reserved.
# The copyrights embodied in the content of this file are licensed
# by Yahoo! Inc. under the BSD (revised) open source license


import unittest
from xml.dom.minidom import parse, parseString,Node
import sys

import wpfn as fn # webpath functions
import wpcore as core # core conversions
import wpaxes as axis # axes

# Phase 1 Tokenize ######################

# Using PLY: http://www.dabeaz.com/ply/ply.html

tokens = (
    'NAME',  # any unambiguous QName, NodeTest, FunctionName, AxisName, or VariableName
    'NUMBER', # any literal number
    'STRING', # any literal string
    'NAME_OR_OPERATOR', # any ambiguous operator ('div','mod','and','or')
                        # will be disambiguated after tokenization
    'OPERATOR',  # any unambiguous operator ('=', "<=", "::", "@", ...)
    'DELIM' # matchers, don't actually do anything in themselves, ), ]
    )

ambig_operators = {
    'mod' : 'NAME_OR_OPERATOR',
    'div' : 'NAME_OR_OPERATOR',
    'and' : 'NAME_OR_OPERATOR',
    'or'  : 'NAME_OR_OPERATOR',
    'to'  : 'NAME_OR_OPERATOR',
    '*'   : 'NAME_OR_OPERATOR',
    }

# Tokens

def t_NAME(t):
    r'[a-zA-Z_][a-zA-Z0-9._\-]*(:([a-zA-Z_][a-zA-Z0-9._\-]*|\*))?'
    # TODO: Unicodize the above
    # Note: This allows prefix:* syntax, but not unadorned *
    
    # if this name might be an operator, we give it a special type and check later
    t.type = ambig_operators.get(t.value, 'NAME')
    return t

def t_NUMBER(t):
    r'([0-9]+(\.[0-9]+)?)|\.[0-9]+'
    # Note: also allows leading . (e.g. .1234)
    # Note: no leading - sign is allowed here:
    #       unary minus is a separate operator
    try:
        t.value = float(t.value)
    except ValueError:
        print "Numeric value too large", t.value
        t.value = 0 # Infinity??
    return t

def t_STRING(t):
    r'"[^"]*"|\'[^\']*\''
    # single or double-quoted string
    t.value = t.value[1:-1] # strip quotes here
    return t

def t_MULTI_CHAR_OPERATOR(t):
    r'\.\.|//|\<=|\>=|!=|::'
    t.type = 'OPERATOR'
    return t
    
def t_PAREN_OR_BRACKET(t):
    r'\(|\['
    t.type = 'OPERATOR'
    return t
    
def t_DELIM_CLOSERS(t):
    r'\)|\]'
    t.type = 'DELIM'
    return t

def t_COMPARISON_OPERATOR(t):
    r'\<|\>|='
    t.type = 'OPERATOR'
    return t

def t_SINGLE_CHAR_OPERATOR(t):
    r'/|\||\+|\-|\*'
    # Note that * can also be a name, and is thus handled specially
    t.type = ambig_operators.get(t.value, 'OPERATOR')
    return t

def t_MISC_OPERATOR(t):
    r',|\$|@|\.'
    # Strictly speaking, not even operators
    t.type = 'OPERATOR'
    if t.value == ".":
        t.type = 'NAME'
    return t

# Ignored characters
t_ignore = " \t"

def t_newline(t):
    r'\n+'
    t.lexer.lineno += t.value.count("\n")
    
def t_error(t):
    print "Illegal character '%s'" % t.value[0]
    t.lexer.skip(1)
    
# Build the lexer
import lex
lex.lex(debug=0) 

# Phase 2 Recognition ###################

# This is an extra phase between lexing and parsing
# Since XPath has no reserved words, some context is needed
# to tell whether 'div', 'mod', '*', and similar tokens
# are names or operators

# XPath 2.0/XQuery engine Saxon uses this approach, see
# http://idealliance.org/papers/dx_xmle04/papers/02-03-02/02-03-02.html

def toksRaw(s=None):
    """Get all raw tokens, after parsing input string s.
    S may be omitted, in which case it is expected that
    you previously called lex.input() with the input str
    
    Generally speaking, don't call this except for unit tests"""
    tokens = []
    if s:
        lex.input(s)
    while True:
        tok = lex.token()
        if not tok: break
        tokens.append(tok)
    return tokens

def toks(s=None):
    """Get and adjust the token stream, resolving any
    tokens of type NAME_OR_OPERATOR and other abbrevs."""
    tnum = 0
    tokens = []
    def newtok(v, t):
        lextok = lex.LexToken()
        lextok.value = v
        lextok.type = t
        lextok.lineno = 0
        lextok.lexpos = 0
        return lextok
    prevtype = 'OPERATOR'
    this_from_prev = { 'OPERATOR' : 'NAME' }
    if s: lex.input(s)
    tok = lex.token()
    while True:
        if not tok: break
        if tok.value == "/" and prevtype == "OPERATOR":
            # bare / expression is shorthand for root()
            # leading /stuff expression is shorthard for root()/stuff
            tokens.append(newtok("root", "NAME"))
            tokens.append(newtok("(", "OPERATOR"))
            tokens.append(newtok(")", "DELIM"))
            slashtok = tok
            tnum += 1
            prevtype = tok.type
            tok = lex.token()
            if tok and (tok.type == "NAME" or tok.type=="NAME_OR_OPERATOR"):
                # this was not the bare slash,
                # so we need the slash separator after root()
                tokens.append(slashtok)
        elif tok.type == 'NAME_OR_OPERATOR':
            # Everything is considered an operator,
            # unless the previous token was an operator too
            tok.type = this_from_prev.get(prevtype, 'OPERATOR')
            tokens.append(tok)
            tnum += 1
            prevtype = tok.type
            tok = lex.token()
        elif tok.value == "@":
            # expand @ to attribute::
            tok.value = "attribute"
            tok.type = 'NAME'
            tokens.append(tok)
            tokens.append(newtok("::", "OPERATOR"))
            tnum += 1
            prevtype = tok.type
            tok = lex.token()
        elif tok.value == "//":
            # expand // to /descendent-or-self::node()/
            if prevtype == "OPERATOR":
                # and append root() if needed
                tokens.append(newtok("root", "NAME"))
                tokens.append(newtok("(", "OPERATOR"))
                tokens.append(newtok(")", "DELIM"))
            tok.value = "/"
            tok.type = "OPERATOR"
            tokens.append(tok)
            tokens.append(newtok("descendent-or-self", "NAME"))
            tokens.append(newtok("::", "OPERATOR"))
            tokens.append(newtok("node", "NAME"))
            tokens.append(newtok("(", "OPERATOR"))
            tokens.append(newtok(")", "DELIM"))
            tokens.append(newtok("/", "OPERATOR"))
            tnum += 1
            prevtype = tok.type
            tok = lex.token()
        else:
            tokens.append(tok)
            tnum += 1
            prevtype = tok.type
            tok = lex.token()
    return tokens

# Phase 3 Parsing #######################

# Top Down Operator Precedence parser for WebPath.
    
# Compare: http://javascript.crockford.com/tdop/tdop.html
class WebPathParseException(Exception):
    "Parse error evaluating WebPath"

class ProtoObj(object):
    def __init__(self, copyme=None):
        if copyme:
            self.__dict__ = copyme.__dict__.copy()
        else:
            self._nud = None
            self._led = None
    def __str__(self):
        rep = "[" + str(self.__dict__.get('value')) + "]"
        if self.__dict__.get("first"):
            rep += "first:{" + str(self.first) + "}"
        if self.__dict__.get("second"):
            rep += "second:{" + str(self.second) + "}"
        return rep
    def __repr__(self):
        return repr(self.__dict__)
    def error(self, msg):
        print msg
        raise WebPathParseException, msg
    def nud(self):
        if self._nud:
            return self._nud(self)
        else:
            self.error("Undefined.")
    def led(self, left):
        if self._led:
            return self._led(self, left)
        else:
            self.error("Missing operator.")

symbol_table = {}
tokens = []
token = None
nokens_nr = 0

def itself(self):
    return self

def advance(id=None):
    global token
    global symbol_table
    global token_nr
    global tokens
    if id and token.id != id:
        token.error("Expected '" + id + "', got '" + token.id + "'.")
    if token_nr >= len(tokens):
        token = symbol_table["(end)"]
        return
    t = tokens[token_nr]
    token_nr += 1
    v = t.value
    a = t.type
    if a == "NAME":
        o = symbol_table.get("(literal)")
        a = "literal"
    elif a == "OPERATOR" or a == "DELIM":
        o = symbol_table.get(v)
        if not o:
            t.error("Unknown Operator.")
    elif a == "STRING" or a == "NUMBER":
        o = symbol_table.get("(literal)")
        a = "literal"
    else:
        t.error("Unexpected token.")
    token = ProtoObj(o)
    token.value = v
    token.token = t
    token.tt = t.type
    token.arity = a
    return token

def expression(rbp):
    global token
    t = token
    advance()
    left = t.nud()
    while rbp < token.lbp:
        t = token
        advance()
        left = t.led(left)
    return left

def symbol(id, bp=0):
    global symbol_table
    s = symbol_table.get(id)
    if s:
        if bp >= s.lbp:
            s.lbp = bp
    else:
        s = ProtoObj()
        s.id = s.value = id
        s.lbp = bp
        symbol_table[id] = s
    return s

def infix(id, bp, led=None):
    s = symbol(id, bp)
    def infix_led(self, left):
        self.first = left
        self.second = expression(bp)
        self.arity = "binary"
        return self
    s._led = led or infix_led
    return s
    
def infixr(id, bp, led=None):
    s = symbol(id, bp)
    def infixr_led(self, left):
        self.first = left
        self.second = expression(bp-1)
        self.arity = "binary"
        return self
    s._led = led or infixr_led
    return s
    
def prefix(id, bp, nud=None):
    s = symbol(id)
    def prefix_nud(self):
        self.first = expression(bp)
        self.arity = "unary"
        return self
    s._nud = nud or prefix_nud
    return s
    
symbol("(end)")
symbol(")")
symbol("]")
symbol(",")
symbol(".")

symbol("(literal)")._nud = itself

# operator precedence table from Mike Kay's XPath 2.0 book:

# 1  ','
# 2  for, some, every, if
# 3  or
# 4  and
# 5  eq, ne, lt, le, gt, ge, =, !=, <, <=, >, >=, is, <<, >>
# 6  to
# 7  '+' (infix), '-' (infix)
# 8  *, div, idiv, mod
# 9  union, |
# 10 intersect, except
# 11 'instance of'
# 12 'treat as'
# 13 'castable as'
# 14 'cast as'
# 15 '+' (unary), '-' (unary)
# 16 /, //
# 17 [] (predicate)


infix(",", 10) # sequence concat/construct
infixr("or", 30)
infixr("and", 40)
infixr("=", 50)
infixr("!=", 50)
infixr("<", 50)
infixr("<=", 50)
infixr(">", 50)
infixr(">=", 50)
#infix("to", 60) # range constructor
infix("+", 70)
infix("-", 70)
infix("*", 80)
infix("div", 80)
infix("|", 90)
prefix("-", 150)
infix("/", 160) # path operator

def predicate_operator(self, left):
    self.first = left
    self.second = expression(0)
    self.arity = "binary"
    advance("]")
    return self
infix("[", 170, predicate_operator)

infix("::", 180) # axis separator

def function_call(self, left):
    a = []
    self.first = left
    self.second = a
    self.arity = "binary"
    # TODO: some sanity checking
    if token.id != ")":
        while True:
            a.append(expression(10))
            if token.id != ",":
                break
            advance(",")
    advance(")")
    return self
infix("(", 190, function_call)

def grouping_paren(self):
    e = expression(0)
    advance(")")
    return e
prefix("(", 0, grouping_paren)

prefix("$", 200)

# And that's it

def parse_it_baby(toks):
    global token_nr
    global tokens
    token_nr = 0
    tokens = toks
    advance()
    e = expression(0)
    advance("(end)")
    return e

# Phase 4 - Code Generation

# Given a parse tree and context, we now have everything we need
# to generate actual minidom calls that implement the WebPath
# expression

class WebPathRuntimeException(Exception):
    "Runtime error evaluating WebPath"


class WebPathContext(object):
    def __init__(self, nodelist):
        self.nodelist = nodelist
        self.nlstack = [nodelist]
        self.varbind = {}
        self.cachedocs = {}
        
    def push_nl(self, nl):
        "let a sub-tree work on a different context list"
        self.nlstack.append(nl)
        self.nodelist = nl
        return self
        
    def pop_nl(self):
        self.nlstack.pop()
        self.nodelist = self.nlstack[-1]
        
    def getfunc(self, name):
        return fn.getfunc(name)

def webpath(e, ctxt):
    #print "webpath: $$$", e.value, e, "$$$", e.id, e.tt

    if e.tt == "NAME":
        # abbreviated child axis, or . for context node
        if e.value == ".":
            return ctxt.nodelist
        else:
            axis_nodes = axis.child(ctxt.nodelist)
            return nodeTest(axis_nodes, e.value)
    elif e.tt == "NUMBER":
        return float(e.value)
    elif e.tt == "STRING":
        return e.value
    if e.arity == "binary":
        return wp_binary_impls[e.id](e.first, e.second, ctxt)
    elif e.arity == "unary":
        return wp_unary_impls[e.id](e.first, ctxt)
    else:
        raise WebPathRuntimeException, "I'm confused about " + e

def webpath_string(e, ctxt):
    return core.string(webpath(e, ctxt))

def impl_plus(left,right, ctxt):
    "implement addition. Convert both params to numbers as appropriate"
    lv = core.number(webpath(left, ctxt))
    rv = core.number(webpath(right, ctxt))
    return lv + rv
    
def impl_minus(left, right, ctxt):
    "implement subtraction. Convert both params to numbers as appropriate"
    lv = core.number(webpath(left, ctxt))
    rv = core.number(webpath(right, ctxt))
    return lv - rv
    
def impl_times(left, right, ctxt):
    "implement multiplication. Convert both params to numbers as appropriate"
    lv = core.number(webpath(left, ctxt))
    rv = core.number(webpath(right, ctxt))
    return lv * rv
    
def impl_divide(left, right, ctxt):
    "implement division. Convert both params to numbers as appropriate"
    lv = core.number(webpath(left, ctxt))
    rv = core.number(webpath(right, ctxt))
    return lv / rv
    
def general_comparator(expr1, expr2, operator):
    # for cmp nodeset to nodeset, = iff any node in left has = string value of node in right
    # for cmp nodeset to num/str, = iff any converted node is =
    # for cmp nodeset to bool, convert the entire nodeset to a bool and cmp
    # for cmp b to b/s/n, convert 2nd op to bool, compare
    # for cmp n to b/s/n, convert 2nd op to number, compare
    # else, convert both operands to string, compare
    if core.issequence(expr1):
        return general_comparator_seq(expr1, expr2, operator)
    if core.issequence(expr2):
        return general_comparator_seq(expr2, expr1, operator)
    if core.isboolean(expr1) or core.isboolean(expr2):
        return operator(core.boolean(expr1), core.boolean(expr2))
    if core.isnumber(expr1) or core.isnumber(expr2):
        return operator(core.number(expr1), core.number(expr2))
    return operator(core.string(expr1), core.string(expr2))
    
def general_comparator_seq(seq, expr, operator):
    "seq is guaranteed to be a sequence. expr may or may not be"
    if core.issequence(expr):
        # n-squared comparisons string-wise
        if not (seq and expr): return False # comparison against empty
        sseq1 = map(core.string, seq)
        sseq2 = map(core.string, expr)
        for lval in sseq1:
            for rval in sseq2:
                if operator(lval, rval): return True
        return False
    if core.isboolean(expr):
        return operator(core.boolean(seq), expr)
    # otherwise n comparisons, number or string-wise
    if core.isnumber(expr): mapfn = core.number
    if core.isstring(expr): mapfn = core.string
    seq = map(mapfn, seq)
    for lval in seq:
        if operator(lval, expr): return True
    return False
    
def impl_equals(left, right, ctxt):
    "implement equality comparison"
    lval = webpath(left, ctxt)
    rval = webpath(right, ctxt)
    import operator
    
    return general_comparator(lval, rval, operator.__eq__)
def impl_notequals(left, right, ctxt):
    "implement inequality comparison"
    lval = webpath(left, ctxt)
    rval = webpath(right, ctxt)
    import operator
    return general_comparator(lval, rval, operator.__ne__)

def impl_less(left, right, ctxt):
    "implement < converting both sides to equals"
    lval = core.number(webpath(left, ctxt))
    rval = core.number(webpath(right, ctxt))
    return lval < rval
    
def impl_lessequal(left, right, ctxt):
    "implement <= converting both sides to numbers"
    lval = core.number(webpath(left, ctxt))
    rval = core.number(webpath(right, ctxt))
    return lval <= rval
    
def impl_great(left, right, ctxt):
    "implement > converting both sides to numbers"
    lval = core.number(webpath(left, ctxt))
    rval = core.number(webpath(right, ctxt))
    return lval > rval

def impl_greatequal(left, right, ctxt):
    "implement >= converting both sides to numbers"
    lval = core.number(webpath(left, ctxt))
    rval = core.number(webpath(right, ctxt))
    return lval >= rval

def impl_slash(left, right, ctxt):
    "implement general path operator. Repeatedly eval right with left as ctxt."
    # resolve left into a nodelist
    left_nodes = webpath(left, ctxt)
    #print "left_nodes:", left_nodes
    # for each node in nodelist, set it as the context and eval right
    
    # Special case: function-node tests get implied child axis
    # Fncall
    spclfn = ("node", "text", "comment", "processing-instruction")
    if right.value == "(" and right.first.value in spclfn:
        left_nodes = axis.child(left_nodes)

    result_nodes = webpath(right, ctxt.push_nl(left_nodes))
    ctxt.pop_nl()
    return result_nodes
    
def impl_predicate(left, right, ctxt):
    "filter a nodelist based on predicate"
    result_nodes = []
    left_nodes = webpath(left, ctxt)
    pos = 1.0
    for node in left_nodes:
        if right.tt == "NUMBER": # shorthand predicate like *[42]
            boolres = (right.value == pos)
        else:
            boolres = webpath(right, ctxt.push_nl([node]))
            ctxt.pop_nl()
        if boolres:
            result_nodes.append(node)
        pos += 1
    return result_nodes
    
def impl_axis(left, right, ctxt):
    "Given an axis specifier (left) and nodeTest (right), compute the nodelist"
    orig_nodes = ctxt.nodelist
    lv = left.value
    if lv=="child": axis_nodes = axis.child(orig_nodes)
    if lv=="preceding-sibling": axis_nodes = axis.preceding_sibling(orig_nodes)
    if lv=="following-sibling": axis_nodes = axis.following_sibling(orig_nodes)
    if lv=="attribute": axis_nodes = axis.attribute(orig_nodes)
    if lv=="self": axis_nodes = axis.self(orig_nodes)
    if lv=="descendent": axis_nodes = axis.descendent(orig_nodes)
    if lv=="descendent-or-self": axis_nodes = axis.descendent_or_self(orig_nodes)
    if lv=="traverse": axis_nodes = axis.traverse(orig_nodes, ctxt)
    
    # undocumented, do not use
    if lv=="inline-descendent": axis_nodes = axis.inline_descendent(orig_nodes)
    
    # todo: rest of axes
    if right.value == "(":
        # function call node test @@Fncall
        filtered_nodes = webpath(right, ctxt.push_nl(axis_nodes))
        ctxt.pop_nl()
    else:
        filtered_nodes = nodeTest(axis_nodes, right.value)
    return filtered_nodes
    
def impl_apply(left, right, ctxt):
    "Lookup a function and apply the arguments to it; left=fname, right=args"
    fn = ctxt.getfunc(left.value)
    args = []
    for v in right: # resolve arguments
        args.append(webpath(v, ctxt))
    return fn(args, ctxt)
    
def impl_union(left, right, ctxt):
    lnodes = webpath(left, ctxt)
    rnodes = webpath(right, ctxt)
    return lnodes + rnodes # this is really poor
    # TODO: we need real unions here, and in each path step
    
def impl_seqconcat(left, right, ctxt):
    lseq = webpath(left, ctxt)
    rseq = webpath(right, ctxt)
    if not core.issequence(lseq):
        lseq = [lseq]
    if not core.issequence(rseq):
        rseq = [rseq]
    return lseq + rseq

def impl_and(left,right,ctxt):
    lval = webpath(left,ctxt)
    rval = webpath(right,ctxt)
    return core.boolean(lval) and core.boolean(rval)

def impl_or(left,right,ctxt):
    lval = webpath(left,ctxt)
    rval = webpath(right,ctxt)
    return core.boolean(lval) or core.boolean(rval)

def impl_varlookup(arg, ctxt):
    "resolve a variable reference"
    return ctxt.varbind[arg.value]
    
def impl_grouping(arg, ctxt):
    "parenthesized expression"
    return webpath(arg, ctxt)
    
def impl_unary_minus(arg, ctxt):
    return -1 * core.number(webpath(arg, ctxt))
    pass

wp_binary_impls = {
        '+' : impl_plus,
        '-' : impl_minus,
        '*' : impl_times,
        'div' : impl_divide,
        '=' : impl_equals,
        '!=': impl_notequals,
        '<' : impl_less,
        '<=': impl_lessequal,
        '>' : impl_great,
        '>=': impl_greatequal,
        '/' : impl_slash,
        '[' : impl_predicate,
        '::': impl_axis,
        '(' : impl_apply,
        '|' : impl_union,
        ',' : impl_seqconcat,
        'and' : impl_and,
        'or' : impl_or
        }

wp_unary_impls = {
        '$' : impl_varlookup,
        '(' : impl_grouping,
        '-' : impl_unary_minus,
        }


def nodeTest(nodelist, test):
    if test== "*": return nodelist
    return [n for n in nodelist if (n.nodeName==test and n.nodeType!=Node.DOCUMENT_TYPE_NODE)] 

              
# Unit Tests...
class TestTokenizer(unittest.TestCase):
    def setUp(self):
        pass
    
    def tokValues(self, s=None):
        return [t.value for t in toksRaw(s)]
        
    def tokValuesA(self, s=None):
        return [t.value for t in toks(s)]
        
    def tokTypes(self, s=None):
        return [t.type for t in toksRaw(s)]
        
    def tokTypesA(self, s=None):
        return [t.type for t in toks(s)]
        
    def testTokenizer(self):
        print "testTokenizer..."
        self.assert_(True)
        self.assertEquals(self.tokValues(".."), [".."])
        self.assertEquals(self.tokValues("."), ["."])
        self.assertEquals(self.tokValues("/"), ["/"])
        self.assertEquals(self.tokValues("spam"), ["spam"])
        self.assertEquals(self.tokValues("child::spam"), ["child","::","spam"])
        self.assertEquals(self.tokValues("3.14"), [3.14])
        self.assertEquals(self.tokValues(".123"), [0.123])
        self.assertEquals(self.tokValues("'string'"), ["string"])
        self.assertEquals(self.tokValues("$a/b"), ["$", "a", "/", "b"])
        self.assertEquals(self.tokValues("/eggs"), ["/", "eggs"])
        self.assertEquals(self.tokValues("a[@b]//c(.)"), ["a","[","@","b","]","//","c","(",".",")"])
        self.assertEquals(self.tokValues("/* div //b"), ["/", "*", "div", "//", "b"])
        self.assertEquals(self.tokValues("$var"), ["$", "var"])
        self.assertEquals(self.tokValues("a|b"), ["a", "|", "b"])
        
        O = "OPERATOR"
        NO = "NAME_OR_OPERATOR"
        N = "NAME"
        D = "DELIM"
        NUM = "NUMBER"
        # / expands to root()
        self.assertEquals(self.tokValuesA("/"), ["root", "(", ")"])
        self.assertEquals(self.tokTypesA("/"), [N, O, D])
        
        # /foo expands to root()/foo
        self.assertEquals(self.tokValuesA("/*"), ["root", "(", ")", "/", "*"])
        self.assertEquals(self.tokTypesA("/*"), [N, O, D, O, N])
        
        self.assertEquals(self.tokTypesA("spam"), [N])
        self.assertEquals(self.tokTypesA("child::spam"), [N, O, N])
        
        # Both * and div are ambiguous at this stage
        self.assertEquals(self.tokTypes("/* div /b:*"), [O, NO, NO, O, N])
        self.assertEquals(self.tokTypes("mod mod mod"), [NO, NO, NO])
        self.assertEquals(self.tokTypes("* * *"), [NO, NO, NO])
        
        # See whether the disambig worked
        # equv. root()/name div root()/name
        self.assertEquals(self.tokTypesA("/* div /b:*"), [N, O, D, O, N, O, N, O, D, O, N])
        self.assertEquals(self.tokTypesA("mod mod mod"), [N, O, N])
        self.assertEquals(self.tokTypesA("* * *"), [N, O, N])
        
        # test expansion of abbreviations
        self.assertEquals(self.tokValuesA("@foo"), ["attribute", "::", "foo"])
        self.assertEquals(self.tokTypesA("@foo"), [N, O, N])
        
        self.assertEquals(self.tokValuesA("a//b"), ["a", "/", "descendent-or-self", "::", "node", "(", ")", "/", "b"])
        self.assertEquals(self.tokTypesA("a//b"), [N, O, N, O, N, O, D, O, N])
        
        self.assertEquals(self.tokValuesA("//x"), ["root", "(", ")", "/", "descendent-or-self", "::", "node", "(", ")", "/", "x"])
        self.assertEquals(self.tokTypesA("//x"), [N, O, D, O, N, O, N, O, D, O, N])
        
        # test function calls and predicates (delimiters, subexpressions)
        self.assertEquals(self.tokValuesA("true()"), ["true", "(", ")"])
        self.assertEquals(self.tokTypesA("true()"), [N, O, D])
        
        self.assertEquals(self.tokTypesA("a()/b()"), [N, O, D, O, N, O, D])
        self.assertEquals(self.tokTypesA("/b()"), [N, O, D, O, N, O, D])
        
        self.assertEquals(self.tokValuesA("func(.,/b)"), ["func", "(", ".", ",", "root", "(", ")", "/", "b", ")"])
        self.assertEquals(self.tokTypesA("func(.,/b)"), [N, O, N, O, N, O, D, O, N, D])
        
        self.assertEquals(self.tokValuesA("func(/,b)"), ["func", "(", "root", "(", ")", ",", "b", ")"])
        self.assertEquals(self.tokTypesA("func(/,b)"), [N, O, N, O, D, O, N, D])
        
        self.assertEquals(self.tokTypesA("*[*]"), [N, O, N, D])
        
        self.assertEquals(self.tokValuesA("*[/]"), ["*", "[", "root", "(", ")", "]"])
        self.assertEquals(self.tokTypesA("*[/]"), [N, O, N, O, D, D])
        
        self.assertEquals(self.tokValuesA("/[*]"), ["root", "(", ")", "[", "*", "]"])
        self.assertEquals(self.tokTypesA("/[*]"), [N, O, D, O, N, D])
        
        self.assertEquals(self.tokTypesA("/*[*] * div[/div]"), [N, O, D, O, N, O, N, D, O, N, O, N, O, D, O, N, D])

        self.assertEquals(self.tokTypesA("/*[*]/*"), [N, O, D, O, N, O, N, D, O, N])
        
        self.assertEquals(self.tokValuesA("$foo"), ["$", "foo"])
        self.assertEquals(self.tokTypesA("$foo"), [O, N])
        
        self.assertEquals(self.tokValuesA("-3.1415"), ["-", 3.1415])
        self.assertEquals(self.tokTypesA("-3.1415"), [O, NUM])
        
        self.assertEquals(self.tokValuesA("(foo)"), ["(", "foo", ")"])
        self.assertEquals(self.tokTypesA("(foo)"), [O, N, D])
        
        # (/) should expand to (root())
        self.assertEquals(self.tokValuesA("(/)"), ["(", "root", "(", ")", ")"])
        self.assertEquals(self.tokTypesA("(/)"), [O, N, O, D, D])
        
        self.assertEquals(self.tokValuesA("(/) / *"), ["(", "root", "(", ")", ")", "/", "*"])
        self.assertEquals(self.tokTypesA("(/) / *"), [O, N, O, D, D, O, N])
        
        # space is required after div (div4 is a legit NAME)
        self.assertEquals(self.tokValuesA("1+2*3div 4"), [1, "+", 2, "*", 3, "div", 4])
        self.assertEquals(self.tokTypesA("1+2*3div 4"), [NUM, O, NUM, O, NUM, O, NUM])
        
class TestParser(unittest.TestCase):
    def setUp(self):
        pass
        
    def testParser(self):
        print "testParser..."
        
        toks0 = toks(".")
        e = parse_it_baby(toks0)
        self.assertEquals(e.value, ".")
        
        toks1 = toks("child::a")
        e = parse_it_baby(toks1)
        self.assertEquals(e.value, "::")
        self.assertEquals(e.first.value, "child")
        self.assertEquals(e.second.value, "a")
        
        toks2 = toks("child::a/child::b")
        e = parse_it_baby(toks2)
        self.assertEquals(e.value, "/")
        self.assertEquals(e.first.first.value, "child")
        self.assertEquals(e.first.second.value, "a")
        self.assertEquals(e.second.first.value, "child")
        self.assertEquals(e.second.second.value, "b")

        toks3 = toks("a+b")
        e = parse_it_baby(toks3)
        self.assertEquals(e.value, "+")
        self.assertEquals(e.first.value, "a")
        self.assertEquals(e.second.value, "b")
        
        toks4 = toks("ancestor::spam[traverse::eggs]")
        e = parse_it_baby(toks4)
        self.assertEquals(e.value, "[")
        self.assertEquals(e.first.value, "::")
        self.assertEquals(e.first.first.value, "ancestor")
        self.assertEquals(e.first.second.value, "spam")
        self.assertEquals(e.second.value, "::")
        self.assertEquals(e.second.first.value, "traverse")
        self.assertEquals(e.second.second.value, "eggs")
        
        toks5 = toks("a[b/c]/d")
        e = parse_it_baby(toks5)
        self.assertEquals(e.value, "/")
        self.assertEquals(e.first.value, "[")
        self.assertEquals(e.first.first.value, "a")
        self.assertEquals(e.first.second.value, "/")
        self.assertEquals(e.first.second.first.value, "b")
        self.assertEquals(e.first.second.second.value, "c")
        self.assertEquals(e.second.value, "d")

        toks6 = toks("callme(withthis, andthis)")
        e = parse_it_baby(toks6)
        self.assertEquals(e.value, "(")
        self.assertEquals(e.first.value, "callme")
        self.assertEquals(len(e.second), 2)
        self.assertEquals(e.second[0].value, "withthis")
        self.assertEquals(e.second[1].value, "andthis")
        
        toks7 = toks("a/b()/c")
        # Should be      /
        #             /     c
        #           a   (
        #              b  []
        e = parse_it_baby(toks7)
        self.assertEquals(e.value, "/")
        self.assertEquals(e.first.value, "/")
        self.assertEquals(e.first.first.value, "a")
        self.assertEquals(e.first.second.value, "(")
        self.assertEquals(e.first.second.first.value, "b")
        self.assertEquals(e.first.second.second, [])
        self.assertEquals(e.second.value, "c")
        
        toks8 = toks("a/@b")
        e = parse_it_baby(toks8)
        self.assertEquals(e.value, "/")
        self.assertEquals(e.first.value, "a")
        self.assertEquals(e.second.value, "::") # expanded
        self.assertEquals(e.second.first.value, "attribute")
        self.assertEquals(e.second.second.value, "b")
        
        toks9 = toks("a//b")
        # should expand out similar to the a/b()/c example
        e = parse_it_baby(toks9)
        self.assertEquals(e.value, "/") # expanded
        self.assertEquals(e.first.value, "/")
        self.assertEquals(e.first.first.value, "a")
        self.assertEquals(e.first.second.value, "::")
        self.assertEquals(e.first.second.first.value, "descendent-or-self")
        self.assertEquals(e.first.second.second.value, "(")
        self.assertEquals(e.first.second.second.first.value, "node")
        self.assertEquals(e.first.second.second.second, [])
        self.assertEquals(e.second.value, "b")
        
        toks10 = toks("a()[b]")
        e = parse_it_baby(toks10)
        self.assertEquals(e.value, "[")
        self.assertEquals(e.first.value, "(")
        self.assertEquals(e.first.first.value, "a")
        self.assertEquals(e.first.second, [])
        self.assertEquals(e.second.value, "b")
        
        toks11 = toks("/")
        e = parse_it_baby(toks11)
        self.assertEquals(e.value, "(")
        self.assertEquals(e.first.value, "root")
        self.assertEquals(e.second, [])
        
        toks12 = toks("/*")
        e = parse_it_baby(toks12)
        self.assertEquals(e.value, "/")
        self.assertEquals(e.first.value, "(")
        self.assertEquals(e.first.first.value, "root")
        self.assertEquals(e.first.second, [])
        self.assertEquals(e.second.value, "*")
        
        toks13 = toks("$foo")
        e = parse_it_baby(toks13)
        self.assertEquals(e.value, "$")
        self.assertEquals(e.first.value, "foo")
        
        toks14 = toks("$foo/@bar")
        e = parse_it_baby(toks14)
        self.assertEquals(e.value, "/")
        self.assertEquals(e.first.value, "$")
        self.assertEquals(e.first.first.value, "foo")
        self.assertEquals(e.second.value, "::") # expanded
        self.assertEquals(e.second.first.value, "attribute")
        self.assertEquals(e.second.second.value, "bar")
        
        toks15 = toks("//*[contains(.,'foo')]")
        e = parse_it_baby(toks15)
        self.assertEquals(e.value, "/")
        self.assertEquals(e.first.value, "/")
        self.assertEquals(e.first.first.value, "(")
        self.assertEquals(e.first.first.first.value, "root")
        self.assertEquals(e.first.first.second, [])
        
        self.assertEquals(e.first.second.value, "::")
        self.assertEquals(e.first.second.first.value, "descendent-or-self")
        self.assertEquals(e.first.second.second.value, "(")
        self.assertEquals(e.first.second.second.first.value, "node")
        self.assertEquals(e.first.second.second.second, [])
        
        self.assertEquals(e.second.value, "[")
        self.assertEquals(e.second.first.value, "*")
        self.assertEquals(e.second.second.value, "(")
        self.assertEquals(e.second.second.first.value, "contains")
        self.assertEquals(e.second.second.second[0].value, ".")
        self.assertEquals(e.second.second.second[1].value, "foo")
        
        
class TestWebPathInterp(unittest.TestCase):
    def setUp(self):
        pass
    
    def wp(self, expr, doc):
        "execute a WebPath expr. Use documentElement as context"
        tree = parse_it_baby(toks(expr))
        ctxt = WebPathContext([doc.documentElement])
        return webpath(tree, ctxt)        
        
    def testInterp(self):
        print "testCodeGen..."
        rssdoc = parseString("<rss><channel spam='eggs'><item><guid>1</guid></item><item><guid>2</guid></item></channel></rss>")
        linkdoc = parseString("<root><a href='http://dubinko.info'>click</a></root>")
        ids = parseString("<r id='root'><child id='c1'/><child id='c2'><gc id='ham'/></child><child id='c3'/></r>")
        uniondoc = parseString("<r><a><x/></a><b><y/></b></r>")
    
        testdoc = parseString('''<!DOCTYPE story PUBLIC "hello world" "http://whatever.com"><story>

          <chapter><title>Chapter 1</title>
            <par>A Dungeon horrible, on all sides round</par>
          </chapter>

          <chapter><title>Chapter 2</title>
            <par>More unexpert, I boast not: them let those</par>
            <par>Contrive who need, or when they need, not now.</par>
            <sect><title>Chapter 2, Section 1</title>
            <par>For while they sit contriving, shall the rest,</par>
            <par>Millions that stand in Arms, and longing wait</par>
            </sect>
          </chapter>

          <chapter><title>Chapter 3</title>
            <par>So thick a drop serene hath quenched their Orbs</par>
          </chapter>

            </story> ''')

        result = self.wp("/story/preceding-sibling::story",testdoc)
        self.assertEquals(len(result),0)

        result = self.wp("string(/story/chapter/preceding-sibling::chapter[2]/title)",testdoc)
        self.assertEquals(result,"Chapter 1")

        result = self.wp("string(/story/chapter/following-sibling::chapter[2]/title)",testdoc)
        self.assertEquals(result,"Chapter 3")

        result = self.wp("/story/chapter[3]/preceding-sibling::chapter/title",testdoc)
        self.assertEquals(len(result),2)
        self.assertEquals(result[0].nodeName,"title")
        self.assertEquals(result[0].childNodes[0].nodeValue,"Chapter 1")

        self.assertEquals(result[1].nodeName,"title")
        self.assertEquals(result[1].childNodes[0].nodeValue,"Chapter 2")
        # first try out some basic stuff
        result = self.wp("count(//channel)",rssdoc)
        self.assertEquals(result, 1)

        result = self.wp("3.1415", rssdoc)
        self.assertEquals(result, 3.1415)
        
        result = self.wp("-3.1415", rssdoc)
        self.assertEquals(result, -3.1415)
        
        result = self.wp("'spam'", rssdoc)
        self.assertEquals(result, "spam")
        
        result = self.wp("3 = 3", rssdoc)
        self.assertEquals(result, True)
        
        result = self.wp("3.14 = 3.15", rssdoc)
        self.assertEquals(result, False)
        
        result = self.wp("'webpath' = 'webpath'", rssdoc)
        self.assertEquals(result, True)
        
        result = self.wp("'spam' = 'eggs'", rssdoc)
        self.assertEquals(result, False)
        
        result = self.wp("'3' = 3", rssdoc)
        self.assertEquals(result, True)
        
        result = self.wp("true() = 1", rssdoc)
        self.assertEquals(result, True)
        
        result = self.wp("true()", rssdoc)
        self.assertEquals(result, True)
        
        result = self.wp("string('abcde')", rssdoc)
        self.assertEquals(result, "abcde") 
        
        result = self.wp("string-length('abcde')", rssdoc)
        self.assertEquals(result, 5)
        
        result = self.wp("string(3.1415)", rssdoc)
        self.assertEquals(result, "3.1415")
        
        result = self.wp("string(a)", linkdoc)
        self.assertEquals(result, "click")
        
        # string() concats text values of all child nodes
        result = self.wp("string(channel)", rssdoc)
        self.assertEquals(result, "12")
        
        # and defaults to the context node
        result = self.wp("string()", rssdoc)
        self.assertEquals(result, "12")
        
        # note that numeric offset (2nd param) is 1-based
        result = self.wp("substring('abcdefghij', 2, 3)", rssdoc)
        self.assertEquals(result, "bcd")
        
        result = self.wp("substring('abcdefghij', 2, 0)", rssdoc)
        self.assertEquals(result, "")
        
        result = self.wp("substring('abcdefghij', 0, 100)", rssdoc)
        self.assertEquals(result, "abcdefghij")
        
        result = self.wp("substring('abcdeghij', 0, 0)", rssdoc)
        self.assertEquals(result, "")
        
        result = self.wp("substring('abcdefghij', 5)", rssdoc)
        self.assertEquals(result, "efghij")
        
        result = self.wp("substring('abcdefghij', 1, 9)", rssdoc)
        self.assertEquals(result, "abcdefghi")
        
        result = self.wp("substring('abcdefghij', 1, 10)", rssdoc)
        self.assertEquals(result, "abcdefghij")
        
        result = self.wp("substring-before('foo', 'x')", rssdoc)
        self.assertEquals(result, "")
        
        result = self.wp("substring-before('abcdefghij','f')", rssdoc)
        self.assertEquals(result, "abcde")
        
        result = self.wp("substring-after('foo', 'x')", rssdoc)
        self.assertEquals(result, "")
        
        result = self.wp("substring-after('abcdefghij', 'f')", rssdoc)
        self.assertEquals(result, "ghij")
        
        result = self.wp("starts-with('foo', 'x')", rssdoc)
        self.assertEquals(result, False)
        
        result = self.wp("starts-with('abcdefghij', 'abcd')", rssdoc)
        self.assertEquals(result, True)
        
        result = self.wp("ends-with('foo', 'x')", rssdoc)
        self.assertEquals(result, False)
        
        result = self.wp("ends-with('abcdefghij', 'hij')", rssdoc)
        self.assertEquals(result, True)
        
        result = self.wp("concat('hel', 'lo', ' worl', 'd')", rssdoc)
        self.assertEquals(result, "hello world")
 
        result = self.wp("normalize-space('   a\tb c\rd\ne  ')", rssdoc)
        self.assertEquals(result, "a b c d e")
        
        result = self.wp("normalize-space('abc')", rssdoc)
        self.assertEquals(result, "abc")
        
        result = self.wp("normalize-space('')", rssdoc)
        self.assertEquals(result, "")
       
        result = self.wp("translate('--aaa--','abc-','ABC')",rssdoc)
        self.assertEquals(result, "AAA")

        result = self.wp(u"translate('of \u201CThe Wire,\u201D F','\u0022\u201C\u201D\u0060\u00B4\u0018\u0019','')",rssdoc)
        self.assertEquals(result, u'''of The Wire, F''')


        # test comparing node-set to string
        result = self.wp("child[@id='c2']", ids)
        self.assertEquals(len(result), 1)
        
        result = self.wp("channel/item/guid = 1", rssdoc)
        self.assertEquals(result, True)
        
        result = self.wp("channel/item/guid = 2", rssdoc)
        self.assertEquals(result, True)
        
        result = self.wp("channel/item/guid = 3", rssdoc)
        self.assertEquals(result, False)
                
        result = self.wp("contains('abcde', 'bc')", rssdoc)
        self.assertEquals(result, True)
        
        result = self.wp("contains('abcde', 'z')", rssdoc)
        self.assertEquals(result, False)
        
        result = self.wp("number('0')", rssdoc)
        self.assertEquals(result, 0)
        
        result = self.wp("number('3.14')", rssdoc)
        self.assertEquals(result, 3.14)
        
        result = self.wp("0+3.14", rssdoc)
        self.assertEquals(result, 3.14)
        
        result = self.wp("0*42", rssdoc)
        self.assertEquals(result, 0)
        
        result = self.wp("3.14 * 2", rssdoc)
        self.assertEquals(result, 6.28)
        
        result = self.wp("42 div 7", rssdoc)
        self.assertEquals(result, 6)
        
        result = self.wp("3 + 2 * 4", rssdoc)
        self.assertEquals(result, 11)
        
        result = self.wp("(3 + 2) * 4", rssdoc)
        self.assertEquals(result, 20)
       
        result = self.wp("true() and false()",rssdoc)
        self.assertEquals(result, False)
      
        result = self.wp("true() or false()",rssdoc)
        self.assertEquals(result, True)
         
        # now some real tests...
        result = self.wp(".", rssdoc)
        self.assertEquals(len(result), 1)
        self.assertEquals(result[0].nodeName, "rss")
        
        result = self.wp("/", rssdoc)
        self.assertEquals(len(result), 1)
        self.assertEquals(result[0].nodeName, "#document")
        
        result = self.wp("/*", rssdoc)
        self.assertEquals(result[0].nodeName, "rss")
        
        result = self.wp("string(channel/@spam)", rssdoc)
        self.assertEquals(result, "eggs")
        
        result = self.wp("string-length(channel/@spam)", rssdoc)
        self.assertEquals(result, 4)

        result = self.wp("child::channel/attribute::spam", rssdoc)
        # should return a list of 1 Attr instance
        self.assertEquals(len(result), 1)
        self.assertEquals(result[0].nodeName, "spam")        
        #again, with abbreviated syntax
        result = self.wp("channel/@spam", rssdoc)
        self.assertEquals(len(result), 1)
        self.assertEquals(result[0].nodeName, "spam")
        
        result = self.wp("self::rss", rssdoc)
        self.assertEquals(len(result), 1)
        self.assertEquals(result[0].nodeName, "rss")
        
        result = self.wp("descendent::item", rssdoc)
        self.assertEquals(len(result), 2)
        self.assertEquals(result[0].nodeName, "item")
        self.assertEquals(result[1].nodeName, "item")
        
        result = self.wp("descendent-or-self::item", rssdoc)
        self.assertEquals(len(result), 2)
        self.assertEquals(result[0].nodeName, "item")
        self.assertEquals(result[1].nodeName, "item")

        result = self.wp("child::node()", rssdoc)
        self.assertEquals(len(result), 1)
        self.assertEquals(result[0].nodeName, "channel")
        
        result = self.wp("a/child::text()", linkdoc)
        # this returns a text node
        self.assertEquals(len(result), 1)
        self.assertEquals(result[0].nodeType, 3)
        self.assertEquals(result[0].nodeValue, "click")

        result = self.wp("a/text()", linkdoc)
        # this returns a text node
        self.assertEquals(len(result), 1)
        self.assertEquals(result[0].nodeType, 3)
        self.assertEquals(result[0].nodeValue, "click")
        
        result = self.wp("/descendent-or-self::item", rssdoc)
        self.assertEquals(len(result), 2)

        result = self.wp("//*", ids)
        self.assertEquals(len(result), 5)
        
        result = self.wp("/r/child/gc", ids)
        self.assertEquals(len(result), 1)

        result = self.wp("/r/child/gc[@id]", ids)
        self.assertEquals(len(result), 1)

        result = self.wp("/r/child/gc[contains(@id, 'h')]", ids)
        self.assertEquals(len(result), 1)

        # todo: shouldn't need the string() call here...
        result = self.wp("/r/child/gc[@id = 'ham']", ids)
        self.assertEquals(len(result), 1)

        result = self.wp("node()", rssdoc)
        self.assertEquals(len(result), 1)
        # !!! Issue: should the implied child:: axis apply to
        #            a straight function call like this?
        self.assertEquals(result[0].nodeName, "rss")
        
        result = self.wp("//item", rssdoc)
        self.assertEquals(len(result), 2)
        self.assertEquals(result[0].nodeName, "item")
        self.assertEquals(result[1].nodeName, "item")

        result = self.wp("*/*", rssdoc)
        self.assertEquals(len(result), 2)
        self.assertEquals(result[0].nodeName, "item")
        self.assertEquals(result[1].nodeName, "item")

        result = self.wp("*/*[true()]", rssdoc)
        self.assertEquals(len(result), 2)
        
        result = self.wp("*/*[false()]", rssdoc)
        self.assertEquals(result, [])
        
        result = self.wp("*/*[2]", rssdoc) # shortcut predicate
        self.assertEquals(len(result), 1)  # note: these are 1-based
        
        result = self.wp("*/*[3]", rssdoc)
        self.assertEquals(result, [])
        
        result = self.wp("*/*[string-length('')]", rssdoc)
        self.assertEquals(result, [])
        
        result = self.wp("*/*[string-length('a')]", rssdoc)
        # the shortcut should only work for literal numbers
        # i.e. this is NOT equiv to */*[1]
        self.assertEquals(len(result), 2)
        
        result = self.wp("*/*['spam']", rssdoc)
        # weird, but legal. Check to make sure conversion to bool works
        self.assertEquals(len(result), 2)
        
        result = self.wp("a | b", uniondoc)
        self.assertEquals(len(result), 2)
        self.assertEquals(result[0].nodeName, "a")
        self.assertEquals(result[1].nodeName, "b")
        
        # union expressions even work as node steps
        result = self.wp("/r/(a|b)/*", uniondoc)
        self.assertEquals(len(result), 2)
        self.assertEquals(result[0].nodeName, "x")
        self.assertEquals(result[1].nodeName, "y") 
        
        # string match        
        result = self.wp("//*[contains(.,'c')]", linkdoc)
        # this matches <root>, <a>, and the text node itself
        self.assertEquals(len(result), 3)
        self.assertEquals(result[0].nodeName, "root")
        self.assertEquals(result[1].nodeName, "a")
        self.assertEquals(result[2].nodeName, "#text")
        
        # HTML-specific extension axis
        html1 = parseString("<tr><td><b><strong><font>Text!</font><div>not</div></strong></b></td></tr>")
        result = self.wp("/tr/td/inline-descendent::*", html1)
        self.assertEquals(len(result), 4) # three elements, 1 text
        
        result = self.wp("/tr/td/inline-descendent::text()", html1)
        self.assertEquals(len(result), 1)
        self.assertEquals(result[0].nodeValue, "Text!")
        
        # make sure the search breaks off appropriately
        html2 = parseString("<tr><td><b>NYT</b></td><td><table><strong>not!</strong></table></td></tr>")
        result = self.wp("/tr/td/inline-descendent::text()", html2)
        self.assertEquals(len(result), 1)
        self.assertEquals(result[0].nodeValue, "NYT")
        

class TestTraverse(unittest.TestCase):
    "test get() and traverse::, along with URL caching"
    
    def testHttpRequests(self):
        print "testHttpRequests..."
        
        linkdoc2 = parseString("<root><a href='http://dubinko.info'>click</a><a href='http://dubinko.info/blog'>here</a></root>")

        # these tests share a context (where the cached docs live)
        ctxt = WebPathContext([linkdoc2.documentElement])
        
        query = "child::a[1]/attribute::href/traverse::*"
        t = parse_it_baby(toks(query))
        result = webpath(t, ctxt)
        # should return the html element from the linked doc
        # this test may fail if network is unavailable, etc.
        self.assertEquals(len(result), 1)
        self.assertEquals(result[0].nodeName, "html")
        self.assertEquals(len(ctxt.cachedocs), 1)
        self.assertEquals(ctxt.cachedocs.has_key("http://dubinko.info"), True)
        
        query = "get(a/@href)/head/title"
        t = parse_it_baby(toks(query))
        result = webpath(t, ctxt)
        # this branches out and fetches both hrefs
        self.assertEquals(len(result), 2)
        self.assertEquals(result[0].nodeName, "title")
        self.assertEquals(result[1].nodeName, "title")
        # these are titles from _different_ documents, right?
        self.assert_(result[0].firstChild.nodeValue != result[1].firstChild.nodeValue)
        # and a 2nd doc is now cached, right?
        self.assertEquals(len(ctxt.cachedocs), 2)
        self.assertEquals(ctxt.cachedocs.has_key("http://dubinko.info"), True)
        self.assertEquals(ctxt.cachedocs.has_key("http://dubinko.info/blog"), True)
        
        query = "get('http://dubinko.info')/head/title"
        t = parse_it_baby(toks(query))
        result = webpath(t, ctxt)
        self.assertEquals(len(result), 1)
        self.assertEquals(result[0].nodeName, "title")
        
        # full test. Find a rottentomatoes.com link, follow it, extract data
        #result = self.wp("get('http://movies.yahoo.com/movie/1808405878/web')//a[contains(@href, 'rottentomatoes.com')]/get(@href)//*[@id='critics_tomatometer_score_txt']", linkdoc)
        #print "Movie is rated", result[0].firstChild.nodeValue, "!!!!!!"
        #self.assertEquals(len(result), 1)
        
class TestHTMLExtensions(unittest.TestCase):
    "Test HTML-specific extensions"
    
    def wp(self, expr, doc):
        "execute a WebPath expr. Use documentElement as context"
        tree = parse_it_baby(toks(expr))
        ctxt = WebPathContext([doc.documentElement])
        return webpath(tree, ctxt)        
        
    def testHTMLStuff(self):
        html1 = parseString("""<html>
        <head><title>Test Me</title></head>
        <body>
          <h1>Heading</h1>
          <p>
          Some <i>very</i> traditional HTML
          </p>
          <h2>Heading 2</h2>
            <table>
              <tr>
                <td>... some nonsemantic layout stuff...</td>
                <td>
                  <table>
                    <tr>
                      <td>Actual Data!</td>
                      <td>Widget Cost</td>
                      <td>$9.09</td>
                    </tr>
                  </table>
                </td>
                <td>...more nonsemantic layout stuff...</td>
              </tr>
            </table>
        </body>
        </html>""")
        
        result = self.wp("normalize-space(/html/body/p)", html1)
        #self.assertEquals(result, "Some very interesting traditional HTML")
        # above not working, disadvantgae of breadth-first treewalk :(
        
        result = self.wp("inline-text(/html/body/p)", html1)
        #self.assertEquals(result, "Some very interesting traditional HTML")
        # above not working, disadvantgae of breadth-first treewalk :(
        
        # this function useful for finding un-nested tables...
        result = self.wp("//tr[contains(td/inline-text(), 'Actual Data')]", html1)
        self.assertEquals(len(result), 1)
        
        
class TestContext(unittest.TestCase):
    "test unusual configurations of the WebPathContext"
    
    def wpc(self, expr, doc, varsdict):
        "execute a WebPath expr. Use documentElement and vars as context"
        tree = parse_it_baby(toks(expr))
        ctxt = WebPathContext([doc.documentElement])
        ctxt.varbind = varsdict
        return webpath(tree, ctxt)        

    def testContext(self):
        print "testContext..."
        simpdoc = parseString("<a><b id='x'/><b id='y'/><b id='z'/></a>")
        
        result = self.wpc("$foo", simpdoc, {"foo": "testing123" })
        self.assertEquals(result, "testing123")
        
        result = self.wpc("get($start)/head/title", simpdoc, {"start": "http://dubinko.info"})
        self.assertEquals(len(result), 1)
        self.assertEquals(result[0].nodeName, "title")
        
        # variables should work as strings,
        result = self.wpc("string-length($start)", simpdoc, {"start": "http://dubinko.info"})
        self.assertEquals(result, 19)
        
        # as numbers,
        result = self.wpc("$num + 3", simpdoc, {"num": 4})
        self.assertEquals(result, 7)
        
        # and as node-lists
        intermed = self.wpc("b", simpdoc, {})
        self.assertEquals(len(intermed), 3)
        self.assertEquals(intermed[0].nodeName, "b")
        self.assertEquals(intermed[1].nodeName, "b")
        self.assertEquals(intermed[2].nodeName, "b")
        
        result = self.wpc("$test/@id", simpdoc, {"test" : intermed})
        self.assertEquals(len(result), 3)
        self.assertEquals(result[0].nodeValue, "x")
        self.assertEquals(result[1].nodeValue, "y")
        self.assertEquals(result[2].nodeValue, "z")

class TestSequences(unittest.TestCase):
    "experimental support for arbitrary sequences"
    def wp(self, expr, doc):
        "execute a WebPath expr. Use documentElement as context"
        tree = parse_it_baby(toks(expr))
        ctxt = WebPathContext([doc.documentElement])
        return webpath(tree, ctxt)        

    def testSequences(self):
        print "testSequences..."
        doc1 = parseString("<a foo='bar'><b/><b/><b/></a>")
        doc2 = parseString("<_><item pr='1.23' quan='4'/><item pr='0.99' quan='1'/></_>")
        
        result = self.wp("seq()", doc1)
        self.assertEquals(result, [])
        
        result = self.wp("seq(3,4)", doc1)
        self.assertEquals(len(result), 2)
        self.assertEquals(result[0], 3)
        self.assertEquals(result[1], 4)
        
        result = self.wp("seq(/,self::a,self::a/b)", doc1)
        self.assertEquals(len(result), 5)
        self.assertEquals(result[0], doc1)
        self.assertEquals(result[1].nodeName, "a")
        self.assertEquals(result[2].nodeName, "b")
        # flattened sequence
        self.assertEquals(result[3].nodeName, "b")
        self.assertEquals(result[4].nodeName, "b")
        
        result = self.wp("seq(1,2,3) = 3", doc1)
        self.assertEquals(result, True)
        
        result = self.wp("seq(1,2,3) = 4", doc1)
        self.assertEquals(result, False)
        
        result = self.wp("seq(1,2,3) != 3", doc1)
        self.assertEquals(result, True)
        
        result = self.wp("seq(1,2,3) != 4", doc1)
        self.assertEquals(result, True)
        
        result = self.wp("seq(3,3,3) != 3", doc1)
        self.assertEquals(result, False)
        
        result = self.wp("seq('spam', 'eggs', 'ham')", doc1)
        self.assertEquals(len(result), 3)
        self.assertEquals(result[0], "spam")
        self.assertEquals(result[1], "eggs")
        self.assertEquals(result[2], "ham")
        
        result = self.wp("seq('spam', 'eggs', 'ham') = 'ham'", doc1)
        self.assertEquals(result, True)
        
        result = self.wp("seq('spam', 'eggs', 'ham') = 'foo'", doc1)
        self.assertEquals(result, False)
        
        result = self.wp("seq('spam', 'eggs', 'ham') != 'ham'", doc1)
        self.assertEquals(result, True)
        
        result = self.wp("seq('spam', 'eggs', 'ham') != 'foo'", doc1)
        self.assertEquals(result, True)
        
        result = self.wp("seq('spam', 'spam', 'spam') != 'spam'", doc1)
        self.assertEquals(result, False)
        
        result = self.wp("seq(1,2,3) = seq(3,4,5)", doc1)
        self.assertEquals(result, True)
        
        result = self.wp("seq(1,2,3) = seq(5,6,7)", doc1)
        self.assertEquals(result, False)
        
        result = self.wp("range(1,9)", doc1)
        self.assertEquals(len(result), 9)
        self.assertEquals(result[6], 7)
        
        result = self.wp("range(1,99999) = 12345", doc1)
        self.assertEquals(result, True)
        
        result = self.wp("(1,2,3)", doc1)
        self.assertEquals(len(result), 3)
        
        # empty node sequence should collapse to nothing
        result = self.wp("(notmatch, 'N/A')", doc1)
        self.assertEquals(len(result), 1)
        self.assertEquals(result[0], "N/A")
        
        result = self.wp("(@foo, 'N/A')[1]", doc1)
        self.assertEquals(len(result), 1)
        self.assertEquals(result[0].nodeName, "foo")
        self.assertEquals(result[0].nodeValue, "bar")

        result = self.wp("(@xyz, 'N/A')[1]", doc1)
        self.assertEquals(len(result), 1)
        self.assertEquals(result[0], "N/A")
        
        # stuff to get working next...
        #result = self.wp("item/(@pr * @quan)", doc2)
        
        #result = self.wp("sum(item/@pr)", doc2)
        #print result
        
        
# the following 2 functions already defined in Python 2.5+
def all(iterable):
    for element in iterable:
        if not element:
            return False
    return True
    
def any(iterable):
    for element in iterable:
        if element:
            return True
    return False
    
def main():    
    unittest.main()
    
# To test just the lexer...
#if __name__ == '__main__':
#     lex.runmain()

if __name__ == "__main__":
    main()
