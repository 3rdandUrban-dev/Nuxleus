# Copyright (c) 2008 Yahoo! Inc.  All rights reserved.
# The copyrights embodied in the content of this file are licensed
# by Yahoo! Inc. under the BSD (revised) open source license


import platonicweb

def child(nodelist):
    results = []
    for node in nodelist:
        results += node.childNodes
    return results

def preceding_sibling(nodelist):
    results = []
    for node in nodelist:
        parent = node.parentNode
        if parent is None:
            continue
        children = parent.childNodes
        for child in children:
            if child==node:
                break
            results += [child]
    return results           

def following_sibling(nodelist):
    results = []
    past_match = False
    for node in nodelist:
        parent = node.parentNode
        if parent is None:
            continue
        children = parent.childNodes
        for child in children:
            if past_match is False and child==node:
                past_match = True
            if past_match is True and child!=node:
                results += [child]
    return results

def attribute(nodelist):
    results = []
    for node in nodelist:
        if node.attributes is None: continue
        if node.attributes.length == 0: continue
        for idx in range(node.attributes.length):
            results.append(node.attributes.item(idx))
    return results

def self(nodelist):
    return nodelist
    
def descendent(nodelist, filt=None):
    results = []
    nl = nodelist
    while len(nl):
        layer = child(nl)
        layer = filter(filt, layer)
        results += layer
        nl = layer
    return results

def descendent_or_self(nodelist):
    return self(nodelist) + descendent(nodelist)

def traverse(nodelist, ctxt):
    urllist = [u.nodeValue for u in nodelist]
    return fetchDocs(urllist, ctxt)
    
def inline_descendent(nodelist):
    inline = ("b", "i", "font", "center", "span", "strong", "emph")
    return descendent(nodelist, lambda n: n.nodeName in inline or n.nodeType==3)

    
def fetchDocs(urllist, ctxt):
    results = []
    for url in urllist:
        if ctxt.cachedocs.has_key(url):
            results.append(ctxt.cachedocs[url].documentElement)
            continue
        dom = platonicweb.get(url)
        if dom:
            ctxt.cachedocs[url] = dom
            print "just added to cache:", url
            results.append(dom.documentElement)
        else:
            print "Failed to get document!", url
    return results

