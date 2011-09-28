# Copyright (c) 2008 Yahoo! Inc.  All rights reserved.
# The copyrights embodied in the content of this file are licensed
# by Yahoo! Inc. under the BSD (revised) open source license


# if you get errors with this demo, be sure you have Python Lex-Yacc (ply) installed

# Usage: from a python interactive prompt
# >>> import demo
# >>> demo.demo1()
# >>> demo.demo2()
# etc.

import webpath
from xml.dom.minidom import parse, parseString,Node

markup = u"""<root>
  <a href='http://dubinko.info'>click me 1</a>
  <a href='http://xformsinstitute.com'>no, click me!</a>
</root>"""
doc = parseString(markup)

sharedctx = webpath.WebPathContext([doc.documentElement])

def reset():
    sharedctx = webpath.WebPathContext([doc.documentElement])

def wp(expr, doc = doc, ctxt = sharedctx):
    "execute a WebPath expr. Use documentElement as context"
    tree = webpath.parse_it_baby(webpath.toks(expr))
    return webpath.webpath(tree, ctxt)        

def demo(expr):
    print "Expression:", expr
    result = wp(expr)
    print "Result====>", result

def demo1():
    demo(u"(1,'two', 2*2, count(//*))")
    
def demo2():
    demo(u"//a")
    pass
    
def demo3():
    demo(u"get('http://dubinko.info')")

def demo4():
    demo(u"get(//a/@href)")

def demo5():
    demo(u"//a/@href/traverse::*")

def demo6():
    demo(u"string(//a[contains(.,'1')]/get(@href)//a[contains(.,'Grok')]/get(@href)/head/title)")

