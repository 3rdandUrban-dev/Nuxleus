#!/usr/bin/env python
# -*- coding: utf-8 -*-

__all__ = ['remove_duplicate_namespaces_declaration',
           'remove_useless_namespaces_decalaration',
           'fetch_child', 'fetch_children', 'element_children', 'lookup']

import re

import bridge
from bridge.common import XMLNS_NS

def fetch_child(element, child_name, child_ns):
    """
    Returns the first child named 'child_name' with the namespace 'child_ns'

    Use it like this:
    e = Element.load('<root><id /></root>')
    child = e.filtrate(fetch_child, child_name='id', child_ns=None)

    Keyword arguments:
    element -- parent element to go through
    child_name -- name of the element to lookup
    child_ns -- namespace of the element to lookup
    """
    for child in element.xml_children:
        if isinstance(child, bridge.Element):
            if child.xml_ns == child_ns:
                if child.xml_name == child_name:
                    return child

    return None

def fetch_children(element, child_name, child_ns, recursive=False):
    """
    Returns the list of children named 'child_name' with the namespace 'child_ns'

    Use it like this:
    e = Element.load('<root><node /></node /></root>')
    children = e.filtrate(fetch_children, child_name='node', child_ns=None)
    
    Keyword arguments:
    element -- parent element to go through
    child_name -- name of the element to lookup
    child_ns -- namespace of the element to lookup
    """
    children = []
    element_type = type(element)
    for child in element.xml_children:
        if isinstance(child, element_type):
            if child.xml_ns == child_ns:
                if child.xml_name == child_name:
                    children.append(child)
            if recursive:
                sub_children = fetch_children(child, child_name, child_ns, True)
                children.extend(sub_children)
            
    return children

def remove_useless_namespaces_decalaration(element):
    """
    Will recursuvely go through all the elements of a fragment
    and remove duplicate XML namespace declaration

    Keyword arguments:
    element -- root element to start from
    """
    attrs = element.xml_attributes[:]
    for attr in attrs:
        if (attr.xml_name == element.xml_prefix) and \
           (attr.xml_text == element.xml_ns):
            continue
        if attr.xml_ns == XMLNS_NS:
            element.xml_attributes.remove(attr)
    attrs = None
    for child in element_children(element):
        remove_useless_namespaces_decalaration(child)

def remove_duplicate_namespaces_declaration(element, visited_ns=None):
    """
    Will recursuvely go through all the elements of a fragment
    and remove duplicate XML namespace declaration

    Keyword arguments:
    element -- root element to start from
    visited_ns -- list of already visited namespace
    """
    if visited_ns is None:
        visited_ns = []
    _visited_ns = visited_ns[:]
    attrs = element.xml_attributes[:]
    for attr in attrs:
        if attr.xml_ns == XMLNS_NS:
            if attr.xml_text in visited_ns:
                element.xml_attributes.remove(attr)
            else:
                _visited_ns.append(attr.xml_text)
    attrs = None
    for child in element_children(element):
        remove_duplicate_namespaces_declaration(child, _visited_ns)
    _visited_ns = None

def find_by_id(element, id):
    """
    Looks for an element having the provided 'id'
    into the children recursively.

    Returns the found element or None.
    """
    result = None
    for child in element.xml_children:
        if isinstance(child, bridge.Element):
            _id = child.get_attribute('id')
            if _id is not None:
                if _id.xml_text == id:
                    result = child
                    break
            result = find_by_id(child, id)
            if result is not None:
                break
            
    return result

def tokenize_path(path):
    def handle_ns(start):
        pos = path.find('}', start)
        end = path.find('/', pos)
        return end

    path = path.lstrip('.')
    start = 0
    length = len(path)
    while 1:
        if path[start] == '{':
            end = handle_ns(start)
        elif path[start] == '/':
            if path[start + 1] == '{':
                end = handle_ns(start)
            else:
                end = path.find('/', start + 1)
        else:
            end = path.find('/', start)
            
        if end != -1:
            yield path[start:end].strip('/')
        else:
            yield path[start:].strip('/')
            
        start = end

        if (end == length) or (end == -1):
            break

_namespace_simple_regex = re.compile('{(.*)}(\w*)')
_query_simple_regex = re.compile('(\w*)\[@(\w*)\=[\"|\'](\w*)[\"|\']\]')

def next_token(path):
    for token in tokenize_path(path):
        uri = None
        local_name = token
        match = _namespace_simple_regex.search(token)
        if match:
            uri, local_name = match.groups()
        match = _query_simple_regex.search(token)
        if match:
            yield (uri, match.group(1), match.group(2), match.group(3))
        else:
            yield (uri, local_name, None, None)

def lookup(element, path):
    """Perfoms a lookup of an element matching the path provided
    This path looks like an XPath query but that's where the
    comparison should stop. This is not an XPath engine.

    >>> from bridge import Element as E
    >>> from bridge.filter import lookup
    >>> e = E.load('<a:o xmlns:a="ui"><b h="gr"><c/></b></a:o>')
    >>> e.filtrate(lookup, path=u'/{ui}o/b[@h="gr"]/c')
    <c element at 0xb7c0c30cL />
    >>> e.filtrate(lookup, path=u'{ui}o/b[@h="gr"]/c')
    <c element at 0xb7c0c30cL />
    >>> e.filtrate(lookup, path=u'./{ui}o/b[@h="gr"]/c')
    <c element at 0xb7c0c30cL />

    If the ``path`` starts with a '/' the matching will be
    applied from the root element otherwise from the current
    element (like when it starts with './').

    If you need to provide a namespace it must be within {} right
    before the local name.

    Attribute matching is extremely simple and you can only match
    one attribute per branch.

    This ultimately returns the matching element or None.
    """
    start_at = element
    if path[0] == '/':
        if not isinstance(element, bridge.Document):
            start_at = bridge.Document()
            start_at.xml_children.append(element.xml_root)
        else:
            start_at = element
    
    found_match = None
    for (uri, local_name, attr_name, attr_value) in next_token(path):
        child = start_at.get_child(local_name, ns=uri)
        if child:
            if attr_name:
                attr = child.get_attribute(attr_name)
                if attr and attr.xml_text == attr_value:
                    found_match = start_at = child
                else:
                    found_match = None
                    break
            else:
                found_match = start_at = child
        else:
            found_match = None
            break

    return found_match
    

###################################################################
# For generator consumers
###################################################################

def element_children(element):
    """
    yields every direct bridge.Element child of 'element'
    """
    for child in element.xml_children:
        if isinstance(child, bridge.Element):
            yield child
    
