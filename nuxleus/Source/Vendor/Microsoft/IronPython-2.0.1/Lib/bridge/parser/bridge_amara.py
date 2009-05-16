#!/usr/bin/env python
# -*- coding: utf-8 -*-

import os
import os.path

import xml.sax as xs
import xml.sax.saxutils as xss
from xml.parsers import expat
import StringIO

import amara

__all__ = ['IncrementalParser', 'DispatchParser']

from bridge import ENCODING
from bridge.common import ANY_NAMESPACE
from bridge.parser.bridge_default import DispatchParser as DefaultDispatchParser
from bridge.parser.bridge_default import DispatchHandler as DefaultDispatchHandler

class IncrementalHandler(xss.XMLGenerator):
    def __init__(self, out, encoding=ENCODING):
        xss.XMLGenerator.__init__(self, out, encoding)
        self._root = amara.create_document()
        self._current_el = self._root
        self._current_level = 0

    def reset(self):
        del self._current_el
        del self._root
        self._current_el = None
        self._root = amara.create_document()
        self._current_el = self._root
        self._current_level = 0

    def startDocument(self):
        self._root = amara.create_document()
        self._current_el = self._root
        self._current_level = 0

    def processingInstruction(self, target, data):
        amara.bindery.pi_base(target, data)
        self._root.xml_append(pi)

    def startElementNS(self, name, qname, attrs):
        uri, local_name = name
        prefix = None
        if uri and uri in self._current_context:
            prefix = self._current_context[uri]
        if qname:
            e = self._root.xml_create_element(qname, ns=uri)
        else:
            e = self._root.xml_create_element(local_name, ns=uri)

        for name, value in attrs.items():
            (namespace, local_name) = name
            qname = attrs.getQNameByName(name)
            e.xml_set_attribute((qname, namespace), value)

        self._current_el.xml_append(e)
        self._current_el = e
        self._current_level = self._current_level + 1
        
    def endElementNS(self, name, qname):
        self._current_level = current_level = self._current_level - 1
        self._current_el = self._current_el.parentNode

    def characters(self, content):
        self._current_el.xml_children.append(content)

    def comment(self, data):
        self._current_el.xml_append_fragment('<!-- %s -->' % data)
        
    def startCDATA(self):
        pass

    def endCDATA(self):
        pass

    def startDTD(self, name, public_id, system_id):
        pass

    def endDTD(self):
        pass
    
    def doc(self):
        """Returns the root amara.root_base instance of the parsed
        document. You have to call the close() method of the
        parser first.
        """
        return self._root

class IncrementalParser(object):
    def __init__(self, out=None, encoding=ENCODING):
        self.parser = xs.make_parser()
        self.parser.setFeature(xs.handler.feature_namespaces, True)
        if not out:
            out = StringIO.StringIO()
        self.out = out
        self.handler = IncrementalHandler(self.out, encoding)
        self.parser.setContentHandler(self.handler)
        self.parser.setProperty(xs.handler.property_lexical_handler, self.handler)

    def feed(self, chunk):
        self.parser.feed(chunk)
        
    def reset(self):
        self.handler.reset()
        self.parser.reset()

class DispatchHandler(IncrementalHandler, DefaultDispatchHandler):
    def __init__(self, out, encoding='UTF-8'):
        """This handler allows the incremental parsing of an XML document
        while providing simple ways to dispatch at precise point of the
        parsing back to the caller.

        Here's an example:

        >>> from bridge.parser.bridge_amara import DispatchParser
        >>> p = DispatchParser()
        >>> def dispatch(e):
        ...     # e is an amara instance
        ...     print e.xml()
        ...
        >>> h.register_at_level(1, dispatch)
        >>> p.feed('<r')
        >>> p.feed('><b')
        >>> p.feed('/></r>')
        <?xml version="1.0" encoding="UTF-8"?>
        <b xmlns=""></b>
        
        Alternatively this can even be used as a generic parser. If you
        don't need dispatching you simply call ``disable_dispatching``.

        >>> from bridge.parser import DispatchParser
        >>> p = DispatchParser()
        >>> h.disable_dispatching()
        >>> p.feed('<r><b/></r>')
        >>> h.doc()
        <r element at 0xb7ca99ccL />
        >>> h.doc().xml()
        '<r><b></b></r>'

        Note that this handler has limitations as it doesn't
        manage DTDs.

        Note also that this class is not thread-safe.
        """
        IncrementalHandler.__init__(self, out=None, encoding=ENCODING)
        self._level_dispatchers = {}
        self._element_dispatchers = {}
        self._element_level_dispatchers = {}
        self._path_dispatchers = {}
        self.default_dispatcher = None

        self.disable_dispatching()

    def endElementNS(self, name, qname):
        self._current_level = current_level = self._current_level - 1
        current_element = self._current_el
        dispatched = False
        
        if self.enable_level_dispatching:
            if current_level in self._level_dispatchers:
                self._level_dispatchers[current_level](current_element)
                dispatched = True
        if self.enable_element_dispatching:
            pattern = (current_element.namespaceURI, current_element.localName)
            if pattern in self._element_dispatchers:
                self._element_dispatchers[pattern](current_element)
                dispatched = True
            else:
                pattern = (ANY_NAMESPACE, current_element.localName)
                if pattern in self._element_dispatchers:
                    self._element_dispatchers[pattern](current_element)
                    dispatched = True
        if self.enable_element_by_level_dispatching:
            pattern = (current_level, (current_element.namespaceURI, current_element.localName))
            if pattern in self._element_level_dispatchers:
                self._element_level_dispatchers[pattern](current_element)
                dispatched = True 
            else:
                pattern = pattern = (current_level, (ANY_NAMESPACE, current_element.localName))
                if pattern in self._element_level_dispatchers:
                    self._element_level_dispatchers[pattern](current_element)
                    dispatched = True   
        if self.enable_dispatching_by_path:
            for path in self._path_dispatchers:
                match_found = current_element.xml_xpath(path)
                if match_found:
                    self._path_dispatchers[path](match_found)
                    dispatched = True       
                    break

        if not dispatched and callable(self.default_dispatcher):
            self.default_dispatcher(current_element)
            
        self._current_el = self._current_el.parentNode

class DispatchParser(DefaultDispatchParser):
    def __init__(self, out=None, encoding=ENCODING):
        self.parser = xs.make_parser()
        self.parser.setFeature(xs.handler.feature_namespaces, True)
        if not out:
            out = StringIO.StringIO()
        self.out = out
        self.handler = DispatchHandler(self.out, encoding)
        self.parser.setContentHandler(self.handler)
        self.parser.setProperty(xs.handler.property_lexical_handler, self.handler)
    
if __name__ == '__main__':
    def dispatch(e):
        print e.xml()
        
    d = DispatchParser()
    d.register_on_element('b', dispatch)
    d.feed('<')
    d.feed('a><b')
    d.feed('>hey</b></a')
    d.feed('>')
