#!/usr/bin/env python
# -*- coding: utf-8 -*-

from bridge.common import ATOM10_NS, ATOMPUB_NS, THR_NS, XHTML1_NS

import datetime
from bridge.lib import isodate

__all__ = ['published_after', 'updated_after',
           'published_before', 'updated_before',
           'requires_summary', 'lookup_links',
           'lookup_entry','requires_author', 'valid_categories']

_xml_media_types = ['text/xml', 'application/xml', 'text/xml-external-parsed-entity',
                    'application/xml-external-parsed-entity', 'application/xml-dtd']
    
def _is_before_date(node, dt_pivot, strict=True):
    dt = isodate.parse(str(node))
    dt = datetime.datetime.utcfromtimestamp(dt)
    if strict:
        return dt < dt_pivot
    else:
        return dt <= dt_pivot
    
def _is_after_date(node, dt_pivot, strict=True):
    dt = isodate.parse(str(node))
    dt = datetime.datetime.utcfromtimestamp(dt)
    if strict:
        return dt > dt_pivot
    else:
        return dt >= dt_pivot

def _cmp_date(func, name, element, dt_pivot, strict=True, recursive=False, include_feed=True):
    elements = []
    if element.name == u'feed' and element.xmlns == ATOM10_NS:
        if include_feed:
            if element.has_element(name, ATOM10_NS):
                if func(element.published, dt_pivot, strict):
                    elements.append(element)
            
        if recursive:
            for entry in element.entry:
                if entry.has_element(name, ATOM10_NS):
                    if func(entry.published, dt_pivot, strict):
                        elements.append(entry)        
    elif element.name == u'entry' and element.xmlns == ATOM10_NS:
        if element.has_element(name, ATOM10_NS):
            if func(element.published, dt_pivot, strict):
                elements.append(element) 
                
    return elements

def published_after(element, dt_pivot, strict=True, recursive=False, include_feed=True):
    """
    Returns the list of elements which have been published after the given date.

    Keyword arguments:
    element -- atom feed or entry element
    dt_pivot -- datetime instance to compare to
    strict -- if True only accepts elements which are published strictly after
    the dt_pivot. if False elmeents which published date equal dt_pivot will be included
    in the result
    recursive -- if the element is a feed and recursive is True it will iterate through
    the feed entries as well
    include_feed -- if the element is a feed, recursive is True but you don't want the
    feed element to be part of the result set this to False
    """
    return _cmp_date(_is_after_date, 'published', element, dt_pivot,
                     strict, recursive, include_feed)

def updated_after(element, dt_pivot, strict=True, recursive=False, include_feed=True):
    """
    Returns the list of elements which have been updated  after the given date.

    Keyword arguments:
    element -- atom feed or entry element
    dt_pivot -- datetime instance to compare to
    strict -- if True only accepts elements which are published strictly after
    the dt_pivot. if False elements which published date equals dt_pivot will be included
    in the result
    recursive -- if the element is a feed and recursive is True it will iterate through
    the feed entries as well
    include_feed -- if the element is a feed, recursive is True but you don't want the
    feed element to be part of the result set this to False
    """
    return _cmp_date(_is_after_date, 'updated', element, dt_pivot,
                     strict, recursive, include_feed)

def published_before(element, dt_pivot, strict=True, recursive=False, include_feed=True):
    """
    Returns the list of elements which have been published before the given date.

    Keyword arguments:
    element -- atom feed or entry element
    dt_pivot -- datetime instance to compare to
    strict -- if True only accepts elements which are published strictly before
    the dt_pivot. if False elements which published date equals dt_pivot will be included
    in the result
    recursive -- if the element is a feed and recursive is True it will iterate through
    the feed entries as well
    include_feed -- if the element is a feed, recursive is True but you don't want the
    feed element to be part of the result set this to False
    """
    return _cmp_date(_is_before_date, 'published', element, dt_pivot,
                     strict, recursive, include_feed)

def updated_before(element, dt_pivot, strict=True, recursive=False, include_feed=True):
    """
    Returns the list of elements which have been updated before the given date.

    Keyword arguments:
    element -- atom feed or entry element
    dt_pivot -- datetime instance to compare to
    strict -- if True only accepts elements which are updated strictly before
    the dt_pivot. if False elements which updated date equals dt_pivot will be included
    in the result
    recursive -- if the element is a feed and recursive is True it will iterate through
    the feed entries as well
    include_feed -- if the element is a feed, recursive is True but you don't want the
    feed element to be part of the result set this to False
    """
    return _cmp_date(_is_before_date, 'updated', element, dt_pivot,
                     strict, recursive, include_feed)


def lookup_entry(element, id):
    """
    Returns the first entry matching the id provided in parameter
    """
    if element.has_child(u'entry', ATOM10_NS):
        entries = element.get_children(u'entry', ATOM10_NS)
        for entry in entries:
            entry_id = entry.get_child('id', ATOM10_NS)
            if entry_id.xml_text == id:
                return entry
                
    return None

def lookup_links(element, **kwargs):
    """
    returns a list of links matching the attributes passed as parameters.
    For instance:

    lookup_links(entry, rel=u'alternate', type=u'text/html')
    """
    results = []
    if element.has_child(u'link', ATOM10_NS):
        links = element.get_children(u'link', ATOM10_NS)
        for link in links:
            candidate = False
            for arg in kwargs:
                attr = link.get_attribute(arg)
                if attr:
                    value = kwargs.get(arg)
                    if value == attr.xml_text:
                        candidate = True
                    else:
                        candidate = False
                        break
                else:
                    candidate = False
                    break

            if candidate:
                results.append(link)
                
    return results

def requires_summary(element):
    """
    Returns True if the entry requires an atom:summary
    to be added based on section 4.1.2 of RFC 4287.

    Keyword argument:
    element -- entry element
    """
    # atom:entry elements MUST contain an atom:summary element in either of the following cases:
    #   * the atom:entry contains an atom:content that has a "src" attribute (and is thus empty).
    #   * the atom:entry contains content that is encoded in Base64; i.e.,
    #     the "type" attribute of atom:content is a MIME media type [MIMEREG],
    #     but is not an XML media type [RFC3023], does not begin with "text/",
    #     and does not end with "/xml" or "+xml".
    needs_summary = False
    content = element.get_child('content', ATOM10_NS)
    if content:
        src = content.get_attribute_ns('src', ATOM10_NS)
        mime_type = content.get_attribute_ns('type', ATOM10_NS)
        if src:
            needs_summary = True
        elif mime_type:
            mime_type = mime_type.xml_text
            if mime_type not in _xml_media_types:
                needs_summary = True
            if mime_type.startswith("text/"):
                needs_summary = False
            if mime_type.endswith("/xml") or mime_type.endswith("+xml"):
                needs_summary = False
    else:
        needs_summary = True
    return needs_summary

def requires_author(element):
    """
    Returns True if the entry requires an atom:author
    to be added based on section 4.1.2 of RFC 4287.

    Keyword argument:
    element -- entry element
    """
    # atom:entry elements MUST contain one or more atom:author elements,
    # unless the atom:entry contains an atom:source element
    # that contains an atom:author element or, in an Atom Feed Document,
    # the atom:feed element contains an atom:author element itself.
    needs_author = False
    author = element.get_child('author', ATOM10_NS)
    if not author:
        needs_author = True
        source = element.get_child('source', ATOM10_NS)
        if source:
            author = source.get_child('author', ATOM10_NS)
            if author:
                needs_author = False
        if element.xml_parent and element.xml_parent.xml_name == 'feed' and \
           element.xml_parent.xml_prefix == element.xml_prefix and \
           element.xml_parent.xml_ns == ATOM10_NS:
            author = element.xml_parent.get_child('author', ATOM10_NS)
            if author:
               needs_author = False
    return needs_author

def valid_categories(element, test_set, matching=None):

    # The app:categories element can contain a "fixed" attribute, with a
    # value of either "yes" or "no", indicating whether the list of
    # categories is a fixed or an open set.  Newly created or updated
    # members whose categories are not listed in the Collection Document
    # MAY be rejected by the server.  Collections that indicate the set is
    # open SHOULD NOT reject otherwise acceptable members whose categories
    # are not listed in the Collection.
    
    categories = element.get_children('category', ATOM10_NS)
    if not matching:
        matching = ['term']
    for candidate in test_set:
        valid = False
        for current in categories:
            for token in matching:
                current_attr = current.get_attribute(token)
                if current_attr:
                    candidate_attr = candidate.get_attribute(token)
                    if current_attr.xml_text == candidate_attr.xml_text:
                        valid = True
                    else:
                        valid = False
                        break
                else:
                    valid = False
                    break
            if valid:
                return True

    return valid


def fetch_empty_authors(element, matching=None):
    """
    Return a list of atom:author elements which have an empty text for the
    children specified in 'matching' (which if not provided defaults to "name")

    fetch_empty_authors(entry, matching=['name', 'email'])
    """
    if not matching:
        matching = ['name', 'email', 'uri']
    result = []
    authors = element.get_children('author', ATOM10_NS)
    for author in authors:
        for token in matching:
            if not author.has_child(token, ATOM10_NS):
                result.append(author)
            else:
                child = author.get_child(token, ATOM10_NS)
                if not child.xml_text:
                    result.append(author)

    return result
