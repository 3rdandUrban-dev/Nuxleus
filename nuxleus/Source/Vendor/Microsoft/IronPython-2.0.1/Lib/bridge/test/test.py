import unittest

from test_default import TestDefault
from test_amara import TestAmara
from test_lxml import TestLXML
from test_elementtree import TestElementTree

if __name__ == '__main__':
    suite = unittest.TestSuite()
    suite.addTest(unittest.makeSuite(TestDefault))
    suite.addTest(unittest.makeSuite(TestAmara))
    suite.addTest(unittest.makeSuite(TestLXML))
    suite.addTest(unittest.makeSuite(TestElementTree))
    unittest.TextTestRunner(verbosity=2).run(suite)
