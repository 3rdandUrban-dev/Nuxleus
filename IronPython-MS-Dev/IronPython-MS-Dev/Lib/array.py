# Copyright (c) 2006 Seo Sanghyeon

# This code is derived from PyPy's implementation of array module.
# The main change is the use of .NET array as storage instead of Python list.

# 2006-11-11 sanxiyn Bare minimum to run pycrypto's randpool

from System import Array
from System import Byte

_type_mapping = {
    'B': Byte,
}

class array(object):

    def __new__(cls, typecode, initializer=[]):
        self = object.__new__(cls)
        self.typecode = typecode
        type = _type_mapping[typecode]
        self.array = Array[type](initializer)
        return self

    def __repr__(self):
        if not self.array:
            return "array('%s')" % self.typecode
        else:
            return "array('%s', %s)" % (self.typecode, list(self.array))

    def __getitem__(self, index):
        if isinstance(index, slice):
            sliced = array(self.typecode)
            sliced.array = self.array[index]
            return sliced
        return self.array[index]

    def __setitem__(self, index, value):
        self.array[index] = value
