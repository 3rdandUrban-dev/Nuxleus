#!/usr/bin/env python
# -*- coding: utf-8 -*-

#############################################
# Use IronPython via the System.Xml assembly
#############################################

import os.path


__all__ = ['Parser', 'IncrementalParser', 'DispatchParser']

import clr
clr.AddReference('System.Xml')
from System.Xml import XmlReader, XmlTextReader, XmlNodeType, \
     XmlWriterSettings, XmlReaderSettings, XmlWriter, XmlDocument
from System import Enum, Array, Byte, String, Char
from System.IO import StringReader, Stream, MemoryStream, BinaryReader,  StreamReader, SeekOrigin
from System.Text import Encoding

from xml.sax.saxutils import unescape

import bridge
from bridge import Element as E
from bridge import Attribute as A
from bridge import Document, Comment, PI, Element
from bridge.common import XMLNS_NS, ANY_NAMESPACE

raw = Encoding.GetEncoding('iso-8859-1')

class Parser(object):
    def handle_start_element(self, reader, parent=None):
        prefix = reader.Prefix
        if prefix == '':
            prefix = None
        ns = reader.NamespaceURI
        if ns == '':
            ns = None
            
        e = E(reader.LocalName, prefix=prefix, namespace=ns, parent=parent)
        while reader.MoveToNextAttribute():
            prefix = reader.Prefix
            if prefix == '':
                prefix = None
            ns = reader.NamespaceURI
            if ns == '':
                ns = None
            A(reader.LocalName, reader.Value, prefix=prefix,
              namespace=ns, parent=e)
            
        return e

    def handle_end_element(self, parent):
        text_only = True
        for child in parent.xml_children:
            if not isinstance(child, str):
                text_only = False
                break
        if text_only:
            parent.xml_text = ''.join(parent.xml_children)
            parent.xml_children = []
        
    def _deserialize(self, reader, parent):
        
        while reader.Read():
            node_type = reader.NodeType
            if node_type == XmlNodeType.Element:
                current = self.handle_start_element(reader, parent)
                self._deserialize(reader, current)
            elif node_type == XmlNodeType.EndElement:
                self.handle_end_element(parent)
                return
            elif node_type == XmlNodeType.Text:
                parent.xml_children.append(reader.Value)
            elif node_type == XmlNodeType.Comment:
                Comment(data=reader.Value.Trim(), parent=parent)
            elif node_type == XmlNodeType.ProcessingInstruction:
                PI(target=reader.Name, data=reader.Value, parent=parent)
            elif node_type == XmlNodeType.CDATA:
                parent.as_cdata = True
                parent.xml_children.append(reader.Value)


    def deserialize(self, source, prefixes=None, strict=False, as_attribute=None, as_list=None,
                    as_attribute_of_element=None):
        
        if isinstance(source, basestring):
            try:
                if os.path.exists(source):
                    xtr = XmlTextReader(StreamReader(source))
            except ValueError:
                xtr = XmlTextReader(StringReader(source))
            else:
                xtr = XmlTextReader(StringReader(source))
        elif hasattr(source, 'read'):
            xtr = XmlTextReader(StringReader(source.read()))

        settings = XmlReaderSettings()
        settings.ProhibitDtd = True
        reader = XmlReader.Create(xtr, settings)
        
        document = Document()
        document.as_attribute = as_attribute or {}
        document.as_list = as_list or {}
        document.as_attribute_of_element = as_attribute_of_element or {}

        self._deserialize(reader, document)

        xtr.Close()
        reader.Close()

        return document

    def __qname(self, name, prefix=None):
        if prefix:
            return "%s:%s" % (prefix, name)
        return name

    def __attrs(self, node, element):
        for attr in element.xml_attributes:
            name = attr._local_name
            if attr.xml_ns and attr.xml_ns != XMLNS_NS:
                node.SetAttribute(name, attr.xml_ns, attr.xml_text)
            elif not attr.xml_ns:
                node.SetAttribute(name, attr.xml_text)

    def __start_element(self, doc, element):
        if element.xml_ns:
            return doc.CreateElement(element.xml_prefix, element._local_name, element.xml_ns)
        else:
            return doc.CreateElement(element._local_name)

    def __serialize_element(self, root, node, element):
        self.__attrs(node, element)
        children = element.xml_children
        for child in children:
            if isinstance(child, basestring):
                if element.as_cdata:
                    node.AppendChild(root.CreateCDataSection(child))
                else:
                    node.AppendChild(root.CreateTextNode(child))
            elif isinstance(child, Comment):
                node.AppendChild(root.CreateComment(child.data))
            elif isinstance(child, PI):
                node.AppendChild(root.CreateProcessingInstruction(child.target, child.data))
            elif isinstance(child, E):
                child_node = self.__start_element(root, child)
                
                if child.xml_text:
                    if child.as_cdata:
                        child_node.AppendChild(root.CreateCDataSection(child.xml_text))
                    else:
                        child_node.AppendChild(root.CreateTextNode(child.xml_text))
                    
                self.__serialize_element(root, child_node, child)

                node.AppendChild(child_node)
                
    def __start_document(self, root):
        if root.xml_ns and root.xml_prefix:
            return '<%s:%s xmlns:%s="%s" />' % (root.xml_prefix, root._local_name,
                                                root.xml_prefix, root.xml_ns)
        elif root.xml_ns:
            return '<%s xmlns="%s" />' % (root._local_name, root.xml_ns)
        
        return '<%s />' % (root._local_name, )
    
    def serialize(self, document, indent=False, encoding=bridge.ENCODING, prefixes=None, omit_declaration=False):
        doc = XmlDocument()
        doc.LoadXml(self.__start_document(document))
        if document.xml_text:
            doc.DocumentElement.AppendChild(doc.CreateTextNode(document.xml_text))
        self.__serialize_element(doc, doc.DocumentElement, document)

        settings = XmlWriterSettings()
        settings.Indent = indent
        settings.Encoding = Encoding.GetEncoding(encoding)
        settings.OmitXmlDeclaration = omit_declaration

        ms = MemoryStream()
        xw = XmlWriter.Create(ms, settings)
        doc.Save(xw)
        sr = StreamReader(ms)
        ms.Seek(0, SeekOrigin.Begin)
        content = sr.ReadToEnd()
        ms.Close()

        return content

class Fragment(object):
    def __init__(self, outter=None):
        self.outter = outter
        self.start_tag_complete = False
        self.element = None
        self.prolog = ''
        self.data = []
        self.sub_elements = []
        self.ns = {}
        
    def __repr__(self):
        return "<Fragment at %s>" % hex(id(self))

    def __str__(self):
        return self.prolog + ''.join(self.data)

class BridgeBufferedParser(object):
    def __init__(self):
        self.on_closed_tag = None
        self.on_start_tag_completed = None
        self.prolog = '<?xml version="1.0" encoding="utf-8"?>'
        self.reset()

    def reset(self):
        self.fragment = Fragment()
        self.last_was_opening_tag = False
        self.last_was_slash = False
        self.in_closing_tag = False
        self.found_prolog = False
        self._depth = 0
        
    def lookup_prolog(self, chunk):
        start_pos = chunk.find('<?xml')
        if start_pos >= 0:
            end_pos = chunk.find('?>')
            if end_pos > start_pos:
                self.found_prolog = True
                self.prolog = chunk[start_pos:end_pos] + '?>'
                return True

        return False

    def _lookup_ns(self, tag):
        #self.fragment.ns.update(self.fragment.outter.ns)
        while True:
            pos = tag.find('xmlns')
            if pos >= 0:
                start_at = pos + 5
                pos = tag.find('=', start_at)
                uses_single_quote = tag[pos+1] == "'"
                prefix = ns = None
                if tag[start_at] == ':': 
                    prefix = tag[start_at+1:pos]
                if pos >= 0:
                    start_at = pos + 2
                    if uses_single_quote:
                        end_at = tag.find("'", start_at)
                    else:
                        end_at = tag.find('"', start_at)
                    ns = tag[start_at:end_at]
                self.fragment.ns[prefix] = ns
                tag = tag[end_at+2:]
            else:
                break

        fragment_in_parent_not_in_current = {}
        parent_ns = {}
        if self.fragment.outter:
            parent_ns = self.fragment.outter.ns
        for prefix in parent_ns:
            ns = parent_ns[prefix]
            if prefix not in self.fragment.ns:
                if prefix:
                    self.fragment.data.append(' xmlns:%s="%s"' % (prefix, ns))
                else:
                    self.fragment.data.append(' xmlns="%s"' % (ns,))

    def feed(self, chunk):
        if not chunk:
            return

        if not self.found_prolog:
            self.lookup_prolog(chunk)
            
        chunk = list(chunk)

        if self.last_was_opening_tag:
            chunk.insert(0, '<')
            self.last_was_opening_tag = False

        if self.last_was_slash:
            chunk.insert(0, '/')
            self.last_was_slash = False
        
        if chunk and chunk[-1] == '<':
            self.last_was_opening_tag = True
            chunk = chunk[:-1]
            
        if chunk and chunk[-1] == '/':
            self.last_was_slash = True
            chunk = chunk[:-1]

        previous = None
        for c in chunk:
            if c == '>' and (previous == '/' or self.in_closing_tag):
                self.fragment.data.append(c)
                self._depth = self._depth - 1
                fragment = self.fragment
                self.fragment = self.fragment.outter
                if self.fragment:
                    self.fragment.data.extend(fragment.data)
                self.last_was_slash = False
                self.in_closing_tag = False
                if self.on_closed_tag:
                    self.on_closed_tag(fragment, self._depth)
            elif c == '>' and previous not in ['!', '[', '?'] and not self.fragment.start_tag_complete:
                start_tag = ''.join(self.fragment.data)
                self._lookup_ns(start_tag)
                self.fragment.element = self.on_start_tag_completed(self.fragment)
                self.fragment.data.append(c)
                self.fragment.start_tag_complete = True
            elif previous == '<' and c == '/':
                self.fragment.data.append(previous)
                self.fragment.data.append(c)
                self.in_closing_tag = True
            elif previous == '<' and c not in ['!', '[', '?']:
                self.found_prolog = True
                self.fragment = Fragment(self.fragment)
                self._depth = self._depth + 1
                self.fragment.prolog = self.prolog
                self.fragment.data.append(previous)
                self.fragment.data.append(c)
            elif previous == '<' and c in ['!', '[', '?']:
                self.fragment.data.append(previous)
                self.fragment.data.append(c)
            elif c != '<':
                self.fragment.data.append(c)

            previous = c
    
class IncrementalParser(Parser):
    def __init__(self):
        Parser.__init__(self)
        self.settings = XmlReaderSettings()
        self.settings.ProhibitDtd = True
        self.settings.CloseInput = False

        self.buffered_parser = BridgeBufferedParser()
        self.buffered_parser.on_closed_tag = self._handle_fragment
        self.buffered_parser.on_start_tag_completed = self._handle_empty_fragment

    def feed(self, data):
        self.buffered_parser.feed(data)

    def reset(self):
        self.buffered_parser.reset()

    def close(self):
        self.reset()

    def _handle_fragment(self, fragment, depth):
        xtr = XmlTextReader(StringReader(str(fragment)))
        reader = XmlReader.Create(xtr, self.settings)
        sub_elements = iter(fragment.sub_elements)
        self._deserialize(reader, fragment, sub_elements, depth)

    def _handle_empty_fragment(self, fragment):
        xtr = XmlTextReader(StringReader(str(fragment) + '/>'))
        reader = XmlReader.Create(xtr, self.settings)
        while reader.Read():
            node_type = reader.NodeType
            if node_type == XmlNodeType.Element:
                return self.handle_start_element(reader)

    def _deserialize(self, reader, fragment, sub_elements, depth):
        while reader.Read():
            if reader.Depth not in [0, 1]:
                continue
            node_type = reader.NodeType
            if node_type == XmlNodeType.Element:
                if reader.Depth == 0:
                    is_empty_element = reader.IsEmptyElement
                    current = self.handle_start_element(reader)
                    if is_empty_element:
                        if fragment.outter:
                            current.xml_parent = fragment.outter.element
                            fragment.outter.sub_elements.append(current)
                        self.handle_end_element(current, depth)
                elif reader.Depth == 1:
                    element = sub_elements.next()
                    element.xml_parent = current
                    current.xml_children.append(element)
            elif node_type == XmlNodeType.EndElement:
                if reader.Depth == 0:
                    if fragment.outter:
                        current.xml_parent = fragment.outter.element
                        fragment.outter.sub_elements.append(current)
                    self.handle_end_element(current, depth)
            elif node_type == XmlNodeType.Text:
                current.xml_children.append(reader.Value)
            elif node_type == XmlNodeType.Comment:
                Comment(data=reader.Value.Trim(), parent=current)
            elif node_type == XmlNodeType.ProcessingInstruction:
                PI(target=reader.Name, data=reader.Value, parent=current)
            elif node_type == XmlNodeType.CDATA:
                current.as_cdata = True
                current.xml_children.append(reader.Value)

    def handle_start_element(self, reader):
        current = Parser.handle_start_element(self, reader)
        return current
    
    def handle_end_element(self, parent, depth):
        Parser.handle_end_element(self, parent)
        
    def doc(self):
        return self._root
        
class DispatchParser(IncrementalParser):
    def __init__(self):
        IncrementalParser.__init__(self)
        self._level_dispatchers = {}
        self._element_dispatchers = {}
        self._path_dispatchers = {}
        self._element_level_dispatchers = {}
        self.default_dispatcher = None
        self.disable_dispatching()
        
    def disable_dispatching(self):
        self.default_dispatcher = None
        self.enable_level_dispatching = False
        self.enable_element_dispatching = False
        self.enable_dispatching_by_path = False
        self.enable_element_by_level_dispatching = False

    def enable_dispatching(self):
        self.enable_level_dispatching = True
        self.enable_element_dispatching = True
        self.enable_dispatching_by_path = True
        self.enable_element_by_level_dispatching = True

    def register_default(self, handler):
        self.default_dispatcher = handler

    def unregister_default(self):
        self.default_dispatcher = None
          
    def register_at_level(self, level, dispatcher):
        """Registers a dispatcher at a given level within the
        XML tree of elements being built.

        The ``level``, an integer, is zero-based. So the root
        element of the XML tree is 0 and its direct children
        are at level 1.

        The ``dispatcher`` is a callable object only taking
        one parameter, a bridge.Element instance.
        """
        self.enable_level_dispatching = True
        self._level_dispatchers[level] = dispatcher

    def unregister_at_level(self, level):
        """Unregisters a dispatcher at a given level
        """
        if level in self._level_dispatchers:
            del self._level_dispatchers[level]
        if len(self._level_dispatchers) == 0:
            self.enable_level_dispatching = False
              
    def register_on_element(self, local_name, dispatcher, namespace=None):
        """Registers a dispatcher on a given element met during
        the parsing.

        The ``local_name`` is the local name of the element. This
        element can be namespaced if you provide the ``namespace``
        parameter.

        The ``dispatcher`` is a callable object only taking
        one parameter, a bridge.Element instance.
        """
        self.enable_element_dispatching = True
        self._element_dispatchers[(namespace, local_name)] = dispatcher

    def unregister_on_element(self, local_name, namespace=None):
        """Unregisters a dispatcher for a specific element.
        """
        key = (namespace, local_name)
        if key in self._element_dispatchers:
            del self._element_dispatchers[key]
        if len(self._element_dispatchers) == 0:
            self.enable_element_dispatching = False
         
    def register_on_element_per_level(self, local_name, level, dispatcher, namespace=None):
        """Registers a dispatcher at a given level within the
        XML tree of elements being built as well as for a
        specific element.

        The ``level``, an integer, is zero-based. So the root
        element of the XML tree is 0 and its direct children
        are at level 1.

        The ``local_name`` is the local name of the element. This
        element can be namespaced if you provide the ``namespace``
        parameter.

        The ``dispatcher`` is a callable object only taking
        one parameter, a bridge.Element instance.
        """
        self.enable_element_by_level_dispatching = True
        self._element_level_dispatchers[(level, (namespace, local_name))] = dispatcher

    def unregister_on_element_per_level(self, local_name, level, namespace=None):
        """Unregisters a dispatcher at a given level for a specific
        element.
        """
        key = (level, (namespace, local_name))
        if key in self._element_level_dispatchers:
            del self._element_level_dispatchers[key]
        if len(self._element_level_dispatchers) == 0:
            self.enable_element_by_level_dispatching = False

    def register_by_path(self, path, dispatcher):
        self.enable_dispatching_by_path = True
        self._path_dispatchers[path] = dispatcher

    def unregister_by_path(self, path):
        if path in self._path_dispatchers:
            del self._path_dispatchers[path]
        if len(self._path_dispatchers) == 0:
            self.enable_dispatching_by_path = False
   
    def handle_start_element(self, reader):
        return IncrementalParser.handle_start_element(self, reader)
        
    def handle_end_element(self, parent, depth):
        IncrementalParser.handle_end_element(self, parent, depth)
        current_level = depth
        current_element = parent
        dispatched = False

        if self.enable_level_dispatching:
            if current_level in self._level_dispatchers:
                self._level_dispatchers[current_level](current_element) 
                dispatched = True       
        if self.enable_element_dispatching:
            pattern = (current_element.xml_ns, current_element.xml_name)
            if pattern in self._element_dispatchers:
                self._element_dispatchers[pattern](current_element) 
                dispatched = True 
            else:
                pattern = (ANY_NAMESPACE, current_element.xml_name)
                if pattern in self._element_dispatchers:
                    self._element_dispatchers[pattern](current_element)
                    dispatched = True      
        if self.enable_element_by_level_dispatching:
            pattern = (current_level, (current_element.xml_ns, current_element.xml_name))
            if pattern in self._element_level_dispatchers:
                self._element_level_dispatchers[pattern](current_element) 
                dispatched = True  
            else:
                pattern = pattern = (current_level, (ANY_NAMESPACE, current_element.xml_name))
                if pattern in self._element_level_dispatchers:
                    self._element_level_dispatchers[pattern](current_element)
                    dispatched = True        
        if self.enable_dispatching_by_path:
            for path in self._path_dispatchers:
                match_found = current_element.filtrate(lookup, path=path)
                if match_found:
                    self._path_dispatchers[path](match_found) 
                    dispatched = True       
                    break

        if not dispatched and callable(self.default_dispatcher):
            self.default_dispatcher(current_element)
