#!/usr/bin/env python
# -*- coding: utf-8 -*-
import re, sys

__all__ = ['get_first_available_parser']

def get_first_available_parser():
    """
    Helper function which will return the first available parser
    on your system.
    """
    if sys.platform == 'cli':
        try:
            from bridge.parser.bridge_dotnet import Parser
            return Parser
        except ImportError:
            pass
    
    from bridge.parser.bridge_default import Parser
    
    return Parser

if sys.platform == 'cli':
    from bridge.parser.bridge_dotnet import Parser, IncrementalParser, DispatchParser
else:
    from bridge.parser.bridge_default import Parser, IncrementalParser, DispatchParser
