#!/usr/bin/env python
# -*- coding: utf-8 -*-

from bridge import Element as E
from bridge.common import XMPP_CLIENT_NS

__all__ = ['lookup_first_error']

def lookup_first_error(element):
    for child in element.xml_children:
        if isinstance(child, E):
            if child.xml_name == u'error':
                return child
            lookup_first_error(child)

    return None
