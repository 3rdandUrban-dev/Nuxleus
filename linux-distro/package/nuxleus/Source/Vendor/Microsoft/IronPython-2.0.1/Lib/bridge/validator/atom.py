#!/usr/bin/env python
# -*- coding: utf-8 -*-

from urlparse import urlparse

from bridge.filter.atom import valid_categories
from bridge.validator import BridgeValidatorException

ATOM10_NS = u'http://www.w3.org/2005/Atom'
ATOMPUB_NS = u'http://purl.org/atom/app#'
XHTML1_NS = u'http://www.w3.org/1999/xhtml'
THR_NS = u'http://purl.org/syndication/thread/1.0'

__all__ = ['id_as_url']

def id_as_url(element):
    """
    Is the an atom:id a valid URL. If not it will
    raise a BridgeValidatorException.

    Keyword arguments:
    element -- a bridge.Element element. Either and atom:id element
    or an element that has an atom:id child.
    """
    if element.name == 'id' and element.xmlns == ATOM10_NS:
        url = str(element)
        scheme, netloc, path, parameters, query, fragment = urlparse(url)
        if not scheme or not netloc:
            raise BridgeValidatorException, element
    elif element.has_element('id', ATOM10_NS):
        url = str(element.id)
        scheme, netloc, path, parameters, query, fragment = urlparse(url)
        if not scheme or not netloc:
            raise BridgeValidatorException(element.id)

def respect_fixed_categories(element, test_set, matching=None):    
    # The app:categories element can contain a "fixed" attribute, with a
    # value of either "yes" or "no", indicating whether the list of
    # categories is a fixed or an open set.  Newly created or updated
    # members whose categories are not listed in the Collection Document
    # MAY be rejected by the server.  Collections that indicate the set is
    # open SHOULD NOT reject otherwise acceptable members whose categories
    # are not listed in the Collection.

    respect = valid_categories(element, test_set=test_set, matching=matching)
    if not respect:
        raise BridgeValidatorException
