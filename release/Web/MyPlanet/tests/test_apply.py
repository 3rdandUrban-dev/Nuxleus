#!/usr/bin/env python

import unittest, os, shutil
from planet import config, splice, logger
from xml.dom import minidom

workdir = 'tests/work/apply'
configfile = 'tests/data/apply/config-%s.ini'
testfeed = 'tests/data/apply/feed.xml'

class ApplyTest(unittest.TestCase):
    def setUp(self):
        testfile = open(testfeed)
        self.feeddata = testfile.read()
        testfile.close()

        try:
             os.makedirs(workdir)
        except:
             self.tearDown()
             os.makedirs(workdir)
    
    def tearDown(self):
        shutil.rmtree(workdir)
        os.removedirs(os.path.split(workdir)[0])

    def test_apply_asf(self):
        config.load(configfile % 'asf')
        splice.apply(self.feeddata)

        # verify that selected files are there
        for file in ['index.html', 'default.css', 'images/foaf.png']:
            path = os.path.join(workdir, file)
            self.assertTrue(os.path.exists(path))
            self.assertTrue(os.stat(path).st_size > 0, file + ' has size 0')

        # verify that index.html is well formed, has content, and xml:lang
        html = open(os.path.join(workdir, 'index.html'))
        doc = minidom.parse(html)
        list = []
        content = lang = 0
        for div in doc.getElementsByTagName('div'):
            if div.getAttribute('class') != 'content': continue
            content += 1
            if div.getAttribute('xml:lang') == 'en-us': lang += 1
        html.close()
        self.assertEqual(12, content)
        self.assertEqual(3, lang)

    def test_apply_fancy(self):
        config.load(configfile % 'fancy')
        splice.apply(self.feeddata)

        # verify that selected files are there
        for file in ['index.html', 'planet.css', 'images/jdub.png']:
            path = os.path.join(workdir, file)
            self.assertTrue(os.path.exists(path), path)
            self.assertTrue(os.stat(path).st_size > 0)

        # verify that index.html is well formed, has content, and xml:lang
        html = open(os.path.join(workdir, 'index.html')).read()
        self.assertTrue(html.find('<h1>test planet</h1>')>=0)
        self.assertTrue(html.find(
          '<h4><a href="http://example.com/2">Venus</a></h4>')>=0)

try:
    import libxml2
except ImportError:

    try:
        import win32pipe
        (stdin,stdout) = win32pipe.popen4('xsltproc -V', 't')
        stdin.close()
        stdout.read()
        try:
            exitcode = stdout.close()
        except IOError:
            exitcode = -1
    except:
        import commands
        (exitstatus,output) = commands.getstatusoutput('xsltproc -V')
        exitcode = ((exitstatus>>8) & 0xFF)

    if exitcode:
        logger.warn("xsltproc is not available => can't test XSLT templates")
        for method in dir(ApplyTest):
            if method.startswith('test_'):  delattr(ApplyTest,method)
