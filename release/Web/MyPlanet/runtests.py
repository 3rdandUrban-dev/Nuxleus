#!/usr/bin/python
import glob, unittest, os, sys

# python 2.2 accomodations
try:
    from trace import fullmodname
except:
    def fullmodname(path):
        return os.path.splitext(path)[0].replace(os.sep, '.')

# more python 2.2 accomodations
if not hasattr(unittest.TestCase, 'assertTrue'):
    unittest.TestCase.assertTrue = unittest.TestCase.assert_
if not hasattr(unittest.TestCase, 'assertFalse'):
    unittest.TestCase.assertFalse = unittest.TestCase.failIf

# try to start in a consistent, predictable location
if sys.path[0]: os.chdir(sys.path[0])
sys.path[0] = os.getcwd()

# find all of the planet test modules
modules = map(fullmodname, glob.glob(os.path.join('tests', 'test_*.py')))

# enable warnings
import planet
planet.getLogger("WARNING",None)

# load all of the tests into a suite
try:
    suite = unittest.TestLoader().loadTestsFromNames(modules)
except Exception, exception:
    # attempt to produce a more specific message
    for module in modules: __import__(module)
    raise

verbosity = 1
if "-q" in sys.argv or '--quiet' in sys.argv:
    verbosity = 0
if "-v" in sys.argv or '--verbose' in sys.argv:
    verbosity = 2

# run test suite
unittest.TextTestRunner(verbosity=verbosity).run(suite)
