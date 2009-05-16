#!/usr/bin/env python
# -*- coding: utf-8 -*-

__version__ = "0.3.6"
__authors__ = ["Sylvain Hellegouarch (sh@defuze.org)"]
__contributors__ = ['David Turner']
__date__ = "2008/08/03"
__copyright__ = """
Copyright (c) 2006, 2007, 2008 Sylvain Hellegouarch
All rights reserved.
"""
__license__ = """
Redistribution and use in source and binary forms, with or without modification, 
are permitted provided that the following conditions are met:
 
     * Redistributions of source code must retain the above copyright notice, 
       this list of conditions and the following disclaimer.
     * Redistributions in binary form must reproduce the above copyright notice, 
       this list of conditions and the following disclaimer in the documentation 
       and/or other materials provided with the distribution.
     * Neither the name of Sylvain Hellegouarch nor the names of his contributors 
       may be used to endorse or promote products derived from this software 
       without specific prior written permission.
 
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE 
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
"""

ENCODING = 'UTF-8'
DUMMY_URI = u'http://dummy.com'

from bridge.filter import fetch_child, fetch_children
from bridge.common import  XML_NS, XMLNS_NS 

__all__ = ['Attribute', 'Element', 'PI', 'Comment', 'Document']

class PI(object):
    def __init__(self, target, data, parent=None):
        self.target = target
        self.data = data
        self.xml_parent = parent
    
        if self.xml_parent:
            self.xml_parent.xml_children.append(self)

class Comment(object):
    def __init__(self, data, parent=None):
        self.data = data
        self.xml_parent = parent

        if self.xml_parent:
            self.xml_parent.xml_children.append(self)
          
class Attribute(object):
    """
    Maps the attribute of an XML element to a simple Python object.
    """

    encoding = ENCODING
    as_attribute_of_element = None
    
    def __init__(self, name=None, value=None, prefix=None, namespace=None, parent=None):
        """
        Maps the attribute of an XML element to a simple Python object.

        Keyword arguments:
        name -- Name of the attribute
        value -- content of the attribute
        prefix -- XML prefix of the element
        namespace -- XML namespace defining the prefix
        parent -- element which this attribute belongs to
        """
        if value and not isinstance(value, unicode):
            raise TypeError, "Attribute's (%s) value must be an unicode object or None. Got %s instead." % (name, str(type(value)))
        
        self.xml_parent = parent
        self.xml_ns = namespace
        self._local_name = name
        if name:
            name = name.replace('-', '_').replace('.', '_')
        self.xml_name = name
        self.xml_text = value
        self.xml_prefix = prefix
        self.xml_ns = namespace

        self.as_attribute_of_element = {}
        if self.xml_parent and self.xml_parent.xml_root.as_attribute_of_element:
            self.as_attribute_of_element.update(self.xml_parent.xml_root.as_attribute_of_element)
        elif isinstance(Attribute.as_attribute_of_element, dict):
            self.as_attribute_of_element.update(Attribute.as_attribute_of_element)
            
        if self.xml_parent:
            self.xml_parent.xml_attributes.append(self)

            attrs = self.as_attribute_of_element.get(self.xml_ns, [])
            if self.xml_name in attrs:
                if not hasattr(self.xml_parent, name):
                    setattr(self.xml_parent, name, self.xml_text)

    def __unicode__(self):
        if self.xml_text:
            return self.xml_text
        return unicode(self.xml_text)
    
    def __str__(self):
        if self.xml_text:
            return self.xml_text.encode(self.encoding)
        return str(self.xml_text)
    
    def __repr__(self):
        value = self.xml_text or ''
        return '{%s}%s="%s" attribute at %s' % (self.xml_ns or '', self.xml_name, value, hex(id(self)))
  
class Element(object):
    """
    Maps an XML element to a Python object.
    """

    parser = None
    encoding = ENCODING
    as_list = None
    as_attribute = None
    
    def __init__(self, name=None, content=None, attributes=None, prefix=None, namespace=None, parent=None):
        """
        Maps an XML element to a Python object.
        
        Keyword arguments:
        name -- Name of the XML element
        content -- Content of the element
        attributes -- dictionary of the form {local_name: value}
        prefix -- XML prefix of the element
        namespace -- XML namespace attached to that element
        parent -- Parent element of this element.
        
        If 'parent' is not None, 'self' will be added to the parent.xml_children

        If 'Element.as_list' is set and if (name, namespace) belongs to it
        then we will add a list to parent with the name of the element
        
        If 'Element.as_attribute' is set and if (name, namespace) belongs to it
        then we will add an attribute to parent with the name of the element
        """
        if content and not isinstance(content, unicode):
            raise TypeError, "Element's content (%s) must be an unicode object or None. Got %s instead." % (name, str(type(content)))

        self.as_attribute = {}
        self.as_list= {}
        self.as_attribute_of_element = {}

        self._root = None
        self.xml_parent = parent
        self.xml_prefix = prefix
        self.xml_ns = namespace
        self._local_name = name
        if name:
            name = name.replace('-', '_').replace('.', '_')
        self.xml_name = name
        self.xml_text = content
        self.xml_children = []
        self.xml_attributes = []
        self.as_cdata = False

        if self.xml_root and self.xml_root.as_attribute:
            self.as_attribute.update(self.xml_root.as_attribute)
        elif isinstance(Element.as_attribute, dict):
            self.as_attribute.update(Element.as_attribute)

        if self.xml_root and self.xml_root.as_list:
            self.as_list.update(self.xml_root.as_list)
        elif isinstance(Element.as_list, dict):
            self.as_list.update(Element.as_list)

        if self.xml_root and self.xml_root.as_attribute_of_element:
            self.as_attribute_of_element.update(self.xml_root.as_attribute_of_element)

        if self.xml_parent:
            self.xml_parent.xml_children.append(self)

            as_attr_elts = self.as_attribute.get(self.xml_ns, [])
            as_list_elts = self.as_list.get(self.xml_ns, [])

            if self.xml_name in as_attr_elts:
                setattr(self.xml_parent, name, self)
            elif self.xml_name in as_list_elts:
                if not hasattr(self.xml_parent, name):
                    setattr(self.xml_parent, name, [])
                els = getattr(self.xml_parent, name)
                els.append(self)

        if attributes and isinstance(attributes, dict):
            for name in attributes:
                Attribute(name, attributes[name], parent=self)

    def __repr__(self):
        prefix = self.xml_prefix
        xmlns = self.xml_ns
        if (prefix not in ('', None)) and xmlns:
            return '<%s:%s xmlns:%s="%s" element at %s />' % (prefix, self.xml_name,
                                                              prefix, xmlns, hex(id(self)),)
        else:
            return "<%s element at %s />" % (self.xml_name, hex(id(self)))

    def __unicode__(self):
        if self.xml_text:
            return self.xml_text
        return unicode(None)
    
    def __str__(self):
        if self.xml_text:
            return self.xml_text.encode(self.encoding)
        return str(None)

    def __iter__(self):
        return iter(self.xml_children)

    def __copy__(self):
        return Element.load(self.xml(encoding=self.encoding, omit_declaration=True))

    def clone(self):
        return Element.load(self.xml(encoding=self.encoding, omit_declaration=True),
                            as_attribute=self.as_attribute, as_list=self.as_list,
                            as_attribute_of_element=self.as_attribute_of_element)
        
    def __delattr__(self, name):
        """
        deletes 'name' instance of Element. It will also removes it
        from its parent children and attributes.
        """
        if not hasattr(self, name):
            raise AttributeError, name
        
        attr = getattr(self, name)
        if isinstance(attr, Element):
            if attr in self.xml_children:
                self.xml_children.remove(attr)
        elif isinstance(attr, Attribute):
            if attr in self.xml_attributes:
                self.xml_attributes.remove(attr)

        del self.__dict__[name]

    def get_root(self):
        if self._root is not None:
            return self._root

        if isinstance(self.xml_parent, Document):
            self._root = self
            self.as_attribute.update(self.xml_parent.as_attribute)
            self.as_list.update(self.xml_parent.as_list)
            self.as_attribute_of_element.update(self.xml_parent.as_attribute_of_element)
            return self
        
        if self.xml_parent is None:
            self._root = self
            return self
        return self.xml_parent.get_root()
    xml_root = property(get_root, doc="Retrieve the top level element")

    def get_attribute(self, name):
        for attr in self.xml_attributes:
            if attr.xml_name == name:
                return attr
            
    def get_attribute_value(self, name, default=None):
        for attr in self.xml_attributes:
            if attr.xml_name == name:
                return unicode(attr)
        return default
    
    def set_attribute_value(self, name, value):
        """Sets the attribute value. If the attribute does not
        exist it is created and set"""
        found = False
        for attr in self.xml_attributes:
            if attr.xml_name == name:
                attr.xml_text = value
                found = True
                break

        if not found:
            Attribute(name, value, parent=self)
            
    def get_attribute_ns(self, name, namespace):
        for attr in self.xml_attributes:
            if (attr.xml_name == name) and (attr.xml_ns == namespace):
                return attr

    def get_attribute_ns_value(self, name, namespace, default=None):
        for attr in self.xml_attributes:
            if (attr.xml_name == name) and (attr.xml_ns == namespace):
                return unicode(attr)
        return default
    
    def set_attribute_ns_value(self, name, value, namespace=None, prefix=None):
        """Sets the attribute value in the given namespace. If the attribute does not
        exist it is created and set"""
        found = False
        for attr in self.xml_attributes:
            if (attr.xml_name == name) and (attr.xml_ns == namespace):
                attr.xml_text = value
                found = True
                break

        if not found:
            Attribute(name, value, prefix=prefix, namespace=namespace, parent=self)
            
    def has_element(self, name, ns=None):
        """
        Checks if this element has 'name' attribute

        Keyword arguments:
        name -- local name of the element
        ns -- namespace of the element
        """
        obj = getattr(self, name, None)
        if obj:
            return obj.xml_ns == ns
        return False

    def has_child(self, name, ns=None):
        """
        Checks if this element has a child named 'name' in its children elements

        Keyword arguments:
        name -- local name of the element
        ns -- namespace of the element
        """
        return self.filtrate(fetch_child, child_name=name, child_ns=ns) != None
    
    def get_child(self, name, ns=None):
        """
        Returns the child element named 'name', None if not found.

        Keyword arguments:
        name -- local name of the element
        ns -- namespace of the element
        """
        return self.filtrate(fetch_child, child_name=name, child_ns=ns)
    
    def get_children(self, name, ns=None, recursive=False):
        """
        Returns the all children of this element named 'name'
        
        Keyword arguments:
        name -- local name of the element
        ns -- namespace of the element
        recursive -- if True this will iterate through the entire tree
        """
        return self.filtrate(fetch_children, child_name=name, child_ns=ns, recursive=recursive)

    def get_children_without(self, types=None):
        """
        Returns a list of children not belonging to the types passed in
        ``types`` which must be a list or None. If None is passed
        then returns self.xml_children.

        feed.get_children_without(types=[str, Comment])

        This will return all the children which are not of type string or bridge.Comment.
        """
        if not types:
            return self.xml_children

        children = []
        for child in self.xml_children:
            keep = True
            for t in types:
                if isinstance(child, t):
                    keep = False
                    break

            if keep:
                children.append(child)

        return children
    
    def get_children_with(self, types=None):
        """
        Returns a list of children belonging only to the types passed in
        ``types`` which must be a list or None. If None is passed
        then returns self.xml_children.

        feed.get_children_with(types=[Element])

        This will return all the children which are of type bridge.Element
        """
        if not types:
            return self.xml_children

        children = []
        for child in self.xml_children:
            keep = False
            for t in types:
                if isinstance(child, t):
                    keep = True
                    break

            if keep:
                children.append(child)

        return children
    
    def forget(self):
        """
        Deletes this instance of Element. It will also removes it
        from its parent children and attributes.
        """
        if self.xml_parent:
            self.remove_from(self.xml_parent)

    def remove_from(self, element):
        """
        Removes the instance from the element parameter provided.
        """
        if self in element.xml_children:
            element.xml_children.remove(self)
        if hasattr(element, self.xml_name):
            obj = getattr(element, self.xml_name)
            if isinstance(obj, list):
                obj.remove(self)
            elif isinstance(obj, Element):
                del obj
        
    def insert_before(self, before_element, element):
        """
        Insert 'element' right before 'before_element'.
        This only inserts the new element in self.xml_children

        Keyword arguments:
        before_element -- element pivot
        element -- new element to insert
        """
        self.xml_children.insert(self.xml_children.index(before_element), element)

    def insert_after(self, after_element, element):
        """
        Insert 'element' right after 'after_element'.
        This only inserts the new element in self.xml_children

        Keyword arguments:
        after_element -- element pivot
        element -- new element to insert
        """
        self.xml_children.insert(self.xml_children.index(after_element) + 1, element)

    def replace(self, current_element, new_element):
        """
        replaces the current element with a new element in the list
        of children.
        
        Keyword arguments:
        current_element -- element pivot
        new_element -- new element to insert
        """
        self.xml_children[self.xml_children.index(current_element)] = new_element

    def collapse(self, separator='\n'):
        """
        Collapses all content of this element and its entire subtree.
        """
        text = [self.xml_text or '']
        for child in self.xml_children:
            if isinstance(child, unicode) or isinstance(child, str):
                text.append(child)
            elif isinstance(child, Element):
                text.append(child.collapse(separator))

        return separator.join(text)

    def is_mixed_content(self):
        """
        Returns True if the direct children of this element makes are
        in mixed content.
        """
        for child in self.xml_children:
            if isinstance(child, unicode) or isinstance(child, str):
                return True

        return False

    def xml(self, indent=True, encoding=ENCODING, prefixes=None, omit_declaration=False, parser=None):
        """
        Serializes as a string this element

        Keyword arguments
        indent -- pretty print the XML string (defaut: True)
        encoding -- encoding to use during the serialization process
        prefixes -- dictionnary of prefixes of the form {'prefix': 'ns'}
        omit_declaration -- prevent the result to start with the XML declaration
        parser -- instance of an existing parser, if not provided the default one will be used
        """
        if not parser and self.parser:
            parser = self.parser()
        elif not self.parser:
            from bridge.parser import get_first_available_parser
            parser = get_first_available_parser()
            self.parser = parser
            parser = parser()

        return parser.serialize(self, indent=indent, encoding=encoding,
                                prefixes=prefixes, omit_declaration=omit_declaration)

    def load(self, source, prefixes=None, as_attribute=None, as_list=None,
             as_attribute_of_element=None, parser=None):
        """
        Load source into an Element instance

        Keyword arguments:
        source -- an XML string, a file path or a file object
        prefixes -- dictionnary of prefixes of the form {'prefix': 'ns'}
        as_attribute -- dictionary of element names to set as attribute of their parent
        as_list -- dictionary of element names to set into a list of their parent
        as_attribute_of_element -- dictionary of attribute names to set as attribute of their
        parent

        If any of those last three parameters are provided they will take
        precedence over those set on the Element and Attribute class.
        """
        if not parser and self.parser:
            parser = self.parser()
        elif not self.parser:
            from bridge.parser import get_first_available_parser
            parser = get_first_available_parser()
            self.parser = parser
            parser = parser()

        return parser.deserialize(source, prefixes=prefixes, as_attribute=as_attribute,
                               as_list=as_list, as_attribute_of_element=as_attribute_of_element)
    load = classmethod(load)

    def __update_prefixes(self, element, dst, srcns, dstns, update_attributes):
        if update_attributes:
            for attr in element.xml_attributes:
                if attr.xml_ns == srcns:
                    attr.xml_prefix = dst
                    attr.xml_ns = dstns
                elif attr.xml_ns == XMLNS_NS:
                    attr.xml_name = dst

        if element.xml_ns == srcns:
            element.xml_prefix = dst
            if element.xml_ns and not dstns:
                element.xml_ns = None
            elif not element.xml_ns:
                element.xml_ns = dstns
        
        for child in element.xml_children:
            if isinstance(child, Element):
                self.__update_prefixes(child, dst, srcns, dstns, update_attributes)
                
    def update_prefix(self, dst, srcns, dstns, update_attributes=True):
        """
        Updates prefixes of all the element of document matching (src, srcns)

        Keyword arguments:
        dst -- new prefix to be used
        srcns -- source namespace
        dstns -- destination namespace
        update_attributes -- update attributes' namespace as well (default: True)
        """
        self.__update_prefixes(self, dst, srcns, dstns, update_attributes)

    def filtrate(self, some_filter, **kwargs):
        """
        Applies a filter to this element. Returns what is returned from
        some_filter.

        Keyword arguments:
        some_filter -- a callable(**kwargs)
        """
        return some_filter(element=self, **kwargs)

    def validate(self, validator, **kwargs):
        """
        Applies a validator on this element
        """
        validator(self, **kwargs)

class Document(Element):
    def __init__(self):
        Element.__init__(self)
        
    def get_root(self):
        if self._root is None:
            for child in self.xml_children:
                if isinstance(child, Element):
                    self._root = child
                    break
        return self._root
    xml_root = property(get_root)

    def __repr__(self):
        return "document at %s" % hex(id(self))
