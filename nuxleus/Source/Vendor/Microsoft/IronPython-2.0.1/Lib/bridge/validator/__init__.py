#!/usr/bin/env python
# -*- coding: utf-8 -*-

class BridgeValidatorException(StandardError):
    def __init__(self, element=None, message=''):
        self.element = element
        self.message = message

    def __str__(self):
        return self.message
